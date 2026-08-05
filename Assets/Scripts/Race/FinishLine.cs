using UnityEngine;

/// <summary>
/// Place at the end of the track with a BoxCollider2D (Is Trigger ON).
/// First racer to enter wins.
/// </summary>
public class FinishLine : MonoBehaviour
{
    private bool raceOver = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (raceOver) return;

        Racer racer = other.GetComponent<Racer>();
        if (racer == null) return;

        raceOver = true;
        RaceManager.Instance.OnRacerFinished(racer);
    }

    // Reset for play again
    public void Reset() => raceOver = false;
}