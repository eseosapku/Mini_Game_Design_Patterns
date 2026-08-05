using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Place on any empty GameObject in the scene (e.g. "RaceManager").
/// Drag all four Racer GameObjects into the Racers list in the Inspector.
/// Optionally assign a UI Text for the countdown display.
/// </summary>
public class RaceManager : MonoBehaviour
{
    [Header("Racers — drag all 4 characters here")]
    public Racer[] racers;

    [Header("Countdown")]
    public float countdownTime = 3f;

    [Header("UI (optional)")]
    [Tooltip("Assign a UI Text object to show 3-2-1-GO!")]
    public Text countdownText;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        // Count down 3 - 2 - 1
        for (int i = (int)countdownTime; i > 0; i--)
        {
            ShowText(i.ToString());
            yield return new WaitForSeconds(1f);
        }

        ShowText("GO!");
        yield return new WaitForSeconds(0.6f);
        HideText();

        // Tell every racer to start
        foreach (var r in racers)
        {
            if (r != null) r.StartRacing();
        }
    }

    private void ShowText(string msg)
    {
        if (countdownText == null) return;
        countdownText.text    = msg;
        countdownText.enabled = true;
    }

    private void HideText()
    {
        if (countdownText == null) return;
        countdownText.enabled = false;
    }

    /// <summary>
    /// Called by FinishLine when the first racer crosses.
    /// You can expand this to show a win screen etc.
    /// </summary>
    public void OnRacerFinished(Racer winner)
    {
        Debug.Log($"{winner.gameObject.name} wins the race!");

        // Stop everyone
        foreach (var r in racers)
        {
            if (r != null) r.StopRacing();
        }

        // Update UI if you have a win text — hook it up here later
        ShowText($"{winner.gameObject.name} Wins!");
    }
}
