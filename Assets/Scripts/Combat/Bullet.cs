using UnityEngine;
using AstroRush.Core;

namespace AstroRush.Combat
{
    /// <summary>
    /// A fired shot. Moves in the given direction; on hitting any Racer
    /// calls TakeHit() polymorphically — never checks Player vs. AI.
    ///
    /// OOP — POLYMORPHISM: racer.TakeHit() dispatches to the correct
    /// override (PlayerController or AIController) at runtime with no
    /// type switch needed here.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed    = 12f;
        [SerializeField] private float lifetime = 3f;

        private Vector2 direction;
        private Racer   owner;   // racer that fired — won't hit its own owner

        public void Init(Vector2 dir, Racer firedBy = null)
        {
            direction = dir.normalized;
            owner     = firedBy;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var racer = other.GetComponent<Racer>();
            if (racer == null || racer == owner) return;

            racer.TakeHit();    // polymorphic — correct override runs automatically
            Destroy(gameObject);
        }
    }
}
