using UnityEngine;

public class AIController : Racer
{
    [Header("AI Settings")]
    [SerializeField] private float jumpCheckDistance = 2.5f;
    [SerializeField] private LayerMask obstacleLayer;

    protected override void Act()
    {
        if (ObstacleAhead()) TryJump();
    }

    private bool ObstacleAhead()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, jumpCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

    public override void TakeHit()
    {
        base.TakeHit();
    }
}