using UnityEngine;
using System.Collections.Generic;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Class Singleton chịu trách nhiệm quản lý vòng đời của các Chunk địa hình trong game.
    /// - Tự động theo dõi vị trí của Player để sinh chunk mới phía trước hoặc phía sau (khi quay đầu).
    /// - Hủy các chunk đã ở quá xa để tiết kiệm tài nguyên.
    /// - Kiểm soát việc sinh vật phẩm/bẫy: không sinh lại khi Player quay lại vùng đất cũ.
    /// - Tự động tính toán độ khó tăng dần dựa trên quãng đường xa nhất Player đi được.
    /// </summary>
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager Instance { get; private set; }

        [Header("Databases")]
        [Tooltip("Cơ sở dữ liệu chứa danh sách địa hình các Chunk")]
        [SerializeField] private ChunkDatabase chunkDatabase;

        [Tooltip("Cơ sở dữ liệu chứa danh sách vật phẩm")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Tooltip("Cơ sở dữ liệu chứa danh sách bẫy")]
        [SerializeField] private TrapDatabase trapDatabase;

        [Header("Configuration")]
        [Tooltip("Cấu hình tỉ lệ spawn và scaling độ khó")]
        [SerializeField] private SpawnConfig spawnConfig;

        [Header("Spawning Parameters")]
        [Tooltip("Khoảng cách tối thiểu sinh thêm chunk phía trước Player")]
        [SerializeField] private float spawnAheadDistance = 35f;

        [Tooltip("Khoảng cách tối thiểu sinh thêm chunk phía sau Player khi quay lại")]
        [SerializeField] private float keepBehindDistance = 35f;

        [Tooltip("Khoảng cách tối đa để hủy chunk (phải lớn hơn spawnAhead/keepBehind để tránh lỗi giật lag)")]
        [SerializeField] private float despawnDistance = 60f;

        [Tooltip("Vị trí bắt đầu sinh chunk đầu tiên")]
        [SerializeField] private Transform initialSpawnPoint;

        private Transform playerTransform;
        private List<ChunkInstance> activeChunks = new List<ChunkInstance>();
        private float maxPlayerX = 0f; // Điểm X xa nhất người chơi từng đạt tới

        private void Awake()
        {
            // Thiết lập Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            FindPlayer();

            // Khởi tạo điểm bắt đầu cho maxPlayerX
            if (playerTransform != null)
            {
                maxPlayerX = playerTransform.position.x;
            }
            else
            {
                maxPlayerX = initialSpawnPoint != null ? initialSpawnPoint.position.x : 0f;
            }

            // Đảm bảo thông số an toàn tránh vòng lặp vô hạn sinh - hủy
            float minSafeDespawn = Mathf.Max(spawnAheadDistance, keepBehindDistance) + 10f;
            if (despawnDistance < minSafeDespawn)
            {
                despawnDistance = minSafeDespawn;
                Debug.LogWarning($"[ChunkManager] DespawnDistance quá nhỏ! Đã tự động điều chỉnh lên {despawnDistance} để an toàn.");
            }

            // Sinh chunk đầu tiên tại điểm xuất phát
            SpawnInitialChunk();
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            // Cập nhật khoảng cách kỷ lục Player từng đi được (chỉ tính tiến về bên phải)
            if (playerTransform.position.x > maxPlayerX)
            {
                maxPlayerX = playerTransform.position.x;
            }

            ManageChunks();
        }

        /// <summary>
        /// Tìm kiếm đối tượng Player trong scene dựa theo Tag.
        /// </summary>
        private void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        /// <summary>
        /// Quản lý việc sinh và hủy các chunk tùy thuộc vào vị trí hiện tại của Player.
        /// </summary>
        private void ManageChunks()
        {
            float playerX = playerTransform.position.x;

            // 1. Sinh thêm chunk bên PHẢI (tiến lên)
            float rightmostEdge = activeChunks.Count > 0 ? activeChunks[activeChunks.Count - 1].RightEdge : playerX;
            if (rightmostEdge < playerX + spawnAheadDistance)
            {
                SpawnChunkToRight();
            }

            // 2. Sinh thêm chunk bên TRÁI (khi Player đi lùi lại rìa trái)
            float leftmostEdge = activeChunks.Count > 0 ? activeChunks[0].LeftEdge : playerX;
            if (leftmostEdge > playerX - keepBehindDistance)
            {
                SpawnChunkToLeft();
            }

            // 3. Tự động dọn dẹp các chunk quá xa tầm nhìn cả 2 phía để tối ưu hóa hiệu năng
            for (int i = activeChunks.Count - 1; i >= 0; i--)
            {
                ChunkInstance chunk = activeChunks[i];
                
                // Chunk nằm hoàn toàn bên trái vùng giới hạn hoặc hoàn toàn bên phải vùng giới hạn
                if (chunk.RightEdge < playerX - despawnDistance || chunk.LeftEdge > playerX + despawnDistance)
                {
                    activeChunks.RemoveAt(i);
                    Destroy(chunk.gameObject);
                }
            }
        }

        /// <summary>
        /// Sinh chunk khởi tạo ban đầu khi vào game.
        /// </summary>
        private void SpawnInitialChunk()
        {
            float startX = initialSpawnPoint != null ? initialSpawnPoint.position.x : 0f;

            // Chọn chunk đầu tiên (độ khó 0)
            ChunkData chunkData = chunkDatabase != null ? chunkDatabase.GetRandomChunk(0) : null;
            if (chunkData == null)
            {
                Debug.LogError("[ChunkManager] Không tìm thấy Chunk nào trong Database để khởi tạo!");
                return;
            }

            Vector3 spawnPos = new Vector3(startX, 0f, 0f);
            GameObject spawnedObj = Instantiate(chunkData.Prefab, spawnPos, Quaternion.identity, transform);
            ChunkInstance instance = spawnedObj.AddComponent<ChunkInstance>();
            instance.Initialize(chunkData, 0);

            activeChunks.Add(instance);

            // Sinh vật phẩm/bẫy ban đầu
            if (spawnConfig != null && itemDatabase != null && trapDatabase != null)
            {
                ObjectSpawner.PopulateChunk(instance, spawnConfig, itemDatabase, trapDatabase, 0);
            }
        }

        /// <summary>
        /// Sinh một chunk mới ghép nối vào bên phải của chunk cuối cùng hiện tại.
        /// </summary>
        private void SpawnChunkToRight()
        {
            float spawnX = 0f;
            if (activeChunks.Count > 0)
            {
                spawnX = activeChunks[activeChunks.Count - 1].RightEdge;
            }
            else
            {
                if (playerTransform != null) spawnX = playerTransform.position.x;
            }

            int currentDiff = GetCurrentDifficulty();
            ChunkData chunkData = chunkDatabase.GetRandomChunk(currentDiff);
            if (chunkData == null) return;

            Vector3 spawnPos = new Vector3(spawnX, 0f, 0f);
            GameObject spawnedObj = Instantiate(chunkData.Prefab, spawnPos, Quaternion.identity, transform);
            ChunkInstance instance = spawnedObj.AddComponent<ChunkInstance>();
            instance.Initialize(chunkData, currentDiff);

            activeChunks.Add(instance);

            // Chỉ sinh vật phẩm/bẫy nếu đây là vùng đất mới (nằm ở rìa tiến lên so với maxPlayerX)
            // Có buffer nhỏ 2 unit để tránh sai số nhỏ của float
            if (spawnX + 2f >= maxPlayerX)
            {
                if (spawnConfig != null && itemDatabase != null && trapDatabase != null)
                {
                    ObjectSpawner.PopulateChunk(instance, spawnConfig, itemDatabase, trapDatabase, currentDiff);
                }
            }
            else
            {
                // Vùng đất cũ do đi lùi sinh lại -> Chỉ có địa hình
                instance.IsPopulated = true;
            }
        }

        /// <summary>
        /// Sinh một chunk mới ghép nối vào bên trái của chunk đầu tiên hiện tại (khi người chơi quay đầu).
        /// </summary>
        private void SpawnChunkToLeft()
        {
            if (activeChunks.Count == 0) return;
            
            ChunkInstance leftmost = activeChunks[0];
            int currentDiff = GetCurrentDifficulty();
            ChunkData chunkData = chunkDatabase.GetRandomChunk(currentDiff);
            if (chunkData == null) return;

            // Vị trí spawn bằng mép trái của chunk trái nhất trừ đi chiều rộng của chunk mới sinh
            float spawnX = leftmost.LeftEdge - chunkData.Width;
            Vector3 spawnPos = new Vector3(spawnX, 0f, 0f);

            GameObject spawnedObj = Instantiate(chunkData.Prefab, spawnPos, Quaternion.identity, transform);
            ChunkInstance instance = spawnedObj.AddComponent<ChunkInstance>();
            instance.Initialize(chunkData, currentDiff);

            // Chèn vào đầu danh sách activeChunks vì nó nằm bên trái nhất
            activeChunks.Insert(0, instance);

            // Đi lùi luôn là đi vào vùng đất cũ -> Tuyệt đối không sinh thêm vật phẩm/bẫy mới
            instance.IsPopulated = true;
        }

        /// <summary>
        /// Tính toán độ khó hiện tại của game dựa trên khoảng cách xa nhất Player đi được.
        /// </summary>
        public int GetCurrentDifficulty()
        {
            if (spawnConfig == null) return 0;

            float startX = initialSpawnPoint != null ? initialSpawnPoint.position.x : 0f;
            float traveledDistance = Mathf.Max(0f, maxPlayerX - startX);

            int calculatedDifficulty = (int)(traveledDistance / spawnConfig.DistanceIntervalForDifficulty);
            return Mathf.Clamp(calculatedDifficulty, 0, spawnConfig.MaxDifficultyLevel);
        }

        /// <summary>
        /// Trả về danh sách các chunk đang hoạt động (dùng cho debug hoặc kiểm tra ngoài).
        /// </summary>
        public List<ChunkInstance> GetActiveChunks() => activeChunks;
    }
}
