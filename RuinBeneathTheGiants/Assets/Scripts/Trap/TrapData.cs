using UnityEngine;

/// <summary>
/// Dữ liệu cấu hình cho một loại bẫy.
/// Bẫy được cấu hình các trạng thái hình ảnh, sát thương và thời gian tồn tại.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Traps/Trap Data", fileName = "NewTrapData")]
public class TrapData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string trapName;
    [SerializeField] private Sprite activeSprite;      // Sprite trạng thái hoạt động (chưa kích hoạt)
    [SerializeField] private Sprite triggeredSprite;   // Sprite trạng thái đã sập/kích hoạt (nếu có)

    [Header("Damage")]
    [SerializeField, Min(0f)] private float damage = 10f;

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float lifetime = 15f; // Tự hủy sau X giây ngoài world (để tránh rác bộ nhớ)

    public string TrapName => trapName;
    public Sprite ActiveSprite => activeSprite;
    public Sprite TriggeredSprite => triggeredSprite;
    public float Damage => damage;
    public float Lifetime => lifetime;
}
