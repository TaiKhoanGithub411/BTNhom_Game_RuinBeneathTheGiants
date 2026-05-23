using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Cấu trúc liên kết dữ liệu cấu hình vật phẩm (ItemData) với Prefab tương ứng trong thế giới.
    /// </summary>
    [System.Serializable]
    public struct ItemEntry
    {
        [Tooltip("Cấu hình ScriptableObject của vật phẩm")]
        public ItemData data;

        [Tooltip("Prefab thực tế có gắn component ItemPickup")]
        public GameObject prefab;
    }
}

/// <summary>
/// Database cho item, được mở rộng để hỗ trợ Procedural Spawning thông qua liên kết Prefab.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Items/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [Header("Old Database Config (Backward Compatibility)")]
    [SerializeField] private ItemData[] items;

    [Header("Procedural Spawning Config")]
    [Tooltip("Danh sách liên kết dữ liệu vật phẩm và prefab thực tế")]
    [SerializeField] private BTNhom.MapGen.ItemEntry[] itemEntries;

    public ItemData[] Items => items;
    public BTNhom.MapGen.ItemEntry[] ItemEntries => itemEntries;

    /// <summary>
    /// Lấy một vật phẩm ngẫu nhiên từ cơ sở dữ liệu (chỉ lấy dữ liệu).
    /// </summary>
    public ItemData GetRandomItem()
    {
        if (items == null || items.Length == 0) return null;
        return items[Random.Range(0, items.Length)];
    }

    /// <summary>
    /// Lấy ngẫu nhiên một liên kết vật phẩm (gồm dữ liệu và prefab) phục vụ sinh ngẫu nhiên.
    /// </summary>
    public BTNhom.MapGen.ItemEntry? GetRandomItemEntry()
    {
        if (itemEntries == null || itemEntries.Length == 0) return null;
        return itemEntries[Random.Range(0, itemEntries.Length)];
    }
}
