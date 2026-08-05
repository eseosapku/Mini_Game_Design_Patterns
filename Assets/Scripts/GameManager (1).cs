using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central controller. Manages which panel is visible and
/// which character the player has selected.
///
/// Attach to an empty GameObject called "GameManager".
/// Wire up all the panel and button references in the Inspector.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectPanel;
    public GameObject resultsPanel;

    [Header("Results")]
    public Text resultsText;

    [Header("Characters in scene — same order: 0=Purple 1=Pink 2=Yellow 3=Green")]
    public GameObject[] characterObjects;

    // Which character index the player chose (0-3)
    public int SelectedCharacterIndex { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        ShowMainMenu();
    }

    // ── Panel navigation ──────────────────────────────────────────────────────

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        resultsPanel.SetActive(false);
    }

    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
        resultsPanel.SetActive(false);
    }

    public void ShowResults(string winnerName)
    {
        resultsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(false);

        if (resultsText != null)
            resultsText.text = winnerName + " Wins!";
    }

    // ── Called by character select buttons ───────────────────────────────────

    /// <summary>
    /// Call this from each character button's OnClick.
    /// Pass 0=Purple, 1=Pink, 2=Yellow, 3=Green
    /// </summary>
    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;
        StartRace();
    }

    // ── Race flow ─────────────────────────────────────────────────────────────

    private void StartRace()
    {
        // Hide all panels so the race is visible
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        resultsPanel.SetActive(false);

        // Tell RaceManager to begin
        RaceManager.Instance.BeginRace(SelectedCharacterIndex);
    }

    public void PlayAgain()
    {
        // Reset the race then go back to character select
        RaceManager.Instance.ResetRace();
        ShowCharacterSelect();
    }
}
