using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the race itself — countdown, start, finish.
/// BeginRace() is called by GameManager after character select.
/// </summary>
public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("All 4 racers — same order as GameManager: 0=Purple 1=Pink 2=Yellow 3=Green")]
    public Racer[] racers;

    [Header("Starting positions for each racer")]
    public Transform[] startPositions;

    [Header("Countdown UI")]
    public Text countdownText;

    [Header("Countdown duration")]
    public float countdownTime = 3f;

    private bool raceFinished = false;

    // ── Singleton ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Called by GameManager after player picks character ────────────────────

    /// <summary>
    /// playerIndex = which racer the human controls (0-3).
    /// All others become AI.
    /// </summary>
    public void BeginRace(int playerIndex)
    {
        raceFinished = false;

        // Reset positions
        for (int i = 0; i < racers.Length; i++)
        {
            if (startPositions != null && i < startPositions.Length)
                racers[i].transform.position = startPositions[i].position;

            racers[i].SetAsPlayer(i == playerIndex);
        }

        StartCoroutine(CountdownRoutine());
    }

    public void ResetRace()
    {
        StopAllCoroutines();
        raceFinished = false;

        foreach (var r in racers)
            r.StopRacing();
    }

    // ── Countdown ─────────────────────────────────────────────────────────────

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
            r.StartRacing();
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

    // ── Finish line calls this ────────────────────────────────────────────────

    public void OnRacerFinished(Racer winner)
    {
        if (raceFinished) return;
        raceFinished = true;

        // Stop all racers
        foreach (var r in racers)
            r.StopRacing();

        // Show results
        GameManager.Instance.ShowResults(winner.gameObject.name);
    }
}