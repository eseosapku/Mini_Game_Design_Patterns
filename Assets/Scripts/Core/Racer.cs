using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Racer : MonoBehaviour
{
    [Header("Racing")]
    public float baseSpeed = 5f;
    public float acceleration = 0.05f;
    public float playerSpeed = 2.5f;

    [Header("AI Speed Variance")]
    public float speedVariance = 0.8f;
    public float varianceInterval = 1.5f;

    [Header("Jumping")]
    public float jumpForce = 14f;
    public float fallMultiplier = 4f; // higher = snappier fall
    public Sprite jumpSprite;
    public Sprite runSprite;

    [Header("Ground Layer")]
    public LayerMask groundLayer;

    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 2f;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public bool IsFrozen => isFrozen;

    private bool isPlayer = false;
    private bool raceStarted = false;
    private bool finished = false;
    private bool wasGrounded = false;
    private bool isGrounded = false;

    private float currentSpeed = 0f;
    private float varianceTimer = 0f;
    private float currentVariance = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private static readonly int RunningParam = Animator.StringToHash("Running");
    private static readonly int FrozenParam = Animator.StringToHash("Frozen");

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Ignore collisions between all racers so they pass through each other
        Racer[] allRacers = FindObjectsByType<Racer>(FindObjectsSortMode.None);
        foreach (Racer other in allRacers)
        {
            if (other == this) continue;
            Collider2D otherCol = other.GetComponent<Collider2D>();
            if (otherCol != null && col != null)
                Physics2D.IgnoreCollision(col, otherCol, true);
        }
    }

    private void FixedUpdate()
    {
        if (!raceStarted || finished || col == null) return;

        // Ground check
        isGrounded = Physics2D.OverlapBox(
            new Vector2(col.bounds.center.x, col.bounds.min.y - 0.05f),
            new Vector2(col.bounds.size.x * 0.8f, 0.1f),
            0f, groundLayer);

        // Snappy fall — apply extra gravity when falling
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                               * (fallMultiplier - 1f) * Time.fixedDeltaTime;
    }

    public void SetAsPlayer(bool playerControlled)
    {
        isPlayer = playerControlled;
        if (playerControlled)
            currentSpeed = playerSpeed;
        else
        {
            currentVariance = Random.Range(-speedVariance, speedVariance);
            currentSpeed = baseSpeed + currentVariance;
        }
        Debug.Log(gameObject.name + " isPlayer=" + playerControlled
                  + " startSpeed=" + currentSpeed);
    }

    public void StartRacing()
    {
        raceStarted = true;
        finished = false;
        anim.enabled = true;
        anim.SetBool(RunningParam, true);
    }

    public void StopRacing()
    {
        raceStarted = false;
        finished = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool(RunningParam, false);
    }

    private void Update()
    {
        if (!raceStarted || finished) return;

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f) Unfreeze();
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Sprite swap for jump
        if (!isGrounded && jumpSprite != null)
        {
            anim.enabled = false;
            sr.sprite = jumpSprite;
        }
        else if (isGrounded && !wasGrounded)
        {
            anim.enabled = true;
            anim.SetBool(RunningParam, true);
        }
        wasGrounded = isGrounded;

        if (isPlayer)
        {
            HandlePlayerInput();
            currentSpeed += (acceleration * 0.3f) * Time.deltaTime;
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            HandleAI();
            varianceTimer -= Time.deltaTime;
            if (varianceTimer <= 0f)
            {
                currentVariance = Random.Range(-speedVariance, speedVariance);
                varianceTimer = varianceInterval;
            }
            currentSpeed += acceleration * Time.deltaTime;
            rb.linearVelocity = new Vector2(
                currentSpeed + currentVariance, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        // Reset Y velocity before jumping so double-jump doesn't stack
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void HandlePlayerInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        bool jumpPressed = keyboard.wKey.wasPressedThisFrame
                        || keyboard.upArrowKey.wasPressedThisFrame;
        if (jumpPressed && isGrounded) Jump();
    }

    private void HandleAI()
    {
        if (!isGrounded || col == null) return;
        RaycastHit2D hit = Physics2D.Raycast(
            col.bounds.center, Vector2.right, 1.5f);
        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            Jump();
    }

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
        anim.enabled = true;
        anim.SetBool(FrozenParam, false);
    }

    protected virtual void Act() { }
    protected void MoveForward() { }
    protected void StopHorizontal() { }
    protected void TryJump() { if (isGrounded) Jump(); }
    protected Animator Anim => anim;
}