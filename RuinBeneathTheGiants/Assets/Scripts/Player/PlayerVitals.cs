using System;
using UnityEngine;

/// <summary>
/// Lớp chịu trách nhiệm ĐỘC QUYỀN về Sinh Lực (Sinh tồn):
/// - Máu, Stamina, Sát thương, Chết.
/// - Phát tín hiệu (Event) ra bên ngoài khi có thay đổi.
/// </summary>
public class PlayerVitals : MonoBehaviour
{
    // =========================
    // C# Events (Để HUD, Âm thanh, Game Manager lắng nghe)
    // =========================
    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;
    public event Action OnDeath;
    public event Action OnExhausted;
    public event Action OnTakeDamage; // Sự kiện mới khi bị nhận sát thương

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
    
    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;

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
        
        currentStamina += staminaRegenRate * Time.deltaTime;
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }
        OnStaminaChanged?.Invoke(currentStamina);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        OnTakeDamage?.Invoke(); // Kích hoạt sự kiện Hurt

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || IsDead) return;
        
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
            currentStamina -= amount;
            OnStaminaChanged?.Invoke(currentStamina);
            
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
        
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        OnStaminaChanged?.Invoke(currentStamina);
    }
}
