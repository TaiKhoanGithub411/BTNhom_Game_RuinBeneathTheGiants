using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Component quản lý thực thể của một Chunk khi được đưa vào màn chơi.
    /// Giúp theo dõi ranh giới địa hình và lưu vết trạng thái sinh vật thể (Item/Trap) khi Player quay đầu.
    /// Tự động quét Tilemap để dịch chuyển khớp với các Chunk lân cận.
    /// </summary>
    public class ChunkInstance : MonoBehaviour
    {
        [Header("Runtime Info")]
        [Tooltip("Có sinh vật thể (Item/Trap) tại chunk này chưa")]
        [SerializeField] private bool isPopulated = false;

        [Tooltip("Độ khó tại thời điểm sinh chunk")]
        [SerializeField] private int spawnDifficulty = 0;

        // Các thuộc tính lưu giữ thông số tính toán ranh giới
        public ChunkData ChunkData { get; private set; }
        public float LeftEdge { get; private set; }
        public float RightEdge { get; private set; }
        
        public bool IsPopulated
        {
            get => isPopulated;
            set => isPopulated = value;
        }

        public int SpawnDifficulty
        {
            get => spawnDifficulty;
            set => spawnDifficulty = value;
        }

        /// <summary>
        /// Khởi tạo thông tin cho chunk khi được spawn bởi ChunkManager.
        /// Tự động dịch chuyển Chunk sao cho mép gạch vừa khít với targetX.
        /// </summary>
        public void Initialize(ChunkData data, int difficulty, float targetX, bool isSpawningRight)
        {
            this.ChunkData = data;
            this.spawnDifficulty = difficulty;
            this.isPopulated = false; // Mặc định khi mới sinh ra là chưa có vật phẩm

            CalculateAndAlignBounds(targetX, isSpawningRight);
        }

        private void CalculateAndAlignBounds(float targetX, bool isSpawningRight)
        {
            UnityEngine.Tilemaps.TilemapRenderer[] tilemaps = GetComponentsInChildren<UnityEngine.Tilemaps.TilemapRenderer>();
            
            if (tilemaps.Length == 0)
            {
                // Nếu không có Tilemap, dự phòng bằng thông số gốc
                LeftEdge = transform.position.x;
                RightEdge = transform.position.x + (ChunkData != null ? ChunkData.Width : 20f);
                return;
            }

            float minX = float.MaxValue;
            float maxX = float.MinValue;

            // Quét ranh giới thực tế của tất cả các gạch
            foreach (var tm in tilemaps)
            {
                if (tm.bounds.min.x < minX) minX = tm.bounds.min.x;
                if (tm.bounds.max.x > maxX) maxX = tm.bounds.max.x;
            }

            float shiftAmount = 0f;
            
            if (isSpawningRight)
            {
                // Nếu sinh bên phải, ta muốn mép trái của gạch (minX) trùng khít với mép phải của Chunk cũ (targetX)
                shiftAmount = targetX - minX;
            }
            else
            {
                // Nếu sinh bên trái, ta muốn mép phải của gạch (maxX) trùng khít với mép trái của Chunk cũ (targetX)
                shiftAmount = targetX - maxX;
            }

            // Dịch chuyển toàn bộ Prefab
            transform.position += new Vector3(shiftAmount, 0f, 0f);

            // Ghi nhận lại ranh giới chính thức sau khi dịch chuyển
            LeftEdge = minX + shiftAmount;
            RightEdge = maxX + shiftAmount;
        }
    }
}
