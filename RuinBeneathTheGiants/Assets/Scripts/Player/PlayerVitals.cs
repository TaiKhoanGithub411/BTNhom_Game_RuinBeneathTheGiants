using System;
using UnityEngine;

/// <summary>
/// Lớp này chỉ chịu trách nhiệm về chỉ số sống còn của nhân vật.
///
/// Nguyên tắc kiến trúc ở đây là:
/// - `PlayerVitals` KHÔNG phát âm thanh.
/// - `PlayerVitals` chỉ thay đổi dữ liệu và phát event báo cho bên ngoài biết.
/// - Các hệ thống khác như UI, Audio, GameManager sẽ tự lắng nghe event này và xử lý phần việc của mình.
///
/// Cách làm này giúp mã dễ bảo trì hơn vì logic gameplay và logic âm thanh không bị trộn lẫn.
/// </summary>
public class PlayerVitals : MonoBehaviour
{
    private StatusEffectManager statusEffectManager;

    // =========================================================
    // Events
    // ---------------------------------------------------------
    // Các event này là "tín hiệu". Bản thân class không biết ai
    // sẽ nghe, nó chỉ báo rằng một trạng thái nào đó đã xảy ra.
    // =========================================================
    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;
    public event Action OnDeath;
    public event Action OnExhausted;
    public event Action OnTakeDamage;

    [Header("Stats Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;
    [SerializeField] private float staminaDrainRate = 15f; // Tốc độ tụt stamina khi chạy
    [SerializeField] private float staminaRegenRate = 10f; // Tốc độ hồi stamina

    public bool IsDead => currentHealth <= 0f;
    
    private bool isExhausted = false;
    public bool IsExhausted => isExhausted;
    
    // Trạng thái bị kẹp bởi bẫy
    private bool isTrapped = false;
    public bool IsTrapped => isTrapped;
    
    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;

    private void Awake()
    {
        statusEffectManager = GetComponent<StatusEffectManager>();
    }

    public void DrainStaminaContinuous()
    {
        if (isExhausted || IsDead) return;
        
        currentStamina -= staminaDrainRate * Time.deltaTime;
        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            isExhausted = true; // Bắt đầu trạng thái kiệt sức
            OnExhausted?.Invoke();
            StartCoroutine(ExhaustionRecoveryRoutine()); // Khóa di chuyển 2.5 giây
        }
        OnStaminaChanged?.Invoke(currentStamina);
    }

    private System.Collections.IEnumerator ExhaustionRecoveryRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        isExhausted = false;
    }

    public void RegenStaminaContinuous()
    {
        if (IsDead || currentStamina >= maxStamina) return;
        
        float regenMultiplier = statusEffectManager != null ? statusEffectManager.StaminaRegenMultiplier : 1f;
        currentStamina += (staminaRegenRate * regenMultiplier) * Time.deltaTime;

        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }
        OnStaminaChanged?.Invoke(currentStamina);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || IsDead) return;

        // Trừ máu trước, sau đó thông báo cho hệ thống khác biết trạng thái đã đổi.
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        OnTakeDamage?.Invoke();

        // Nếu máu về 0, chỉ phát event OnDeath.
        // Phần phát nhạc/game over sẽ do AudioManager hoặc hệ thống game điều khiển.
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Nhận sát thương ngầm (không kích hoạt event OnTakeDamage để tránh spam animation).
    /// Dùng cho sát thương theo thời gian như Độc.
    /// </summary>
    public void TakeDamageSilent(float damage)
    {
        if (damage <= 0f || IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Nhận sát thương từ từ trong một khoảng thời gian, đồng thời bị kẹp tại chỗ.
    /// Dùng cho Trap.
    /// </summary>
    public void TakeDamageOverTime(float totalDamage, float duration)
    {
        if (IsDead) return;
        
        // Nếu có buff Y tế, giảm sát thương bẫy
        if (statusEffectManager != null)
        {
            totalDamage *= statusEffectManager.GetTrapDamageMultiplier();
        }
        
        StartCoroutine(DamageOverTimeRoutine(totalDamage, duration));
    }

    private System.Collections.IEnumerator DamageOverTimeRoutine(float totalDamage, float duration)
    {
        isTrapped = true; // Khóa di chuyển
        OnTakeDamage?.Invoke(); // Báo hiệu bị thương để kích hoạt animation Hurt ban đầu
        
        float damagePerSecond = totalDamage / duration;
        float elapsed = 0f;
        
        while (elapsed < duration && !IsDead)
        {
            currentHealth -= damagePerSecond * Time.deltaTime;
            currentHealth = Mathf.Max(0f, currentHealth);
            OnHealthChanged?.Invoke(currentHealth);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
        
        isTrapped = false; // Mở khóa di chuyển sau khi hết kẹp
    }

    /// <summary>
    /// Chịu va chạm từ bẫy (khóa di chuyển trong 1 khoảng thời gian ngắn và phát hoạt ảnh Hurt).
    /// </summary>
    public void TakeTrapImpact(float lockDuration = 1.5f)
    {
        if (IsDead) return;
        StartCoroutine(TrapLockRoutine(lockDuration));
    }

    private System.Collections.IEnumerator TrapLockRoutine(float duration)
    {
        isTrapped = true;
        OnTakeDamage?.Invoke(); // Trigger Hurt animation
        yield return new WaitForSeconds(duration);
        isTrapped = false;
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || IsDead) return;

        // Chỉ cập nhật dữ liệu và phát event, không xử lý âm thanh ở đây.
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Tiêu hao Stamina. Trả về True nếu đủ Stamina để thực hiện, False nếu không đủ.
    /// </summary>
    public bool ConsumeStamina(float amount)
    {
        if (amount <= 0f || IsDead) return false;

        if (currentStamina >= amount)
        {
            // Trừ stamina và báo cho hệ thống lắng nghe biết.
            currentStamina -= amount;
            OnStaminaChanged?.Invoke(currentStamina);

            // Khi stamina chạm 0, phát event để bên ngoài tự quyết định
            // có hiển thị cảnh báo, phát âm thanh hay xử lý gì khác.
            if (currentStamina <= 0f)
            {
                OnExhausted?.Invoke();
            }

            return true;
        }

        return false;
    }

    public void RestoreStamina(float amount)
    {
        if (amount <= 0f || IsDead) return;

        // Giống các hàm khác, chỉ cập nhật trạng thái và phát event.
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        OnStaminaChanged?.Invoke(currentStamina);
    }
}
