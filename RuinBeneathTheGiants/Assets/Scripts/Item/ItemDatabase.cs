using UnityEngine;

/// <summary>
/// Placeholder database cho item, để sau này mở rộng data-driven dễ hơn.
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Items/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private ItemData[] items;

    public ItemData[] Items => items;
}
