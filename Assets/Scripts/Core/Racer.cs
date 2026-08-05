using UnityEngine;

/// <summary>
/// Attach to each of the 4 characters.
/// SetAsPlayer(true)  = human controls this one (arrow keys OR WASD)
/// SetAsPlayer(false) = AI controls this one
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Racer : MonoBehaviour
{
    [Header("Racing")]
    public float speed = 3f;
    public float acceleration = 0.1f;

    [Header("Jumping")]
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 2f;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public bool IsFrozen => isFrozen;

    // Internal state
    private bool isPlayer = false;
    private bool raceStarted = false;
    private bool finished = false;

    private Animator anim;
    private Rigidbody2D rb;

    // Animator parameter hashes
    private static readonly int RunningParam = Animator.StringToHash("Running");
    private static readonly int FrozenParam = Animator.StringToHash("Frozen");

    // ─────────────────────────────────────────────────────────────────────────
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Called by RaceManager — true = player controls this, false = AI
    /// </summary>
    public void SetAsPlayer(bool playerControlled)
    {
        isPlayer = playerControlled;
    }

    public void StartRacing()
    {
        raceStarted = true;
        finished = false;
        anim.SetBool(RunningParam, true);
    }

    public void StopRacing()
    {
        raceStarted = false;
        finished = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool(RunningParam, false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        if (!raceStarted || finished) return;

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f) Unfreeze();
            return;
        }

        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isPlayer)
            HandlePlayerInput();
        else
            HandleAI();

        // Move forward and accelerate
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        speed += acceleration * Time.deltaTime;
    }

    // ── Player input — arrow keys OR WASD both work ───────────────────────────
    private void HandlePlayerInput()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.W)
                        || Input.GetKeyDown(KeyCode.UpArrow);

        if (jumpPressed && isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // ── Simple AI — just runs, jumps if it sees an obstacle ──────────────────
    private void HandleAI()
    {
        // Raycast ahead for obstacles
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.1f),
            Vector2.right, 1.5f);

        if (hit.collider != null && hit.collider.CompareTag("Obstacle") && isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // ── Freeze ────────────────────────────────────────────────────────────────
    public void Freeze()
    {
        isFrozen = true;
        freezeTimer = freezeDuration;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool(FrozenParam, true);
    }

    public virtual void TakeHit() => Freeze();

    private void Unfreeze()
    {
        isFrozen = false;
        anim.SetBool(FrozenParam, false);
    }

    // ── Keep these so AIController/PlayerController still compile ─────────────
    protected virtual void Act() { }
    protected void MoveForward() { }
    protected void StopHorizontal() { }
    protected void TryJump()
    {
        if (isGrounded) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    protected Animator Anim => anim;
    public bool IsGrounded => isGrounded;
}