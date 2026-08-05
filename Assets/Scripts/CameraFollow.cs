using UnityEngine;

/// <summary>
/// Attach to the Main Camera.
/// Drag your selected player into the Target field,
/// OR call CameraFollow.Instance.SetTarget(racer) from code.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Target to follow")]
    public Transform target;

    [Header("How far behind and above the player")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("How smoothly the camera follows (lower = smoother)")]
    public float smoothSpeed = 0.1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;

        // Only follow on X axis — keep Y fixed so camera doesn't bob up/down
        desiredPos.y = transform.position.y;

        transform.position = Vector3.Lerp(
            transform.position, desiredPos, smoothSpeed);
    }

    /// <summary>
    /// Called by RaceManager when race starts so camera snaps to chosen player.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
