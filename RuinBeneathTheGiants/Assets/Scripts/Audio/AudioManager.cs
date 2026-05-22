using UnityEngine;

/// <summary>
/// Quản lý toàn bộ âm thanh trong game.
///
/// Ý tưởng là các hệ thống gameplay khác chỉ phát event.
/// AudioManager sẽ lắng nghe event đó và quyết định clip nào cần phát.
/// Nhờ vậy, âm thanh được gom về một chỗ, tránh việc PlayerVitals hay
/// các class gameplay phải biết quá nhiều về AudioClip.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip gameoverMusic;
    [SerializeField] private AudioClip collectitemSFX;
    [SerializeField] private AudioClip takedamageSFX;
    [SerializeField] private AudioClip opendoorSFX;
    [SerializeField] private AudioClip giantfallSFX;

    [Header("Auto Bind")]
    [SerializeField] private PlayerVitals playerVitals;

    private void Awake()
    {
        // Nếu chưa kéo thả playerVitals trong Inspector thì tự tìm ở scene.
        // Cách này tiện cho prototype, nhưng nếu game có nhiều nhân vật thì
        // nên gán thủ công để tránh AudioManager nghe nhầm nhân vật.
        if (playerVitals == null)
        {
            playerVitals = FindFirstObjectByType<PlayerVitals>();
        }
    }

    private void OnEnable()
    {
        // Đăng ký nghe event từ PlayerVitals.
        // PlayerVitals chỉ phát tín hiệu, còn việc phát âm thanh là trách nhiệm của AudioManager.
        if (playerVitals != null)
        {
            playerVitals.OnTakeDamage += HandleTakeDamage;
            playerVitals.OnDeath += HandleDeath;
            playerVitals.OnExhausted += HandleExhausted;
        }
    }

    private void Start()
    {
        // Bắt đầu bằng nhạc nền của game.
        PlayBackgroundMusic();
    }

    private void OnDisable()
    {
        // Gỡ đăng ký để tránh memory leak và callback vào object đã bị vô hiệu hóa.
        if (playerVitals != null)
        {
            playerVitals.OnTakeDamage -= HandleTakeDamage;
            playerVitals.OnDeath -= HandleDeath;
            playerVitals.OnExhausted -= HandleExhausted;
        }
    }

    private void HandleTakeDamage()
    {
        // Khi nhân vật nhận sát thương, AudioManager phát sound effect tương ứng.
        PlaySFX(takedamageSFX);
    }

    private void HandleDeath()
    {
        // Khi chết, chuyển sang nhạc game over.
        PlayGameOverMusic();
    }

    private void HandleExhausted()
    {
        // Đây là ví dụ một âm thanh cảnh báo khi stamina cạn.
        // Nếu sau này bạn có clip riêng cho exhausted thì chỉ cần đổi ở đây.
        PlaySFX(takedamageSFX);
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null) return;

        musicSource.Stop();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameOverMusic()
    {
        if (musicSource == null || gameoverMusic == null) return;

        musicSource.Stop();
        musicSource.clip = gameoverMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource == null || clip == null) return;

        SFXSource.PlayOneShot(clip);
    }

    public void PlayCollectItemSFX()
    {
        PlaySFX(collectitemSFX);
    }

    public void PlayOpenDoorSFX()
    {
        PlaySFX(opendoorSFX);
    }

    public void PlayGiantFallSFX()
    {
        PlaySFX(giantfallSFX);
    }
}
