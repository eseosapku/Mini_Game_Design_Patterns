using UnityEngine;

namespace AstroRush.Player
{
    public class PlayerController : Racer
    {
        [Header("Key Bindings")]
        [SerializeField] private KeyCode jumpKey = KeyCode.W;
        [SerializeField] private KeyCode shootKey = KeyCode.Q;

        protected override void Act()
        {
            // Shooting and jumping will be wired up later
            if (Input.GetKeyDown(jumpKey)) TryJump();
        }

        public override void TakeHit()
        {
            base.TakeHit();
        }
    }
}