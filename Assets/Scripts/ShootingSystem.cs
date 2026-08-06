using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShootingSystem : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public int startingShots = 5;

    [Header("HUD")]
    public TMP_Text shotCountText;

    private int shotsLeft;
    private bool isPlayer = false;
    private Racer racer;

    public bool IsPlayer => isPlayer;

    private void Awake()
    {
        racer = GetComponent<Racer>();
        shotsLeft = startingShots;
    }

    private void Start() => UpdateHUD();

    public void SetAsPlayer(bool value)
    {
        isPlayer = value;
        shotsLeft = startingShots; // reset shots each race
        UpdateHUD();
    }

    private void Update()
    {
        if (!isPlayer || shotsLeft <= 0) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame) Fire();
    }

    private void Fire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("No bullet prefab on " + gameObject.name);
            return;
        }

        // Always spawn at character's current world position
        // Offset slightly to the right so it doesn't immediately hit self
        Vector3 spawnPos = transform.position + new Vector3(0.8f, 0f, 0f);

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