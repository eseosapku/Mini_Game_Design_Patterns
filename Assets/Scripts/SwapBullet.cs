using UnityEngine;

/// <summary>
/// The projectile fired by the player.
/// On hitting another Racer, swaps positions with them.
/// </summary>
public class SwapBullet : MonoBehaviour
{
    [Header("Speed")]
    public float speed = 12f;

    private Racer shooter;

    public void Init(Racer firedBy)
    {
        shooter = firedBy;
        // Destroy bullet after 4 seconds if it hits nothing
        Destroy(gameObject, 4f);
    }

    private void Update()
    {
        // Move right (forward along the track)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the shooter themselves
        if (other.gameObject == shooter.gameObject) return;

        Racer hit = other.GetComponent<Racer>();
        if (hit == null) return;

        // Swap positions
        Vector3 shooterPos = shooter.transform.position;
        Vector3 hitPos     = hit.transform.position;

        shooter.transform.position = hitPos;
        hit.transform.position     = shooterPos;

        Debug.Log(shooter.gameObject.name + " swapped with " + hit.gameObject.name);

        Destroy(gameObject);
    }
}
