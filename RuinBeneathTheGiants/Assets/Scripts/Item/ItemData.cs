using UnityEngine;

/// <summary>
/// Dữ liệu cấu hình cho một loại item.
/// Item được gắn thời gian tồn tại để tự hủy sau khi rơi ra ngoài world.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Items/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;

    [Header("Effect")]
    [SerializeField] private ItemEffectType effectType;
    [SerializeField, Min(0f)] private float value = 10f;

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float lifetime = 10f;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public ItemEffectType EffectType => effectType;
    public float Value => value;
    public float Lifetime => lifetime;
}
