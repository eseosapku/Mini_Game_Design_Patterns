using UnityEngine;
using AstroRush.Core;
using AstroRush.Patterns;
using AstroRush.Combat;

namespace AstroRush.Player
{
    /// <summary>
    /// OOP — INHERITANCE + POLYMORPHISM:
    /// Inherits Racer's freeze system, ground check, and animator wiring.
    /// Overrides Act() to read keyboard input — the only thing that makes
    /// this racer different from an AI one.
    ///
    /// Because Bullet calls racer.TakeHit() polymorphically, it doesn't
    /// care whether this is a PlayerController or an AIController.
    /// </summary>
    public class PlayerController : Racer
    {
        [Header("Shooting")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float     fireCooldown = 0.5f;
        private float fireTimer;

        // ── Input bindings (serialised so they're tweakable per-player) ────────
        [Header("Key Bindings (supports 2-player split keyboard)")]
        [SerializeField] private KeyCode jumpKey  = KeyCode.W;
        [SerializeField] private KeyCode shootKey = KeyCode.Q;
        // Movement is always "run right" — no left key; racers only go forward

        protected override void Act()
        {
            // Race hasn't started yet (countdown) — stand still
            if (Patterns.RaceManager.Instance != null &&
                !Patterns.RaceManager.Instance.RaceRunning)
            {
                StopHorizontal();
                return;
            }

            MoveForward();

            if (Input.GetKeyDown(jumpKey))  TryJump();

            fireTimer -= Time.deltaTime;
            if (Input.GetKeyDown(shootKey) && fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireCooldown;
            }
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
            // Player-specific reaction — screen flash etc. can go here
        }
    }
}
