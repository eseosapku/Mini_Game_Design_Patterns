using UnityEngine;

/// <summary>
/// Attach this to each character (purple, pink, yellow, green).
/// Set a different Speed value on each one in the Inspector.
///
/// Requirements:
///   - Animator component on the same GameObject
///   - An Animation clip assigned in the Animator for "Running" state
///   - The Animator must have a Bool parameter called "Running"
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

    // ── Internal state ─────────────────────────────────────────────────────
    private Animator anim;
    private bool raceStarted = false;
    private bool finished = false;

    // ── Animator parameter name — must match exactly what you set in Unity ──
    private static readonly int RunningParam = Animator.StringToHash("Running");

    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Called by RaceManager when the countdown finishes.
    /// </summary>
    public void StartRacing()
    {
        raceStarted = true;
        anim.SetBool(RunningParam, true);
    }

    /// <summary>
    /// Called by FinishLine when this racer crosses.
    /// </summary>
    public void StopRacing()
    {
        finished = true;
        raceStarted = false;
        anim.SetBool(RunningParam, false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        if (!raceStarted || finished) return;

        // Move right
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Speed gradually increases over time
        speed += acceleration * Time.deltaTime;
    }
}