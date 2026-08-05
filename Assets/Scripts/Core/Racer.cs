using UnityEngine;

namespace AstroRush.Core
{
    /// <summary>
    /// Abstract base class for every racer in Astro Rush.
    ///
    /// OOP — ABSTRACTION + ENCAPSULATION:
    ///   This class defines *what* a racer can do (Run, Jump, Shoot, take a hit)
    ///   without committing to *how*. PlayerController fills in human input;
    ///   AIController fills in decision logic. Neither implementation leaks into
    ///   the other — the abstraction boundary is this class.
    ///
    ///   isFrozen and the freeze timer are private: nothing outside can corrupt
    ///   the freeze state by writing to it directly. Other code calls Freeze()
    ///   and the class handles its own bookkeeping.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public abstract class Racer : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float runSpeed   = 5f;
        [SerializeField] protected float jumpForce  = 10f;

        [Header("Freeze")]
        [SerializeField] private float freezeDuration = 2f;

        // ── Encapsulation ──────────────────────────────────────────────────────
        private bool  isFrozen    = false;
        private float freezeTimer = 0f;

        // Read-only access for other systems that need to query state
        public bool IsFrozen   => isFrozen;
        public bool IsGrounded { get; protected set; }

        // Component refs cached in Awake — subclasses access via protected props
        protected Rigidbody2D Rb        { get; private set; }
        protected Animator    Anim      { get; private set; }

        // ── Unity lifecycle ────────────────────────────────────────────────────
        protected virtual void Awake()
        {
            Rb   = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            if (isFrozen)
            {
                freezeTimer -= Time.deltaTime;
                if (freezeTimer <= 0f) Unfreeze();
                return;          // frozen racers neither move nor act
            }

            Act();               // polymorphic: player reads input, AI reads logic
        }

        // ── Abstraction: public contract ───────────────────────────────────────
        /// <summary>
        /// Called every frame when not frozen.
        /// OOP — POLYMORPHISM: PlayerController overrides this to read keyboard;
        /// AIController overrides it to run pathfinding / decision logic.
        /// Bullet only knows "this is a Racer" and calls TakeHit() — it never
        /// branches on whether it hit a player or an AI.
        /// </summary>
        protected abstract void Act();

        public virtual void TakeHit()
        {
            Freeze();
            GameEvents.RaiseRacerHit(this);
        }

        // ── Shared movement helpers (called by subclasses) ─────────────────────
        protected void MoveForward()
        {
            Rb.linearVelocity = new Vector2(runSpeed, Rb.linearVelocity.y);
            Anim.SetBool(AnimHash.Running, true);
        }

        protected void StopHorizontal()
        {
            Rb.linearVelocity = new Vector2(0f, Rb.linearVelocity.y);
            Anim.SetBool(AnimHash.Running, false);
        }

        protected void TryJump()
        {
            if (!IsGrounded) return;
            Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Anim.SetTrigger(AnimHash.Jump);
        }

        // ── Freeze / unfreeze ──────────────────────────────────────────────────
        private void Freeze()
        {
            isFrozen    = true;
            freezeTimer = freezeDuration;
            Rb.linearVelocity = Vector2.zero;
            Anim.SetBool(AnimHash.Frozen, true);
        }

        private void Unfreeze()
        {
            isFrozen = false;
            Anim.SetBool(AnimHash.Frozen, false);
        }

        // ── Ground detection ───────────────────────────────────────────────────
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;
        private const float GroundCheckRadius = 0.1f;

        protected virtual void FixedUpdate()
        {
            IsGrounded = Physics2D.OverlapCircle(
                groundCheck.position, GroundCheckRadius, groundLayer);
            Anim.SetBool(AnimHash.Grounded, IsGrounded);
        }
    }

    /// <summary>
    /// Central store for Animator parameter hashes.
    /// Using hashes instead of strings avoids per-frame string allocations
    /// and removes the risk of typos silently breaking animation triggers.
    /// </summary>
    public static class AnimHash
    {
        public static readonly int Running = Animator.StringToHash("Running");
        public static readonly int Jump    = Animator.StringToHash("Jump");
        public static readonly int Grounded= Animator.StringToHash("Grounded");
        public static readonly int Frozen  = Animator.StringToHash("Frozen");
        public static readonly int Shoot   = Animator.StringToHash("Shoot");
    }
}
