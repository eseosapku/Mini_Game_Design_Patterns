using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AstroRush.Core;
using AstroRush.Race;

namespace AstroRush.UI
{
    /// <summary>
    /// DESIGN PATTERN: OBSERVER (subscriber side).
    ///
    /// Listens to GameEvents and updates the HUD — countdown, position labels,
    /// win screen. Has zero direct references to PlayerController or AIController.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Countdown")]
        [SerializeField] private Text countdownText;

        [Header("Position labels (one per racer, same order as racers list)")]
        [SerializeField] private Text[] positionLabels;

        [Header("Win screen")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Text       winnerText;

        [Header("Racers to track")]
        [SerializeField] private List<Racer> racers = new List<Racer>();

        private bool raceActive;

        // ── Subscribe / unsubscribe cleanly ───────────────────────────────────
        private void OnEnable()
        {
            GameEvents.OnCountdownTick  += HandleCountdown;
            GameEvents.OnRaceStart      += HandleRaceStart;
            GameEvents.OnRacerFinished  += HandleRacerFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnCountdownTick  -= HandleCountdown;
            GameEvents.OnRaceStart      -= HandleRaceStart;
            GameEvents.OnRacerFinished  -= HandleRacerFinished;
        }

        // ── Observer callbacks ─────────────────────────────────────────────────
        private void HandleCountdown(int n)
        {
            countdownText.text    = n == 0 ? "GO!" : n.ToString();
            countdownText.enabled = true;
            StopAllCoroutines();
            StartCoroutine(HideCountdownAfter(n == 0 ? 0.8f : 0.9f));
        }

        private IEnumerator HideCountdownAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            countdownText.enabled = false;
        }

        private void HandleRaceStart()
        {
            raceActive = true;
        }

        private void HandleRacerFinished(Racer winner)
        {
            raceActive = false;
            winPanel.SetActive(true);
            winnerText.text = $"{winner.gameObject.name} wins!";
        }

        // ── Live position update (uses leaderboard sorting algorithm) ──────────
        private void Update()
        {
            if (!raceActive || racers.Count == 0) return;

            // ALGORITHM 2 in use: sort the small racer list by X position each frame
            RaceLeaderboard.SortByProgress(racers);

            string[] medals = { "1st", "2nd", "3rd", "4th" };
            for (int i = 0; i < positionLabels.Length && i < racers.Count; i++)
            {
                positionLabels[i].text = $"{medals[i]}";
            }
        }
    }
}
