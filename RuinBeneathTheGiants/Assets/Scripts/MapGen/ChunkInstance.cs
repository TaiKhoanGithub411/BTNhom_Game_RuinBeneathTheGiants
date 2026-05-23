using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Component quản lý thực thể của một Chunk khi được đưa vào màn chơi.
    /// Giúp theo dõi ranh giới địa hình và lưu vết trạng thái sinh vật thể (Item/Trap) khi Player quay đầu.
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
        public float LeftEdge => transform.position.x;
        public float RightEdge => transform.position.x + (ChunkData != null ? ChunkData.Width : 0f);
        
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
        /// </summary>
        public void Initialize(ChunkData data, int difficulty)
        {
            this.ChunkData = data;
            this.spawnDifficulty = difficulty;
            this.isPopulated = false; // Mặc định khi mới sinh ra là chưa có vật phẩm
        }
    }
}
