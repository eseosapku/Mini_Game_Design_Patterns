using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to an empty GameObject called "PauseMenu".
/// Assign your pause panel in the Inspector.
/// Press Escape to toggle pause.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("Pause Panel - assign your pause UI panel here")]
    public GameObject pausePanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;  // stops everything
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;  // resumes everything
        pausePanel.SetActive(false);
    }

    public void RestartRace()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pausePanel.SetActive(false);
        // Go back to character select
        RaceManager.Instance.ResetRace();
        GameManager.Instance.ShowCharacterSelect();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        RaceManager.Instance.ResetRace();
        GameManager.Instance.ShowMainMenu();
        pausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}