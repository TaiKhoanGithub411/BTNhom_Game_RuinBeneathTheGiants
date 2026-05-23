using UnityEngine;
using System.Collections.Generic;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Cấu trúc lưu trữ một cặp gồm cấu hình bẫy (TrapData) và Prefab tương ứng để sinh ra thế giới.
    /// </summary>
    [System.Serializable]
    public struct TrapEntry
    {
        [Tooltip("Cấu hình ScriptableObject chứa thông số bẫy")]
        public TrapData data;
        
        [Tooltip("Prefab thực tế có chứa component kế thừa TrapBase để khởi tạo")]
        public GameObject prefab;
    }

    /// <summary>
    /// Cơ sở dữ liệu quản lý toàn bộ các loại bẫy để sinh ngẫu nhiên trong game.
    /// </summary>
    [CreateAssetMenu(menuName = "BTNhom/MapGen/Trap Database", fileName = "TrapDatabase")]
    public class TrapDatabase : ScriptableObject
    {
        [SerializeField] private List<TrapEntry> traps = new List<TrapEntry>();

        public List<TrapEntry> Traps => traps;

        /// <summary>
        /// Lấy ngẫu nhiên một loại bẫy và prefab từ cơ sở dữ liệu.
        /// </summary>
        public TrapEntry? GetRandomTrap()
        {
            if (traps == null || traps.Count == 0) return null;
            return traps[Random.Range(0, traps.Count)];
        }
    }
}
