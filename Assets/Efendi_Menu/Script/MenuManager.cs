using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject InGamePanel;
    public GameObject PausePanel;

    [Header("Buttons")]
    public Button PauseBtn;
    public Button ResumeBtn;
    public Button MenuBtn;

    [Header("Scene Settings")]
    public int menuSceneIndex = 0; // Ana menü sahnesinin Build Index numarası

    [Header("Audio Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;

    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Audio Sources")]
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    private bool sfxEnabled;
    private bool musicEnabled;

    private int selectedButtonIndex = 0;
    private Button[] pauseButtons;

    private void Start()
    {
        InitializePanels();
        SetupButtons();
    }

    private void Update()
    {
        HandlePauseInput();
        HandleMenuNavigation();
    }

    #region PANEL SYSTEM
    private void InitializePanels()
    {
        InGamePanel.SetActive(true);
        PausePanel.SetActive(false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        InGamePanel.SetActive(false);
        PausePanel.SetActive(true);

        pauseButtons = new Button[] { ResumeBtn, MenuBtn };

        EventSystem.current.SetSelectedGameObject(pauseButtons[0].gameObject);
        selectedButtonIndex = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        InGamePanel.SetActive(true);
        PausePanel.SetActive(false);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneIndex); // Eski scriptteki davranış
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PausePanel.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }
    #endregion

    #region MENU NAVIGATION
    private void HandleMenuNavigation()
    {
        if (!PausePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % pauseButtons.Length;
            EventSystem.current.SetSelectedGameObject(pauseButtons[selectedButtonIndex].gameObject);
            PlayHoverSound();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedButtonIndex = (selectedButtonIndex - 1 + pauseButtons.Length) % pauseButtons.Length;
            EventSystem.current.SetSelectedGameObject(pauseButtons[selectedButtonIndex].gameObject);
            PlayHoverSound();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // Enter tuşuna basıldığında o butonun click event’ini çağır
            pauseButtons[selectedButtonIndex].onClick.Invoke();
            PlayClickSound();
        }
    }
    #endregion

    #region BUTTON & AUDIO SETUP
    private void SetupButtons()
    {
        PauseBtn.onClick.AddListener(PauseGame);
        ResumeBtn.onClick.AddListener(ResumeGame);
        MenuBtn.onClick.AddListener(GoToMenu); // Ana menüye gider

        SetupAudioSystem();
    }

    private void SetupAudioSystem()
    {
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (musicAudioSource != null)
            musicAudioSource.mute = !musicEnabled;
    }

    private void PlayHoverSound()
    {
        if (playHoverSound && sfxEnabled && hoverSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(hoverSound);
    }

    private void PlayClickSound()
    {
        if (playClickSound && sfxEnabled && clickSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(clickSound);
    }
    #endregion
}
