using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Racer : MonoBehaviour
{
    [Header("Racing")]
    public float speed = 3f;
    public float acceleration = 0.1f;

    [Header("Jumping")]
    public float jumpForce = 8f;
    [Tooltip("Drag the jump sprite for this character here")]
    public Sprite jumpSprite;
    [Tooltip("Drag the first run sprite for this character here")]
    public Sprite runSprite;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool wasGrounded;

    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 2f;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public bool IsFrozen => isFrozen;

    private bool isPlayer = false;
    private bool raceStarted = false;
    private bool finished = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private static readonly int RunningParam = Animator.StringToHash("Running");
    private static readonly int FrozenParam = Animator.StringToHash("Frozen");

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetAsPlayer(bool playerControlled)
    {
        isPlayer = playerControlled;
        Debug.Log(gameObject.name + " isPlayer = " + playerControlled);
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

        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, 0.1f, groundLayer);

        // Swap sprite based on grounded state
        if (!isGrounded && jumpSprite != null)
        {
            // In the air — show jump sprite, pause run animation
            anim.enabled = false;
            sr.sprite = jumpSprite;
        }
        else if (isGrounded && !wasGrounded)
        {
            // Just landed — restore run animation
            anim.enabled = true;
            anim.SetBool(RunningParam, true);
        }

        wasGrounded = isGrounded;

        if (isPlayer)
            HandlePlayerInput();
        else
            HandleAI();

        // All racers move forward
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        speed += acceleration * Time.deltaTime;
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void HandlePlayerInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool jumpPressed = keyboard.wKey.wasPressedThisFrame
                        || keyboard.upArrowKey.wasPressedThisFrame;

        if (jumpPressed && isGrounded)
            Jump();
    }

    private void HandleAI()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.1f),
            Vector2.right, 1.5f);

        if (hit.collider != null &&
            hit.collider.CompareTag("Obstacle") &&
            isGrounded)
        {
            Jump();
        }
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
    public bool IsGrounded => isGrounded;
}