using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Namespace removed - will wire up properly later
public class HUDManager : MonoBehaviour
{
    [Header("Countdown")]
    public Text countdownText;

    [Header("Win screen")]
    public GameObject winPanel;
    public Text winnerText;

    // Subscribe to GameEvents later when those are wired up
    private void OnEnable() { }
    private void OnDisable() { }
}