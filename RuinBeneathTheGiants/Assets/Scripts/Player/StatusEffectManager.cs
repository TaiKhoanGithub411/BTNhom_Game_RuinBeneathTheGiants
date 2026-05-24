using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý các hiệu ứng kéo dài theo thời gian của Player.
/// (Phase 2 - Status Effects)
/// </summary>
[RequireComponent(typeof(PlayerVitals))]
public class StatusEffectManager : MonoBehaviour
{
    private PlayerVitals playerVitals;

    [Header("Current Status")]
    [SerializeField] private bool isPoisoned = false;
    [SerializeField] private bool hasPoisonImmunity = false;
    [SerializeField] private bool hasTrapResistance = false;

    // Các biến đếm thời gian
    private float immunityEndTime = 0f;
    private float trapResistanceEndTime = 0f;

    // Tham chiếu Coroutine để có thể dừng nếu cần
    private Coroutine poisonCoroutine;
    private Coroutine bleedCoroutine;

    // Các biến ảnh hưởng chỉ số phụ
    private float staminaRegenMultiplier = 1f;
    public float StaminaRegenMultiplier => staminaRegenMultiplier;

    private void Awake()
    {
        playerVitals = GetComponent<PlayerVitals>();
    }

    private void Update()
    {
        // Tự động tắt miễn nhiễm độc khi hết thời gian
        if (hasPoisonImmunity && Time.time >= immunityEndTime)
        {
            hasPoisonImmunity = false;
        }

        // Tự động tắt kháng bẫy khi hết thời gian
        if (hasTrapResistance && Time.time >= trapResistanceEndTime)
        {
            hasTrapResistance = false;
        }
    }

    /// <summary>
    /// Kích hoạt hiệu ứng Miễn Nhiễm Độc
    /// </summary>
    public void ApplyPoisonImmunity(float duration)
    {
        hasPoisonImmunity = true;
        immunityEndTime = Mathf.Max(immunityEndTime, Time.time + duration);

        // Nếu đang bị độc thì giải độc ngay lập tức
        if (isPoisoned)
        {
            CurePoison();
        }
    }

    /// <summary>
    /// Kích hoạt hiệu ứng Độc (Rút máu từ từ)
    /// </summary>
    public void ApplyPoison(float totalDamage, float duration)
    {
        // Nếu đang có miễn nhiễm độc thì vô hiệu hóa nấm độc
        if (hasPoisonImmunity) return;

        // Nếu đang bị độc rồi, ta ghi đè hoặc cộng dồn tùy thiết kế. 
        // Ở đây chọn cách ghi đè (Reset thời gian độc mới nhất)
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }

        poisonCoroutine = StartCoroutine(PoisonRoutine(totalDamage, duration));
    }

    private IEnumerator PoisonRoutine(float totalDamage, float duration)
    {
        isPoisoned = true;
        float damagePerSecond = totalDamage / duration;
        float elapsed = 0f;

        while (elapsed < duration && !playerVitals.IsDead)
        {
            // Trừ máu từ từ ngầm (không phát event Hurt để khỏi giật cục và kẹt hoạt ảnh)
            playerVitals.TakeDamageSilent(damagePerSecond * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isPoisoned = false;
    }

    /// <summary>
    /// Kích hoạt hiệu ứng Kháng Bẫy (Medical)
    /// </summary>
    public void ApplyTrapResistance(float duration)
    {
        hasTrapResistance = true;
        trapResistanceEndTime = Mathf.Max(trapResistanceEndTime, Time.time + duration);
    }

    /// <summary>
    /// Giải độc tức thì
    /// </summary>
    public void CurePoison()
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
            poisonCoroutine = null;
        }
        isPoisoned = false;
    }

    /// <summary>
    /// Trả về hệ số nhân sát thương bẫy.
    /// Nếu có kháng bẫy -> chỉ chịu 50% sát thương (0.5f).
    /// Bình thường -> chịu 100% sát thương (1f).
    /// </summary>
    public float GetTrapDamageMultiplier()
    {
        return hasTrapResistance ? 0.5f : 1f;
    }

    /// <summary>
    /// Kích hoạt hiệu ứng Chảy máu và Khập khiễng (Từ Bẫy Gấu)
    /// </summary>
    public void ApplyBleedAndCripple(float totalDamage, float duration, float staminaPenaltyMult = 0.8f)
    {
        if (bleedCoroutine != null)
        {
            StopCoroutine(bleedCoroutine);
        }
        bleedCoroutine = StartCoroutine(BleedAndCrippleRoutine(totalDamage, duration, staminaPenaltyMult));
    }

    private IEnumerator BleedAndCrippleRoutine(float totalDamage, float duration, float staminaPenaltyMult)
    {
        // Giảm tốc độ hồi Stamina
        staminaRegenMultiplier = staminaPenaltyMult;

        float damagePerSecond = totalDamage / duration;
        float elapsed = 0f;

        while (elapsed < duration && !playerVitals.IsDead)
        {
            playerVitals.TakeDamageSilent(damagePerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hồi phục lại tốc độ Stamina sau khi hết hiệu ứng
        staminaRegenMultiplier = 1f;
    }

    // Biến trạng thái để Animator dùng (đổi màu xanh lá chẳng hạn)
    public bool IsPoisoned => isPoisoned;
}
