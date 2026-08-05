using UnityEngine;

/// <summary>
/// Base racer class. Handles movement, animation, and freeze state.
/// PlayerController and AIController override Act() and TakeHit().
/// </summary>
[RequireComponent(typeof(Animator))]
public class Racer : MonoBehaviour
{
    [Header("Racing")]
    [Tooltip("Set a different value per character to vary their speeds")]
    public float speed = 3f;

    [Header("Speed scaling")]
    [Tooltip("How much speed increases per second as the race goes on")]
    public float acceleration = 0.1f;

    // Freeze state
    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 2f;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public bool IsFrozen => isFrozen;

    // Ground state (used by AIController / PlayerController for jumping later)
    public bool IsGrounded { get; protected set; }

    protected Animator Anim { get; private set; }

    private bool raceStarted = false;
    private bool finished = false;

    private static readonly int RunningParam = Animator.StringToHash("Running");
    private static readonly int FrozenParam = Animator.StringToHash("Frozen");

    // -------------------------------------------------------------------------
    protected virtual void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    public void StartRacing()
    {
        raceStarted = true;
        Anim.SetBool(RunningParam, true);
    }

    public void StopRacing()
    {
        finished = true;
        raceStarted = false;
        Anim.SetBool(RunningParam, false);
    }

    // -------------------------------------------------------------------------
    protected virtual void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f) Unfreeze();
            return;
        }

        if (!raceStarted || finished) return;

        // Subclasses can override Act() to add input / AI on top of movement
        Act();

        transform.Translate(Vector2.right * speed * Time.deltaTime);
        speed += acceleration * Time.deltaTime;
    }

    /// <summary>
    /// Override in PlayerController / AIController to add input or AI logic.
    /// Base version does nothing extra — movement is handled in Update above.
    /// </summary>
    protected virtual void Act() { }

    /// <summary>
    /// Override in subclasses to add extra hit reactions.
    /// Base version freezes the racer.
    /// </summary>
    public virtual void TakeHit()
    {
        Freeze();
    }

    // Freeze helpers
    public void Freeze()
    {
        isFrozen = true;
        freezeTimer = freezeDuration;
        Anim.SetBool(FrozenParam, true);
    }

    private void Unfreeze()
    {
        isFrozen = false;
        Anim.SetBool(FrozenParam, false);
    }

    // Movement helpers for subclasses to call later
    protected void MoveForward()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    protected void TryJump()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null && IsGrounded)
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
    }
}