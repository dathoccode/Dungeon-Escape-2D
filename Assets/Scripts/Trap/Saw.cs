using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public sealed class SawTrap : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Sprite[] rotatingFrames;
    [SerializeField, Min(1f)] private float framesPerSecond = 20f;

    [Header("Patrol (optional)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField, Min(0.01f)] private float arrivalDistance = 0.03f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Transform destination;
    private float frameTimer;
    private int frameIndex;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // MovePosition keeps the patrol aligned with 2D physics.
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (pointA != null && pointB != null)
        {
            body.position = pointA.position;
            destination = pointB;
        }
    }

    private void Update()
    {
        AnimateSaw();
    }

    private void FixedUpdate()
    {
        if (destination == null || moveSpeed <= 0f)
            return;

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            destination.position,
            moveSpeed * Time.fixedDeltaTime);

        body.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, destination.position) <= arrivalDistance)
            destination = destination == pointA ? pointB : pointA;
    }

    private void AnimateSaw()
    {
        if (rotatingFrames == null || rotatingFrames.Length == 0 || framesPerSecond <= 0f)
            return;

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            frameIndex = (frameIndex + 1) % rotatingFrames.Length;
        }

        spriteRenderer.sprite = rotatingFrames[frameIndex];
    }
}
