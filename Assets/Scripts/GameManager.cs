using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectPanel;
    public GameObject resultsPanel;
    public GameObject settingsPanel;

    [Header("Results Panel UI")]
    public TMP_Text resultsText;

    [Header("Characters in scene — 0=Purple 1=Pink 2=Yellow 3=Green")]
    public GameObject[] characterObjects;

    private const float PLAYER_SPEED = 2.5f;
    private const float AI_SPEED = 5.0f;

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

    // ── Main menu buttons ─────────────────────────────────────────────────────

    // Wire to your Play button
    public void PlayGame()
    {
        ShowCharacterSelect();
    }

    // Wire to your Settings button
    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Wire to your back button inside settings
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        ShowMainMenu();
    }

    // Wire to your Quit button
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ── Panel navigation ──────────────────────────────────────────────────────

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowCharacterSelect()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(true);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowResults(string winnerName)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (resultsText != null)
            resultsText.text = winnerName + " Wins!";
    }

    // ── Character select ──────────────────────────────────────────────────────

    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;

        // Auto set speeds — chosen character is slower, AI are faster
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i] == null) continue;
            Racer r = characterObjects[i].GetComponent<Racer>();
            if (r == null) continue;

            if (i == index)
            {
                r.playerSpeed = PLAYER_SPEED;
                r.baseSpeed = PLAYER_SPEED;
            }
            else
            {
                r.baseSpeed = AI_SPEED;
            }
        }

        StartRace();
    }

    private void StartRace()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        RaceManager.Instance.BeginRace(SelectedCharacterIndex);
    }

    // ── Results buttons ───────────────────────────────────────────────────────

    public void PlayAgain()
    {
        RaceManager.Instance.ResetRace();
        ShowCharacterSelect();
    }

    public void ShowMainMenuFromResults()
    {
        RaceManager.Instance.ResetRace();
        ShowMainMenu();
    }
}