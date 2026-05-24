using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PausePanel : MonoBehaviour
{
    private const string KeyMasterVolume = "volume_master";
    private const string KeyMusicVolume = "volume_music";
    private const string KeySfxVolume = "volume_sfx";
    private const string KeyFullScreen = "fullscreen";

    [Header("Roots")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private CanvasGroup contentGroup;

    [Header("Overlay")]
    [SerializeField] private float overlayTargetAlpha = 0.55f;

    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display")]
    [SerializeField] private Button fullscreenOffButton;
    [SerializeField] private Button fullscreenOnButton;

    [Header("Navigation")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button newGameButton;

    [Header("Scenes")]
    [SerializeField] private string welcomeSceneName = "Welcome";

    private bool isVisible;
    private Image overlayImage;

    private void Awake()
    {
        CacheOverlayImage();

        RegisterButton(resumeButton, Resume);
        RegisterButton(backButton, BackToWelcome);
        RegisterButton(newGameButton, NewGame);
        RegisterButton(fullscreenOffButton, () => SetFullScreen(false));
        RegisterButton(fullscreenOnButton, () => SetFullScreen(true));

        RegisterSlider(masterSlider, OnMasterVolumeChanged);
        RegisterSlider(musicSlider, OnMusicVolumeChanged);
        RegisterSlider(sfxSlider, OnSfxVolumeChanged);

        LoadSettings();
    }

    private void Start()
    {
        PrepareClosedState();
    }

    private void Update()
    {
        if (!WasEscapePressed())
        {
            return;
        }

        if (isVisible)
        {
            Resume();
        }
        else
        {
            Show();
        }
    }

    public void Toggle()
    {
        if (isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        if (isVisible)
        {
            return;
        }

        isVisible = true;
        Time.timeScale = 0f;

        SetActiveSafe(overlay, true);
        SetActiveSafe(contentRoot, true);

        LoadSettings();
        RefreshFullScreenButtons();
        ApplyOverlayVisual();
        SetCanvasGroup(overlayGroup, overlayTargetAlpha, true);
        SetCanvasGroup(contentGroup, 1f, true);
    }

    public void Hide()
    {
        if (!isVisible)
        {
            return;
        }

        isVisible = false;
        Time.timeScale = 1f;
        PrepareClosedState();
    }

    public void Resume()
    {
        Hide();
    }

    public void BackToWelcome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(welcomeSceneName);
    }

    public void NewGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat(KeyMasterVolume, 1f);
        float music = PlayerPrefs.GetFloat(KeyMusicVolume, 1f);
        float sfx = PlayerPrefs.GetFloat(KeySfxVolume, 1f);

        SetSliderValue(masterSlider, master);
        SetSliderValue(musicSlider, music);
        SetSliderValue(sfxSlider, sfx);

        ApplyAudioVolumes(master, music, sfx);
        ApplySavedFullScreen();
    }

    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(KeyMasterVolume, value);
        PlayerPrefs.Save();
        AudioListener.volume = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(KeyMusicVolume, value);
        PlayerPrefs.Save();
        ApplyMusicVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(KeySfxVolume, value);
        PlayerPrefs.Save();
        ApplySfxVolume(value);
    }

    private void SetFullScreen(bool enabled)
    {
        Screen.fullScreen = enabled;
        PlayerPrefs.SetInt(KeyFullScreen, enabled ? 1 : 0);
        PlayerPrefs.Save();
        RefreshFullScreenButtons();
    }

    private void ApplySavedFullScreen()
    {
        if (!PlayerPrefs.HasKey(KeyFullScreen))
        {
            RefreshFullScreenButtons();
            return;
        }

        bool enabled = PlayerPrefs.GetInt(KeyFullScreen, 0) == 1;
        Screen.fullScreen = enabled;
        RefreshFullScreenButtons();
    }

    private void RefreshFullScreenButtons()
    {
        bool isFullScreen = Screen.fullScreen;

        if (fullscreenOnButton != null)
        {
            fullscreenOnButton.interactable = !isFullScreen;
        }

        if (fullscreenOffButton != null)
        {
            fullscreenOffButton.interactable = isFullScreen;
        }
    }

    private void ApplyAudioVolumes(float master, float music, float sfx)
    {
        AudioListener.volume = master;
        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);
    }

    private void ApplyMusicVolume(float volume)
    {
        AudioSource source = GetAudioManagerSource(0);
        if (source != null)
        {
            source.volume = volume;
        }
    }

    private void ApplySfxVolume(float volume)
    {
        AudioSource source = GetAudioManagerSource(1);
        if (source != null)
        {
            source.volume = volume;
        }
    }

    private AudioSource GetAudioManagerSource(int index)
    {
        AudioManager manager = FindAnyObjectByType<AudioManager>();
        if (manager == null)
        {
            return null;
        }

        AudioSource[] sources = manager.GetComponents<AudioSource>();
        if (index < 0 || index >= sources.Length)
        {
            return null;
        }

        return sources[index];
    }

    private void PrepareClosedState()
    {
        SetCanvasGroupAlpha(overlayGroup, 0f);
        SetCanvasGroupAlpha(contentGroup, 0f);
        SetActiveSafe(contentRoot, false);
        SetActiveSafe(overlay, false);
    }

    private void CacheOverlayImage()
    {
        if (overlay == null)
        {
            return;
        }

        overlayImage = overlay.GetComponent<Image>();
    }

    private void ApplyOverlayVisual()
    {
        if (overlayImage == null)
        {
            CacheOverlayImage();
        }

        if (overlayImage == null)
        {
            return;
        }

        overlayImage.color = Color.black;
        overlayImage.enabled = true;
    }

    private static void RegisterButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private static void RegisterSlider(Slider slider, UnityEngine.Events.UnityAction<float> action)
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(action);
        }
    }

    private static void SetSliderValue(Slider slider, float value)
    {
        if (slider != null)
        {
            slider.SetValueWithoutNotify(value);
        }
    }

    private static void SetActiveSafe(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private static void SetCanvasGroup(CanvasGroup group, float alpha, bool blocksRaycasts)
    {
        if (group == null)
        {
            return;
        }

        group.alpha = alpha;
        group.blocksRaycasts = blocksRaycasts;
        group.interactable = blocksRaycasts;
    }

    private static void SetCanvasGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }

    private static bool WasEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}