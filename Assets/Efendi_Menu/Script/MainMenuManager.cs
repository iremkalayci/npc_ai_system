using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;
        InitializePanels();
        SetupBackgroundVideo();
        SetupButtons();
        
        // Başlangıçta hiçbir şey seçili olmasın
        eventSystem.SetSelectedGameObject(null);
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    #region 1. Panel System
    [Header("Panels")]
    public GameObject MainPanel;
    public GameObject SettingPanel;

    private void InitializePanels()
    {
        MainPanel.SetActive(true);
        SettingPanel.SetActive(false);
    }

    private void OpenSettings()
    {
        MainPanel.SetActive(false);
        SettingPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(null);
    }

    private void CloseSettings()
    {
        SettingPanel.SetActive(false);
        MainPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(null);
    }
    #endregion

    #region 2. Background Video System
    public GameObject BackgroundPanel;

    [Header("Background Video")]
    public bool playBackgroundVideo = true;
    public GameObject clip;
    public VideoClip backgroundVideoClip;
    public bool videoMute = false;
    [Range(0f, 1f)] public float videoVolume = 1f;
    [Range(0.1f, 2f)] public float playbackSpeed = 1f;

    private VideoPlayer videoPlayer;

    private void SetupBackgroundVideo()
    {
        if (clip != null)
        {
            videoPlayer = clip.GetComponent<VideoPlayer>();
            if (videoPlayer != null && backgroundVideoClip != null)
                videoPlayer.clip = backgroundVideoClip;
        }

        UpdateBackgroundVideoState();
    }

    private void UpdateBackgroundVideoState()
    {
        if (playBackgroundVideo)
        {
            if (BackgroundPanel != null)
                BackgroundPanel.SetActive(false);

            if (clip != null)
            {
                clip.SetActive(true);
                if (videoPlayer != null)
                {
                    videoPlayer.SetDirectAudioMute(0, videoMute);
                    videoPlayer.SetDirectAudioVolume(0, videoVolume);
                    videoPlayer.playbackSpeed = playbackSpeed;
                    videoPlayer.Play();
                }
            }
        }
        else
        {
            if (BackgroundPanel != null)
                BackgroundPanel.SetActive(true);

            if (clip != null)
            {
                clip.SetActive(false);
                if (videoPlayer != null)
                    videoPlayer.Stop();
            }
        }
    }
    #endregion

    #region 3. Button System
    [Header("Buttons")]
    public Button playButton;
    public Button openSettingButton;
    public Button closeSettingButton;
    public Button quitButton;

    [Header("Scene")]
    public int gameSceneIndex;

    private void SetupButtons()
    {
        playButton.onClick.AddListener(PlayGame);
        openSettingButton.onClick.AddListener(OpenSettings);
        closeSettingButton.onClick.AddListener(CloseSettings);
        quitButton.onClick.AddListener(QuitGame);

        SetupToggleSystem();
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region 4. Settings Toggle System
    [Header("Toggles")]
    public Toggle musicToggle;
    public Toggle effectToggle;

    private void SetupToggleSystem()
    {
        LoadSettings();

        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        effectToggle.onValueChanged.AddListener(OnSFXToggleChanged);

        SetupAudioSystem();
    }

    private void LoadSettings()
    {
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        effectToggle.isOn = sfxEnabled;
        musicToggle.isOn = musicEnabled;
    }

    private void OnMusicToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("MusicEnabled", value ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSourcesMute();
    }

    private void OnSFXToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("SFXEnabled", value ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSourcesMute();
    }
    #endregion

    #region 5. Audio System
    [Header("Audio Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;

    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Audio Source")]
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    private void SetupAudioSystem()
    {
        UpdateAudioSourcesMute();
        AddClickSounds();
        AddHoverEvents();
    }

    private void UpdateAudioSourcesMute()
    {
        if (sfxAudioSource != null)
            sfxAudioSource.mute = !effectToggle.isOn;

        if (musicAudioSource != null)
            musicAudioSource.mute = !musicToggle.isOn;
    }

    private void AddClickSounds()
    {
        playButton.onClick.RemoveAllListeners();
        openSettingButton.onClick.RemoveAllListeners();
        closeSettingButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        playButton.onClick.AddListener(() => { PlayClickSound(); PlayGame(); });
        openSettingButton.onClick.AddListener(() => { PlayClickSound(); OpenSettings(); });
        closeSettingButton.onClick.AddListener(() => { PlayClickSound(); CloseSettings(); });
        quitButton.onClick.AddListener(() => { PlayClickSound(); QuitGame(); });

        musicToggle.onValueChanged.RemoveAllListeners();
        effectToggle.onValueChanged.RemoveAllListeners();

        musicToggle.onValueChanged.AddListener((bool value) => { PlayClickSound(); OnMusicToggleChanged(value); });
        effectToggle.onValueChanged.AddListener((bool value) => { PlayClickSound(); OnSFXToggleChanged(value); });
    }

    private void AddHoverEvents()
    {
        AddHoverEventToButton(playButton);
        AddHoverEventToButton(openSettingButton);
        AddHoverEventToButton(closeSettingButton);
        AddHoverEventToButton(quitButton);

        AddHoverEventToToggle(musicToggle);
        AddHoverEventToToggle(effectToggle);
    }

    private void AddHoverEventToButton(Button button)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    private void AddHoverEventToToggle(Toggle toggle)
    {
        if (toggle == null) return;

        EventTrigger trigger = toggle.gameObject.GetComponent<EventTrigger>() ?? toggle.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    private void PlayHoverSound()
    {
        if (playHoverSound && hoverSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(hoverSound);
    }

    private void PlayClickSound()
    {
        if (playClickSound && clickSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(clickSound);
    }
    #endregion

    #region 6. Keyboard Navigation
    private void HandleKeyboardInput()
    {
        // Enter: aktif objeyi tıklat
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            if (selected != null)
            {
                var button = selected.GetComponent<Button>();
                if (button != null)
                    button.onClick.Invoke();

                var toggle = selected.GetComponent<Toggle>();
                if (toggle != null)
                    toggle.isOn = !toggle.isOn;
            }
        }

        // ESC: geri dön
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingPanel.activeSelf)
                CloseSettings();
        }

        // Hiçbir şey seçili değilse manuel seç
        if (eventSystem.currentSelectedGameObject == null)
        {
            if (MainPanel.activeSelf)
                eventSystem.SetSelectedGameObject(playButton.gameObject);
            else if (SettingPanel.activeSelf)
                eventSystem.SetSelectedGameObject(musicToggle.gameObject);
        }
    }
    #endregion
}
