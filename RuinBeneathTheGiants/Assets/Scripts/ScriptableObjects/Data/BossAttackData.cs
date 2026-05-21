using UnityEngine;

/// <summary>
/// ScriptableObject lưu trữ dữ liệu cấu hình cho đòn tấn công của Boss.
/// Giúp dễ dàng cân bằng game mà không cần mở code.
/// </summary>
[CreateAssetMenu(fileName = "NewBossAttackData", menuName = "Data/Boss Attack Data")]
public class BossAttackData : ScriptableObject
{
    [Header("Damage Settings")]
    [Tooltip("Lượng sát thương gây ra. Mặc định 9999 để giết ngay lập tức.")]
    public float damage = 9999f;

    [Header("Timing Settings")]
    [Tooltip("Thời gian boss lơ lửng chờ (cảnh báo) trước khi đạp xuống.")]
    public float telegraphTime = 1f;
    
    [Tooltip("Thời gian chờ tối thiểu giữa các lần đạp.")]
    public float cooldownMin = 2f;
    
    [Tooltip("Thời gian chờ tối đa giữa các lần đạp.")]
    public float cooldownMax = 5f;
}
