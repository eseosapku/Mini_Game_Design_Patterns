using UnityEngine;

/// <summary>
/// Collectible orb on the ground.
/// When the player runs into it, adds shots to their count.
/// 
/// Setup:
/// - Create a GameObject with a sprite (glowing orb)
/// - Add CircleCollider2D with Is Trigger ON
/// - Attach this script
/// - Optionally add a simple rotation in Update for visual flair
/// </summary>
public class ShotPickup : MonoBehaviour
{
    [Header("How many shots this pickup gives")]
    public int shotsToAdd = 1;

    [Header("Rotation speed for visual flair")]
    public float rotateSpeed = 90f;

    private void Update()
    {
        // Spin the orb so it looks like a collectible
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only the player can collect this
        Racer racer = other.GetComponent<Racer>();
        if (racer == null) return;

        // Tell the ShootingSystem to add shots
        ShootingSystem shooter = racer.GetComponent<ShootingSystem>();
        if (shooter == null) return;
        if (!shooter.IsPlayer) return;

        shooter.AddShots(shotsToAdd);
        Debug.Log("Picked up " + shotsToAdd + " shot(s)!");

        Destroy(gameObject);
    }
}
