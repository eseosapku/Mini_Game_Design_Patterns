using System.Collections.Generic;
using UnityEngine;
using AstroRush.Core;

namespace AstroRush.Race
{
    /// <summary>
    /// Placed at the end of the track. Any Racer that enters its trigger
    /// is recorded as finished via GameEvents (Observer).
    /// </summary>
    public class FinishLine : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var racer = other.GetComponent<Racer>();
            if (racer != null) GameEvents.RaiseRacerFinished(racer);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    /// <summary>
    /// ALGORITHM 2: SORTING — race leaderboard (insertion sort).
    ///
    /// Problem: after the race ends (or mid-race for a live position display)
    /// we need to rank racers by how far along the track they are, so we can
    /// show "1st / 2nd" HUD labels and declare a winner.
    ///
    /// Why insertion sort: the list is tiny (2–4 racers) and is updated every
    /// frame, meaning it's almost always already sorted except for at most one
    /// position swap. Insertion sort is O(n) on a nearly-sorted list — the
    /// inner loop almost never runs more than one iteration — versus O(n log n)
    /// for a general sort that doesn't exploit the near-sorted property.
    ///
    /// For larger lists an introsort (List.Sort) would be better; for this
    /// specific tiny-n, nearly-sorted, frequently-updated case, insertion sort
    /// is the right tool.
    ///
    /// Complexity: O(n) per update in the average/best case (nearly sorted),
    ///             O(n²) worst case (fully reversed order — never happens here).
    /// </summary>
    public static class RaceLeaderboard
    {
        /// <summary>
        /// Returns racers ordered by descending X position (furthest ahead first).
        /// Sorts in-place so no allocation occurs on the hot path.
        /// </summary>
        public static void SortByProgress(List<Racer> racers)
        {
            // Insertion sort descending by transform.position.x
            for (int i = 1; i < racers.Count; i++)
            {
                Racer key     = racers[i];
                float keyX    = key.transform.position.x;
                int   j       = i - 1;

                // Shift racers that are behind `key` (smaller x) one slot right
                while (j >= 0 && racers[j].transform.position.x < keyX)
                {
                    racers[j + 1] = racers[j];
                    j--;
                }
                racers[j + 1] = key;
            }
        }
    }
}
