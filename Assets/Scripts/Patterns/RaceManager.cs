using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstroRush.Core;

namespace AstroRush.Patterns
{
    /// <summary>
    /// DESIGN PATTERN: SINGLETON.
    ///
    /// One RaceManager for the whole scene. Owns the countdown, tracks
    /// finishing order, and declares the winner. Every other script that
    /// needs race state reads RaceManager.Instance — no Inspector wiring,
    /// no null-checks for a missing reference.
    ///
    /// Scoped deliberately: only race-global state (is the race running?
    /// who has finished?) lives here. Character movement and animation live
    /// in their own classes.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [SerializeField] private float countdownSeconds = 3f;

        public bool RaceRunning  { get; private set; }
        public bool RaceFinished { get; private set; }

        // Finishing order — populated by Observer events
        private readonly List<Racer> finishOrder = new List<Racer>();
        public IReadOnlyList<Racer> FinishOrder => finishOrder;

        // ── Singleton enforcement ──────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()  => GameEvents.OnRacerFinished += HandleFinish;
        private void OnDisable() => GameEvents.OnRacerFinished -= HandleFinish;

        private void Start() => StartCoroutine(CountdownRoutine());

        // ── Countdown ──────────────────────────────────────────────────────────
        private IEnumerator CountdownRoutine()
        {
            for (int i = (int)countdownSeconds; i > 0; i--)
            {
                GameEvents.RaiseCountdownTick(i);
                yield return new WaitForSeconds(1f);
            }
            GameEvents.RaiseCountdownTick(0); // 0 = "GO"
            RaceRunning = true;
            GameEvents.RaiseRaceStart();
        }

        // ── Finish line handling ───────────────────────────────────────────────
        private void HandleFinish(Racer r)
        {
            if (finishOrder.Contains(r)) return;
            finishOrder.Add(r);

            if (!RaceFinished)
            {
                RaceFinished = true;
                RaceRunning  = false;
                GameEvents.RaiseRaceEnd();
            }
        }
    }
}
