using UnityEngine;
using System.Collections.Generic;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Cơ sở dữ liệu quản lý toàn bộ các Chunk có sẵn trong game.
    /// Hỗ trợ tìm kiếm và chọn ngẫu nhiên Chunk phù hợp với độ khó hiện tại.
    /// </summary>
    [CreateAssetMenu(menuName = "BTNhom/MapGen/Chunk Database", fileName = "ChunkDatabase")]
    public class ChunkDatabase : ScriptableObject
    {
        [SerializeField] private List<ChunkData> chunks = new List<ChunkData>();

        public List<ChunkData> Chunks => chunks;

        /// <summary>
        /// Lấy một Chunk ngẫu nhiên phù hợp với độ khó (difficulty) hiện tại của game.
        /// </summary>
        public ChunkData GetRandomChunk(int currentDifficulty)
        {
            if (chunks == null || chunks.Count == 0) return null;

            List<ChunkData> eligibleChunks = new List<ChunkData>();

            foreach (var chunk in chunks)
            {
                if (chunk == null || chunk.Prefab == null) continue;

                // Kiểm tra xem chunk này có khớp với độ khó hiện tại hay không
                bool isEasyEnough = currentDifficulty >= chunk.MinDifficulty;
                bool isNotTooHard = chunk.MaxDifficulty == 0 || currentDifficulty <= chunk.MaxDifficulty;

                if (isEasyEnough && isNotTooHard)
                {
                    eligibleChunks.Add(chunk);
                }
            }

            // Fallback nếu không tìm thấy chunk nào phù hợp với độ khó hiện tại
            if (eligibleChunks.Count == 0)
            {
                // Trả về bất kỳ chunk nào có độ khó thấp nhất để tránh crash game
                ChunkData easiestChunk = null;
                int minDiffFound = int.MaxValue;
                foreach (var chunk in chunks)
                {
                    if (chunk != null && chunk.MinDifficulty < minDiffFound)
                    {
                        minDiffFound = chunk.MinDifficulty;
                        easiestChunk = chunk;
                    }
                }
                return easiestChunk;
            }

            // Chọn ngẫu nhiên trong danh sách các chunk hợp lệ
            int randomIndex = Random.Range(0, eligibleChunks.Count);
            return eligibleChunks[randomIndex];
        }
    }
}
