using UnityEngine;
using System.Collections.Generic;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Cấu trúc lưu trữ một Prefab vùng Boss (BossEncounterTrigger).
    /// </summary>
    [System.Serializable]
    public struct BossZoneEntry
    {
        [Tooltip("Prefab thực tế có chứa component BossEncounterTrigger để khởi tạo")]
        public GameObject prefab;
    }

    /// <summary>
    /// Cơ sở dữ liệu quản lý toàn bộ các loại vùng kích hoạt Boss để sinh ngẫu nhiên trong game.
    /// </summary>
    [CreateAssetMenu(menuName = "BTNhom/MapGen/Boss Database", fileName = "BossDatabase")]
    public class BossDatabase : ScriptableObject
    {
        [SerializeField] private List<BossZoneEntry> bossZones = new List<BossZoneEntry>();

        public List<BossZoneEntry> BossZones => bossZones;

        /// <summary>
        /// Lấy ngẫu nhiên một vùng Boss từ cơ sở dữ liệu.
        /// </summary>
        public BossZoneEntry? GetRandomBossZone()
        {
            if (bossZones == null || bossZones.Count == 0) return null;
            return bossZones[Random.Range(0, bossZones.Count)];
        }
    }
}
