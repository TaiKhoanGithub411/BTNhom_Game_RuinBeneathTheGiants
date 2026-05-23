using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Dữ liệu cấu hình cho một đoạn bản đồ (Chunk) sinh ngẫu nhiên.
    /// </summary>
    [CreateAssetMenu(menuName = "BTNhom/MapGen/Chunk Data", fileName = "NewChunkData")]
    public class ChunkData : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("Tên nhận diện của chunk")]
        [SerializeField] private string chunkName;

        [Tooltip("Prefab chứa địa hình và các SpawnPoint của chunk")]
        [SerializeField] private GameObject prefab;

        [Tooltip("Chiều rộng của chunk tính bằng đơn vị Unity (World Units) để tính toán điểm ghép nối")]
        [SerializeField, Min(1f)] private float width = 20f;

        [Header("Difficulty Settings")]
        [Tooltip("Độ khó tối thiểu để chunk này bắt đầu xuất hiện")]
        [SerializeField, Min(0)] private int minDifficulty = 0;

        [Tooltip("Độ khó tối đa để chunk này xuất hiện (0 nghĩa là không giới hạn)")]
        [SerializeField, Min(0)] private int maxDifficulty = 0;

        // Getters
        public string ChunkName => chunkName;
        public GameObject Prefab => prefab;
        public float Width => width;
        public int MinDifficulty => minDifficulty;
        public int MaxDifficulty => maxDifficulty;
    }
}
