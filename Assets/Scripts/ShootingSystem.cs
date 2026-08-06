using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Attach to each Racer character.
/// Handles firing swap bullets and tracking shot count.
/// Only the player character can actually fire.
/// </summary>
public class ShootingSystem : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform  firePoint;
    public int        startingShots = 5;

    [Header("HUD - assign the shot count text here")]
    public TMP_Text shotCountText;

    private int  shotsLeft;
    private bool isPlayer = false;
    private Racer racer;

    public bool IsPlayer => isPlayer;

    private void Awake()
    {
        racer      = GetComponent<Racer>();
        shotsLeft  = startingShots;
    }

    private void Start()
    {
        UpdateHUD();
    }

    /// <summary>
    /// Called by RaceManager same time as Racer.SetAsPlayer()
    /// </summary>
    public void SetAsPlayer(bool value)
    {
        isPlayer = value;
        UpdateHUD();
    }

    private void Update()
    {
        if (!isPlayer) return;
        if (shotsLeft <= 0) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
            Fire();
    }

    private void Fire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("No bullet prefab assigned on " + gameObject.name);
            return;
        }

        // Spawn bullet at fire point (or character position if no fire point set)
        Vector3 spawnPos = firePoint != null
            ? firePoint.position
            : transform.position + Vector3.right;

        GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        SwapBullet sb = b.GetComponent<SwapBullet>();
        if (sb != null) sb.Init(racer);

        shotsLeft--;
        UpdateHUD();
        Debug.Log("Fired! Shots left: " + shotsLeft);
    }

    public void AddShots(int amount)
    {
        shotsLeft += amount;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (shotCountText == null) return;
        shotCountText.text = "Shots: " + shotsLeft;
    }
}
