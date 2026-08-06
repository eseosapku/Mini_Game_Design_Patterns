using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Racer : MonoBehaviour
{
    [Header("Racing")]
    public float baseSpeed = 4f;
    public float acceleration = 0.05f;

    [Header("AI Speed Variance - makes race feel competitive")]
    [Tooltip("How much the AI speed randomly varies")]
    public float speedVariance = 0.8f;
    [Tooltip("How often speed changes (seconds)")]
    public float varianceInterval = 1.5f;

    [Header("Jumping")]
    public float jumpForce = 10f;
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
    }

    private void FixedUpdate()
    {
        if (!raceStarted || finished) return;

        // Ground check using box overlap just below character
        if (col != null)
        {
            isGrounded = Physics2D.OverlapBox(
                new Vector2(col.bounds.center.x, col.bounds.min.y - 0.05f),
                new Vector2(col.bounds.size.x * 0.8f, 0.1f),
                0f,
                groundLayer);
        }
    }

    public void SetAsPlayer(bool playerControlled)
    {
        isPlayer = playerControlled;
        // Give AI a random starting variance so they don't all move identically
        if (!playerControlled)
            currentVariance = Random.Range(-speedVariance, speedVariance);
        Debug.Log(gameObject.name + " isPlayer = " + playerControlled);
    }

    public void StartRacing()
    {
        raceStarted = true;
        finished = false;
        currentSpeed = baseSpeed;
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

        // Accelerate over time
        currentSpeed += acceleration * Time.deltaTime;

        if (isPlayer)
        {
            HandlePlayerInput();
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            HandleAI();
            // Apply variance so AI racers jostle naturally
            varianceTimer -= Time.deltaTime;
            if (varianceTimer <= 0f)
            {
                currentVariance = Random.Range(-speedVariance, speedVariance);
                varianceTimer = varianceInterval;
            }
            rb.linearVelocity = new Vector2(
                currentSpeed + currentVariance, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log(gameObject.name + " JUMPED");
    }

    private void HandlePlayerInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool jumpPressed = keyboard.wKey.wasPressedThisFrame
                        || keyboard.upArrowKey.wasPressedThisFrame;

        if (jumpPressed)
        {
            Debug.Log("Jump pressed. isGrounded = " + isGrounded);
            if (isGrounded) Jump();
        }
    }

    private void HandleAI()
    {
        if (!isGrounded) return;

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