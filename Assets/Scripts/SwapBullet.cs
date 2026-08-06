using UnityEngine;

/// <summary>
/// Fired by the player. On hitting another Racer, swaps positions.
/// Create a prefab with:
///   - SpriteRenderer (any small circle/star sprite, or leave default)
///   - CircleCollider2D with Is Trigger ON
///   - This script
/// </summary>
public class SwapBullet : MonoBehaviour
{
    public float speed = 15f;

    private Racer shooter;

    public void Init(Racer firedBy)
    {
        shooter = firedBy;
        Destroy(gameObject, 5f);

        // Make bullet visible with a yellow circle if no sprite assigned
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            sr.color = new Color(1f, 0.9f, 0f); // yellow
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shooter == null) return;
        if (other.gameObject == shooter.gameObject) return;

        // Ignore ground and other non-racer objects
        Racer hit = other.GetComponent<Racer>();
        if (hit == null) return;

        // Swap positions
        Vector3 shooterPos = shooter.transform.position;
        Vector3 hitPos = hit.transform.position;

        shooter.transform.position = hitPos;
        hit.transform.position = shooterPos;

        Debug.Log(shooter.gameObject.name + " swapped with " + hit.gameObject.name);
        Destroy(gameObject);
    }
}