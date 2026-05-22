using UnityEngine;

/// <summary>
/// Kiểu hiệu ứng mà item có thể mang theo.
/// </summary>
public enum ItemEffectType
{
    HealthRestore,       // Hồi máu
    StaminaRestore,      // Hồi thể lực
    PoisonOverTime,      // Trúng độc (trừ máu từ từ)
    PoisonImmunity,      // Miễn nhiễm độc
    TrapResistance       // Giảm sát thương bẫy
}
