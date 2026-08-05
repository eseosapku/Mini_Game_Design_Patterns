using UnityEngine;
using AstroRush.Core;
using AstroRush.Patterns;
using AstroRush.Combat;

namespace AstroRush.Enemy
{
    /// <summary>
    /// OOP — INHERITANCE + POLYMORPHISM:
    /// Same Racer contract as PlayerController, different Act() implementation.
    /// Bullet hits this the same way it hits the player — no type checks anywhere.
    ///
    /// ALGORITHM 1 (STATE-BASED DECISION LOGIC) lives in Act() / DecideAction().
    /// </summary>
    public class AIController : Racer
    {
        [Header("AI Settings")]
        [SerializeField] private float jumpCheckDistance  = 2.5f;  // lookahead for obstacles
        [SerializeField] private float shootRange         = 6f;    // max firing distance
        [SerializeField] private float fireCooldown       = 1.2f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private LayerMask obstacleLayer;

        private float  fireTimer;
        private Racer  targetRacer;   // set by RacerTracker

        private void Start()
        {
            // Find the first racer that isn't this AI
            foreach (var r in FindObjectsOfType<Racer>())
            {
                if (r != this) { targetRacer = r; break; }
            }
        }

        // ── ALGORITHM 1: State-based AI decision tree ──────────────────────────
        /// <summary>
        /// Problem it solves: the AI needs to decide each frame what to do —
        /// run, jump over something, or shoot at the player — without reading
        /// any input device. It uses a priority-ordered state check:
        ///
        ///   1. If race hasn't started: wait.
        ///   2. If an obstacle is directly ahead within jumpCheckDistance: jump.
        ///   3. If the human player is ahead and in shoot range and not frozen: shoot.
        ///   4. Default: run forward.
        ///
        /// Choosing the highest-priority applicable action each frame (rather than
        /// tracking explicit states in a variable) keeps the logic readable and
        /// easy to extend — add a new priority by inserting an if-block.
        ///
        /// Complexity: O(1) per frame — a fixed number of raycasts (1) and
        /// distance comparisons regardless of scene size.
        /// </summary>
        protected override void Act()
        {
            if (Patterns.RaceManager.Instance != null &&
                !Patterns.RaceManager.Instance.RaceRunning)
            {
                StopHorizontal();
                return;
            }

            MoveForward();
            DecideAction();
        }

        private void DecideAction()
        {
            // Priority 1: jump over obstacle ahead
            if (ObstacleAhead())
            {
                TryJump();
                return;
            }

            // Priority 2: shoot at target if close enough and not frozen
            fireTimer -= Time.deltaTime;
            if (targetRacer != null && !targetRacer.IsFrozen && fireTimer <= 0f)
            {
                float dist = targetRacer.transform.position.x - transform.position.x;
                if (dist > 0f && dist <= shootRange)
                {
                    Shoot();
                    fireTimer = fireCooldown;
                }
            }
            // Priority 3 (implicit): MoveForward() already called above
        }

        private bool ObstacleAhead()
        {
            // Cast a short ray forward at knee height; detect obstacles
            Vector2 origin = new Vector2(
                transform.position.x,
                transform.position.y - 0.2f);    // slightly below centre
            RaycastHit2D hit = Physics2D.Raycast(
                origin, Vector2.right, jumpCheckDistance, obstacleLayer);
            return hit.collider != null;
        }

        private void Shoot()
        {
            Anim.SetTrigger(AnimHash.Shoot);
            SpawnerFactory.Instance.Create(
                SpawnType.Bullet, firePoint, Vector2.right);
        }

        public override void TakeHit()
        {
            base.TakeHit();
            // AI-specific reaction (e.g. re-evaluate target) could go here
        }

        // Draw the lookahead ray in the editor for debugging
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position,
                transform.position + Vector3.right * jumpCheckDistance);
        }
    }
}
