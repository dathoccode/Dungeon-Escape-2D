using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    SpriteRenderer sprite;
    Animator anim;
    Vector2 moveDir;
    private float lastFacingDir;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float dashForce = 3f;
    [SerializeField] float dashTime = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] AudioSource audioSource;
    private bool hasDash = true;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashStartTime;

    private enum MovementState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        JumpingSecond,
        WallJumping,
        Dashing
    }

    private MovementState state = MovementState.Idle;

    private int numberOfJumps = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        UpdateAnimationUpdate();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            float elapsedTime = Time.time - dashStartTime;
            if (elapsedTime < dashTime)
            {
                rb.linearVelocity = dashDirection * (dashForce / dashTime);
            }
            else
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void Move()
    {
        moveDir = InputHandler.Instance.MoveDirection;
        rb.linearVelocity = new Vector2(moveDir.x * moveSpeed, rb.linearVelocity.y);

        if (rb.linearVelocityX != 0) lastFacingDir = rb.linearVelocityX;

        // jump logic
        if (moveDir.y > 0 && numberOfJumps > 0)
        {
            audioSource.Play();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            numberOfJumps--;
        }

        if (IsGrounded())
        {
            numberOfJumps = 2;
            hasDash = true;
        }
    }
    private void UpdateAnimationUpdate()
    {
        state = MovementState.Idle;

        if (rb.linearVelocityX != 0) state = MovementState.Running;
        sprite.flipX = lastFacingDir < 0;

        if (rb.linearVelocity.y > .1f)
        {
            state = MovementState.Jumping;
        }
        if (numberOfJumps == 0)
        {
            state = MovementState.JumpingSecond;
        }
        if (rb.linearVelocity.y < -.1f)
        {
            state = MovementState.Falling;
        }

        

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        float boxCastDistance = 0.1f;
        float delay = 0.05f;
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, boxCastDistance, groundLayer) && rb.linearVelocity.y < delay;

    }
}
