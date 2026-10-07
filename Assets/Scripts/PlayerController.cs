using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController - di chuyển và nhảy của nhân vật (Dungeon Escape 2D).
/// Unity 6 + Input System mới.
/// Các thuộc tính công khai (State, IsGrounded, FacingDirection, Velocity)
/// chính là "IPlayerState" trong biểu đồ thành phần để GameManager/AudioManager/UIManager đọc.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle = 0, Running = 1, Jumping = 2, DoubleJumping = 3, Falling = 4 }

    [Header("Input (kéo từ InputSystem_Actions > Player)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Di chuyển ngang")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float groundAcceleration = 90f;   // tăng tốc trên mặt đất
    [SerializeField] private float groundFriction = 110f;      // ma sát khi thả phím (mặt đất)
    [SerializeField] private float airAcceleration = 60f;      // tăng tốc trên không
    [SerializeField] private float airFriction = 25f;          // ma sát trên không (nhỏ hơn)

    [Header("Nhảy")]
    [SerializeField] private float jumpVelocity = 16f;         // vận tốc bật lên ban đầu
    [SerializeField] private int maxJumps = 2;                 // 2 = nhảy đơn + nhảy đúp
    [SerializeField] private float doubleJumpDelay = 0.12f;    // khoảng cách tối thiểu giữa 2 lần nhảy
    [SerializeField, Range(0.1f, 1f)] private float jumpCutMultiplier = 0.5f; // thả phím sớm -> nhảy thấp
    [SerializeField] private float coyoteTime = 0.1f;          // còn được nhảy ngay sau khi vừa rời mép
    [SerializeField] private float jumpBufferTime = 0.1f;      // nhớ phím nhảy bấm sớm trước khi chạm đất

    [Header("Trọng lực")]
    [SerializeField] private float riseGravityScale = 4f;
    [SerializeField] private float fallGravityScale = 6f;      // rơi nhanh hơn -> cảm giác nảy tự nhiên
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Kiểm tra mặt đất")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;

    [Header("Hiển thị (tuỳ chọn)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    // ----- Trạng thái công khai (IPlayerState) -----
    public PlayerState State { get; private set; } = PlayerState.Idle;
    public bool IsGrounded { get; private set; }
    public int FacingDirection { get; private set; } = 1;
    public Vector2 Velocity => rb.linearVelocity;

    private static readonly int StateHash = Animator.StringToHash("State");

    private Rigidbody2D rb;
    private Collider2D col;

    // input: đọc ở Update, dùng ở FixedUpdate
    private float moveInput;
    private bool jumpReleased;

    // bộ đếm thời gian
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float timeSinceJump = 999f;

    private int jumpsLeft;
    private bool isJumping;        // đang bay lên do lực nhảy (để cắt nhảy khi thả phím)
    private bool isDoubleJumping;  // đang ở pha nhảy đúp (để chọn animation)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;
        jumpsLeft = maxJumps;

        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("PlayerController: chưa gán Move Action / Jump Action trong Inspector.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (jumpAction != null) jumpAction.action.Disable();
    }

    // ---------------------------------------------------------------
    // Update: chỉ ĐỌC input và cập nhật hiển thị (animation, lật hình)
    // ---------------------------------------------------------------
    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>().x;

        if (jumpAction.action.WasPressedThisFrame()) jumpBufferCounter = jumpBufferTime;
        if (jumpAction.action.WasReleasedThisFrame()) jumpReleased = true;

        UpdateFacing();
        UpdateState();
        UpdateAnimator();
    }

    // ---------------------------------------------------------------
    // FixedUpdate: xử lý vật lý (di chuyển, nhảy, trọng lực)
    // Di chuyển ngang và nhảy là 2 luồng độc lập, xử lý song song mỗi bước.
    // ---------------------------------------------------------------
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        timeSinceJump += dt;

        CheckGround(dt);
        HandleHorizontal(dt);
        HandleJump();
        HandleJumpCut();
        ApplyGravity();

        jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - dt);
    }

    private void CheckGround(float dt)
    {
        Bounds b = col.bounds;
        Vector2 size = new Vector2(b.size.x * 0.9f, b.size.y);
        RaycastHit2D hit = Physics2D.BoxCast(b.center, size, 0f, Vector2.down, groundCheckDistance, groundLayer);

        // phải đang không bay lên, tránh tính là "chạm đất" ngay khung hình vừa nhảy
        IsGrounded = hit.collider != null && rb.linearVelocity.y <= 0.01f;

        if (IsGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpsLeft = maxJumps;
            isJumping = false;
            isDoubleJumping = false;
        }
        else
        {
            coyoteCounter = Mathf.Max(0f, coyoteCounter - dt);

            // Rơi khỏi mép mà chưa nhảy: mất lượt nhảy mặt đất, chỉ còn lượt nhảy trên không
            if (coyoteCounter <= 0f && jumpsLeft == maxJumps)
                jumpsLeft = maxJumps - 1;
        }
    }

    private void HandleHorizontal(float dt)
    {
        float targetSpeed = moveInput * maxSpeed;
        bool hasInput = Mathf.Abs(moveInput) > 0.01f;

        float rate;
        if (hasInput) rate = IsGrounded ? groundAcceleration : airAcceleration;
        else rate = IsGrounded ? groundFriction : airFriction;

        float vx = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * dt);
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (jumpBufferCounter <= 0f) return; // không có yêu cầu nhảy

        bool canGroundJump = coyoteCounter > 0f && jumpsLeft == maxJumps;
        // nhảy đúp: còn lượt VÀ đã cách lần nhảy trước đủ thời gian (không phải nhảy liên tục)
        bool canAirJump = !canGroundJump && jumpsLeft > 0 && timeSinceJump >= doubleJumpDelay;

        if (canGroundJump) PerformJump(false);
        else if (canAirJump) PerformJump(true);
    }

    private void PerformJump(bool isDouble)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity); // đặt lại vận tốc Y
        jumpsLeft--;
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        timeSinceJump = 0f;
        isJumping = true;
        isDoubleJumping = isDouble;
        IsGrounded = false;
    }

    // Giữ phím = nhảy cao, thả sớm = nhảy thấp
    private void HandleJumpCut()
    {
        if (!jumpReleased) return;
        jumpReleased = false;

        if (isJumping && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
    }

    private void ApplyGravity()
    {
        float vy = rb.linearVelocity.y;
        rb.gravityScale = vy < -0.01f ? fallGravityScale : riseGravityScale;

        if (vy < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
    }

    // ---------------------------------------------------------------
    // Trạng thái + animation
    // ---------------------------------------------------------------
    private void UpdateFacing()
    {
        if (moveInput > 0.01f) FacingDirection = 1;
        else if (moveInput < -0.01f) FacingDirection = -1;

        if (spriteRenderer != null) spriteRenderer.flipX = FacingDirection < 0;
    }

    private void UpdateState()
    {
        float vy = rb.linearVelocity.y;

        if (IsGrounded)
            State = Mathf.Abs(rb.linearVelocity.x) > 0.1f ? PlayerState.Running : PlayerState.Idle;
        else if (vy > 0.01f)
            State = isDoubleJumping ? PlayerState.DoubleJumping : PlayerState.Jumping;
        else
            State = PlayerState.Falling;
    }

    private void UpdateAnimator()
    {
        if (animator != null) animator.SetInteger(StateHash, (int)State);
    }

    // Vẽ hộp kiểm tra mặt đất (màu xanh) khi chọn Player trong Scene view
    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(b.center + Vector3.down * groundCheckDistance,
                            new Vector3(b.size.x * 0.9f, b.size.y, 0f));
    }
}