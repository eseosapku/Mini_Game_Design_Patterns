using UnityEngine;

/// <summary>
/// Place on an empty GameObject at the end of the track.
/// Add a BoxCollider2D (set Is Trigger = true) to that GameObject.
/// Drag your RaceManager into the field below.
///
/// The first Racer to enter the trigger wins.
/// </summary>
public class FinishLine : MonoBehaviour
{
    [Tooltip("Drag your RaceManager GameObject here")]
    public RaceManager raceManager;

    private bool raceOver = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (raceOver) return;

        // Check if it's a Racer
        Racer racer = other.GetComponent<Racer>();
        if (racer == null) return;

        raceOver = true;
        raceManager.OnRacerFinished(racer);
    }
}