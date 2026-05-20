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

    public bool IsDead => currentHealth <= 0f;
    public bool IsExhausted => currentStamina <= 0f;

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
