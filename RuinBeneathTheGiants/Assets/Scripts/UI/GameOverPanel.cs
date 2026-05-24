using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private CanvasGroup contentGroup;
    [SerializeField] private float overlayTargetAlpha = 1f;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button backButton;
    [SerializeField] private string welcomeSceneName = "Welcome";

    [SerializeField] private PlayerVitals playerVitals;

    private bool isVisible;

    private void Awake()
    {
        if (playerVitals == null)
            playerVitals = FindFirstObjectByType<PlayerVitals>();

        RegisterButton(retryButton, Retry);
        RegisterButton(backButton, BackToWelcome);
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

    public void Show()
    {
        if (isVisible) return;
        isVisible = true;
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToWelcome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(welcomeSceneName);
    }
}