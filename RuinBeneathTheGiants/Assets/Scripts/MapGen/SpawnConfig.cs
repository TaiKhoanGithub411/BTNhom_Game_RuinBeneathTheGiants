using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Dữ liệu cấu hình cho hệ thống tự động sinh vật phẩm/bẫy và logic tăng độ khó (Scaling).
    /// </summary>
    [CreateAssetMenu(menuName = "BTNhom/MapGen/Spawn Config", fileName = "SpawnConfig")]
    public class SpawnConfig : ScriptableObject
    {
        [Header("Base Spawn Probability (0.0 to 1.0)")]
        [Tooltip("Xác suất cơ bản sinh vật phẩm tại một SpawnPoint loại Item")]
        [SerializeField, Range(0f, 1f)] private float baseItemSpawnRate = 0.6f;

        [Tooltip("Xác suất cơ bản sinh bẫy tại một SpawnPoint loại Trap")]
        [SerializeField, Range(0f, 1f)] private float baseTrapSpawnRate = 0.4f;

        [Header("Difficulty Scaling")]
        [Tooltip("Khoảng cách (mét/units) Player cần vượt qua để tăng 1 cấp độ khó")]
        [SerializeField, Min(10f)] private float distanceIntervalForDifficulty = 50f;

        [Tooltip("Cấp độ khó tối đa (giới hạn trên để tránh mất cân bằng)")]
        [SerializeField, Min(1)] private int maxDifficultyLevel = 10;

        [Tooltip("Tỉ lệ bẫy tăng thêm mỗi cấp độ khó (ví dụ +0.05 tức là +5%)")]
        [SerializeField, Range(0f, 0.2f)] private float trapRateIncreasePerLevel = 0.05f;

        [Tooltip("Tỉ lệ vật phẩm giảm đi mỗi cấp độ khó (ví dụ -0.03 tức là -3%)")]
        [SerializeField, Range(0f, 0.2f)] private float itemRateDecreasePerLevel = 0.03f;

        // Getters
        public float BaseItemSpawnRate => baseItemSpawnRate;
        public float BaseTrapSpawnRate => baseTrapSpawnRate;
        public float DistanceIntervalForDifficulty => distanceIntervalForDifficulty;
        public int MaxDifficultyLevel => maxDifficultyLevel;
        public float TrapRateIncreasePerLevel => trapRateIncreasePerLevel;
        public float ItemRateDecreasePerLevel => itemRateDecreasePerLevel;

        /// <summary>
        /// Tính toán tỉ lệ sinh vật phẩm thực tế dựa vào độ khó hiện tại.
        /// </summary>
        public float GetAdjustedItemSpawnRate(int currentDifficulty)
        {
            float rate = baseItemSpawnRate - (currentDifficulty * itemRateDecreasePerLevel);
            return Mathf.Clamp(rate, 0.1f, 1f); // Giữ tối thiểu 10% để người chơi vẫn có cơ hội sinh tồn
        }

        /// <summary>
        /// Tính toán tỉ lệ sinh bẫy thực tế dựa vào độ khó hiện tại.
        /// </summary>
        public float GetAdjustedTrapSpawnRate(int currentDifficulty)
        {
            float rate = baseTrapSpawnRate + (currentDifficulty * trapRateIncreasePerLevel);
            return Mathf.Clamp(rate, 0f, 0.9f); // Giữ tối đa 90% tránh bẫy xuất hiện quá đặc đặc
        }
    }
}
