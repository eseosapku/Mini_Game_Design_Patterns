using UnityEngine;

namespace AstroRush.Enemy
{
    public class AIController : Racer
    {
        [Header("AI Settings")]
        [SerializeField] private float jumpCheckDistance = 2.5f;
        [SerializeField] private LayerMask obstacleLayer;

        protected override void Act()
        {
            // Obstacle jumping - shooting will be wired up later
            if (ObstacleAhead()) TryJump();
        }

        private bool ObstacleAhead()
        {
            Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.2f);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, jumpCheckDistance, obstacleLayer);
            return hit.collider != null;
        }

        public override void TakeHit()
        {
            base.TakeHit();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * jumpCheckDistance);
        }
    }
}