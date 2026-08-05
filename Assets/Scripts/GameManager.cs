using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectPanel;
    public GameObject resultsPanel;

    [Header("Results")]
    public TMP_Text resultsText;

    [Header("Characters in scene — same order: 0=Purple 1=Pink 2=Yellow 3=Green")]
    public GameObject[] characterObjects;

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

    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;
        StartRace();
    }

    private void StartRace()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        resultsPanel.SetActive(false);
        RaceManager.Instance.BeginRace(SelectedCharacterIndex);
    }

    public void PlayAgain()
    {
        RaceManager.Instance.ResetRace();
        ShowCharacterSelect();
    }

    public void QuitGame()
    {
        Application.Quit();
        // Also stops play mode in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}