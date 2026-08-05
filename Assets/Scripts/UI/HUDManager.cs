using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Countdown")]
    public TMP_Text countdownText;

    [Header("Win screen")]
    public GameObject winPanel;
    public TMP_Text winnerText;
}