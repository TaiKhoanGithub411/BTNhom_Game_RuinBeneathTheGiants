using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ItemEffect
{
    public ItemEffectType EffectType;
    [Tooltip("Giá trị hồi máu/thể lực, hoặc tổng sát thương độc")]
    public float Value;
    [Tooltip("Thời gian tác dụng (giây). Dành cho Độc, Kháng Độc, Kháng Bẫy")]
    public float Duration;
}

/// <summary>
/// Dữ liệu cấu hình cho một loại item.
/// Hỗ trợ nhiều hiệu ứng cùng lúc trên 1 item.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Items/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;

    [Header("Effects")]
    [Tooltip("Danh sách các hiệu ứng khi ăn item này")]
    [SerializeField] private List<ItemEffect> effects = new List<ItemEffect>();

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float lifetime = 10f;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public List<ItemEffect> Effects => effects;
    public float Lifetime => lifetime;
}
