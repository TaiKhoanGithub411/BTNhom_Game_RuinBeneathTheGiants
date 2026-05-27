using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private CanvasGroup contentGroup;

    [Header("Overlay")]
    [SerializeField] private float overlayTargetAlpha = 1f;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button giveUpButton;

    [Header("Scenes")]
    [SerializeField] private string welcomeSceneName = "Welcome";

    [Header("Player")]
    [SerializeField] private PlayerVitals playerVitals;

    private bool isVisible;
    private Image overlayImage;

    private void Awake()
    {
        if (playerVitals == null)
            playerVitals = FindAnyObjectByType<PlayerVitals>();

        RegisterButton(retryButton, Retry);
        RegisterButton(giveUpButton, GiveUp);
    }

    private void OnEnable()
    {
        if (playerVitals != null)
            playerVitals.OnDeath += Show;
    }

    private void OnDisable()
    {
        if (playerVitals != null)
            playerVitals.OnDeath -= Show;
    }

    private void Start() => PrepareClosedState();

    /// Hiện panel khi chết — overlay đen + nội dung + dừng game
    public void Show()
    {
        if (isVisible)
            return;

        isVisible = true;
        Time.timeScale = 0f;

        SetActiveSafe(overlay, true);
        SetActiveSafe(contentRoot, true);

        ApplyOverlayVisual();
        SetCanvasGroup(overlayGroup, overlayTargetAlpha, true);
        SetCanvasGroup(contentGroup, 1f, true);
    }

    /// RETRY — chơi lại scene hiện tại
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// GIVE UP — về menu Welcome
    public void GiveUp()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(welcomeSceneName);
    }

    private void PrepareClosedState()
    {
        isVisible = false;
        SetCanvasGroupAlpha(overlayGroup, 0f);
        SetCanvasGroupAlpha(contentGroup, 0f);
        SetActiveSafe(contentRoot, false);
        SetActiveSafe(overlay, false);
    }

    private void CacheOverlayImage()
    {
        if (overlay == null)
            return;

        overlayImage = overlay.GetComponent<Image>();
    }

    private void ApplyOverlayVisual()
    {
        if (overlayImage == null)
            CacheOverlayImage();

        if (overlayImage == null)
            return;

        overlayImage.color = Color.black;
        overlayImage.enabled = true;
    }

    private static void RegisterButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    private static void SetActiveSafe(GameObject target, bool active)
    {
        if (target != null)
            target.SetActive(active);
    }

    private static void SetCanvasGroup(CanvasGroup group, float alpha, bool blocksRaycasts)
    {
        if (group == null)
            return;

        group.alpha = alpha;
        group.blocksRaycasts = blocksRaycasts;
        group.interactable = blocksRaycasts;
    }

    private static void SetCanvasGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
            group.alpha = alpha;
    }
}