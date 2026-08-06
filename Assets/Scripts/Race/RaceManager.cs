using System.Collections;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("All 4 racers — same order: 0=Purple 1=Pink 2=Yellow 3=Green")]
    public Racer[] racers;

    [Header("Starting positions for each racer")]
    public Transform[] startPositions;

    [Header("Countdown UI")]
    public TMP_Text countdownText;

    [Header("Countdown duration")]
    public float countdownTime = 3f;

    private bool raceFinished = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void BeginRace(int playerIndex)
    {
        raceFinished = false;

        for (int i = 0; i < racers.Length; i++)
        {
            if (startPositions != null && i < startPositions.Length)
                racers[i].transform.position = startPositions[i].position;

            // Set player vs AI on Racer
            racers[i].SetAsPlayer(i == playerIndex);

            // Also set on ShootingSystem if present
            ShootingSystem ss = racers[i].GetComponent<ShootingSystem>();
            if (ss != null) ss.SetAsPlayer(i == playerIndex);
        }

        // Tell camera to follow chosen player
        if (CameraFollow.Instance != null)
            CameraFollow.Instance.SetTarget(racers[playerIndex].transform);

        StartCoroutine(CountdownRoutine());
    }

    public void ResetRace()
    {
        StopAllCoroutines();
        raceFinished = false;
        foreach (var r in racers)
            r.StopRacing();
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

    public void OnRacerFinished(Racer winner)
    {
        if (raceFinished) return;
        raceFinished = true;
        foreach (var r in racers)
            r.StopRacing();
        GameManager.Instance.ShowResults(winner.gameObject.name);
    }
}