using UnityEngine;

/// <summary>
/// Waits for the player to approach, moves on one fixed axis, blinks once
/// when it touches a solid surface, then returns home. PlayerLife handles death.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public sealed class SpikeHead : MonoBehaviour
{
    public enum ChargeDirection { Left, Right, Up, Down }
    private enum State { Idle, Warning, Moving, ImpactBlink, Returning }

    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float detectionRange = 3f;

    [Header("Movement")]
    [SerializeField] private ChargeDirection chargeDirection = ChargeDirection.Down;
    [SerializeField, Min(0.1f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float warningTime = 0.5f;
    [SerializeField, Min(0.1f)] private float maxTravelFromHome = 50f;
    [SerializeField, Min(0.01f)] private float homeDistance = 0.03f;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] blinkFrames;
    [SerializeField, Min(1f)] private float framesPerSecond = 20f;

    [Header("Collision check")]
    [SerializeField, Min(0f)] private float castSkin = 0.001f;

    private readonly RaycastHit2D[] castHits = new RaycastHit2D[16];
    private Rigidbody2D body;
    private Collider2D headCollider;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private ContactFilter2D obstacleFilter;
    private Vector2 homePosition;
    private Vector2 moveDirection;
    private State state;
    private float stateTimer;
    private float distanceFromHome;
    private bool canStart;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        headCollider = GetComponent<Collider2D>();
        boxCollider = headCollider as BoxCollider2D;
        spriteRenderer = GetComponent<SpriteRenderer>();
        homePosition = body.position;

        if (boxCollider != null && (boxCollider.size.x < 0.01f || boxCollider.size.y < 0.01f))
            boxCollider.size = new Vector2(0.54f, 0.52f);

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.useFullKinematicContacts = true;

        obstacleFilter = new ContactFilter2D();
        obstacleFilter.SetLayerMask(Physics2D.AllLayers);
        obstacleFilter.useTriggers = false;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        SetSprite(idleSprite);
    }

    private void Update()
    {
  
        if (player != null && Vector2.Distance(body.position, player.position) > detectionRange)
            canStart = true;

        switch (state)
        {
            case State.Idle:
                SetSprite(idleSprite);
                if (canStart && player != null && Vector2.Distance(body.position, player.position) <= detectionRange)
                {
                    canStart = false;
                    stateTimer = 0f;
                    state = State.Warning;
                }
                break;

            case State.Warning:
                SetSprite(idleSprite);
                stateTimer += Time.deltaTime;
                if (stateTimer >= warningTime)
                    BeginMovement();
                break;

            case State.Moving:
        
                SetSprite(idleSprite);
                break;

            case State.ImpactBlink:
                PlayBlinkOnce();
                break;

            case State.Returning:
                SetSprite(idleSprite);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Moving)
        {
            float step = moveSpeed * Time.fixedDeltaTime;
            float distanceLeft = maxTravelFromHome - distanceFromHome;
            if (distanceLeft <= 0f)
            {
                BeginReturn();
                return;
            }
            step = Mathf.Min(step, distanceLeft);

            int hitCount = headCollider.Cast(moveDirection, obstacleFilter, castHits, step + castSkin);
            int nearestIndex = -1;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = castHits[i].collider;
                if (hitCollider == null || hitCollider == headCollider || !hitCollider.enabled)
                    continue;

                if (castHits[i].distance < nearestDistance)
                {
                    nearestDistance = castHits[i].distance;
                    nearestIndex = i;
                }
            }

            if (nearestIndex >= 0 && nearestDistance <= step + 0.0001f)
            {
                RaycastHit2D hit = castHits[nearestIndex];
                body.MovePosition(body.position + moveDirection * nearestDistance);
                distanceFromHome += nearestDistance;

                if (hit.transform.root.CompareTag("Player"))
                {
              
                    BeginReturn();
                    return;
                }

                moveDirection = -moveDirection;
                stateTimer = 0f;
                state = State.ImpactBlink;
                return;
            }

            body.MovePosition(body.position + moveDirection * step);
            distanceFromHome += step;

            if (distanceFromHome >= maxTravelFromHome)
                BeginReturn();
        }
        else if (state == State.Returning)
        {
            Vector2 next = Vector2.MoveTowards(body.position, homePosition, moveSpeed * Time.fixedDeltaTime);
            body.MovePosition(next);

            if (Vector2.Distance(next, homePosition) <= homeDistance)
            {
                body.position = homePosition;
                distanceFromHome = 0f;
                state = State.Idle;
                SetSprite(idleSprite);
            }
        }
    }

    private void BeginMovement()
    {
        switch (chargeDirection)
        {
            case ChargeDirection.Left: moveDirection = Vector2.left; break;
            case ChargeDirection.Right: moveDirection = Vector2.right; break;
            case ChargeDirection.Up: moveDirection = Vector2.up; break;
            default: moveDirection = Vector2.down; break;
        }

        distanceFromHome = 0f;
        state = State.Moving;
        SetSprite(idleSprite);
    }

    private void BeginReturn()
    {
        state = State.Returning;
        SetSprite(idleSprite);
    }

    private void PlayBlinkOnce()
    {
        if (blinkFrames == null || blinkFrames.Length == 0 || framesPerSecond <= 0f)
        {
            BeginReturn();
            return;
        }

        stateTimer += Time.deltaTime;
        int frame = Mathf.FloorToInt(stateTimer * framesPerSecond);
        if (frame >= blinkFrames.Length)
        {
            BeginReturn();
            return;
        }

        SetSprite(blinkFrames[frame]);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != State.Moving)
            return;

        if (collision.transform.root.CompareTag("Player"))
        {
            BeginReturn();
            return;
        }

        moveDirection = -moveDirection;
        stateTimer = 0f;
        state = State.ImpactBlink;
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite != null)
            spriteRenderer.sprite = sprite;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}