using System.Collections;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("All 4 racers — 0=Purple 1=Pink 2=Yellow 3=Green")]
    public Racer[] racers;

    [Header("Starting positions for each racer")]
    public Transform[] startPositions;

    [Header("Countdown UI")]
    public TMP_Text countdownText;

    [Header("Countdown duration")]
    public float countdownTime = 3f;

    private bool raceFinished = false;

    // Store initial positions set in editor
    private Vector3[] initialPositions;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Cache starting positions on first load
        initialPositions = new Vector3[racers.Length];
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] != null)
                initialPositions[i] = racers[i].transform.position;
        }
    }

    public void BeginRace(int playerIndex)
    {
        raceFinished = false;

        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] == null) continue;

            // Reset to start position
            if (startPositions != null && i < startPositions.Length && startPositions[i] != null)
                racers[i].transform.position = startPositions[i].position;
            else if (initialPositions != null && i < initialPositions.Length)
                racers[i].transform.position = initialPositions[i];

            // Reset rigidbody
            Rigidbody2D rb = racers[i].GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            racers[i].SetAsPlayer(i == playerIndex);

            // Set shooting system
            ShootingSystem ss = racers[i].GetComponent<ShootingSystem>();
            if (ss != null) ss.SetAsPlayer(i == playerIndex);
        }

        // Camera follows player
        if (CameraFollow.Instance != null)
            CameraFollow.Instance.SetTarget(racers[playerIndex].transform);

        StartCoroutine(CountdownRoutine());
    }

    public void ResetRace()
    {
        StopAllCoroutines();
        raceFinished = false;

        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] == null) continue;

            racers[i].StopRacing();

            // Move back to start
            if (startPositions != null && i < startPositions.Length && startPositions[i] != null)
                racers[i].transform.position = startPositions[i].position;
            else if (initialPositions != null && i < initialPositions.Length)
                racers[i].transform.position = initialPositions[i];

            // Stop rigidbody
            Rigidbody2D rb = racers[i].GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator CountdownRoutine()
    {
        for (int i = (int)countdownTime; i > 0; i--)
        {
            ShowText(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        ShowText("GO!");
        yield return new WaitForSeconds(0.6f);
        HideText();
        foreach (var r in racers)
            if (r != null) r.StartRacing();
    }

    private void ShowText(string msg)
    {
        if (countdownText == null) return;
        countdownText.text = msg;
        countdownText.enabled = true;
    }

    private void HideText()
    {
        if (countdownText == null) return;
        countdownText.enabled = false;
    }

    public void OnRacerFinished(Racer winner)
    {
        if (raceFinished) return;
        raceFinished = true;
        foreach (var r in racers)
            if (r != null) r.StopRacing();
        GameManager.Instance.ShowResults(winner.gameObject.name);
    }
}