using UnityEngine;

/// <summary>
/// Placeholder database cho item, để sau này mở rộng data-driven dễ hơn.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Items/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private ItemData[] items;

    public ItemData[] Items => items;

    /// <summary>
    /// Lấy một vật phẩm ngẫu nhiên từ cơ sở dữ liệu.
    /// </summary>
    public ItemData GetRandomItem()
    {
        if (items == null || items.Length == 0) return null;
        return items[Random.Range(0, items.Length)];
    }
}
