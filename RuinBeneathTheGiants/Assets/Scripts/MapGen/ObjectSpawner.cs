using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Lớp tiện ích chịu trách nhiệm sinh ngẫu nhiên vật phẩm (Item) và bẫy (Trap) lên các SpawnPoint của Chunk.
    /// Toàn bộ các đối tượng được sinh ra sẽ làm con của Chunk để dễ dàng quản lý vòng đời (tự động hủy cùng Chunk).
    /// </summary>
    public static class ObjectSpawner
    {
        /// <summary>
        /// Điền đầy (populate) các vật thể ngẫu nhiên vào các điểm spawn của một chunk dựa trên độ khó.
        /// </summary>
        public static void PopulateChunk(ChunkInstance chunk, SpawnConfig config, ItemDatabase itemsDb, TrapDatabase trapsDb, BossDatabase bossDb, int currentDifficulty)
        {
            if (chunk == null) return;
            if (chunk.IsPopulated) return;

            // Tìm tất cả các SpawnPoint thuộc chunk này
            SpawnPoint[] spawnPoints = chunk.GetComponentsInChildren<SpawnPoint>();
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                chunk.IsPopulated = true;
                return;
            }

            // Tính toán tỉ lệ xuất hiện thực tế dựa theo độ khó hiện tại
            float adjustedItemRate = config.GetAdjustedItemSpawnRate(currentDifficulty);
            float adjustedTrapRate = config.GetAdjustedTrapSpawnRate(currentDifficulty);
            float adjustedBossRate = config.GetAdjustedBossSpawnRate(currentDifficulty);

            foreach (var sp in spawnPoints)
            {
                if (sp == null) continue;

                // Xác suất roll: Tỉ lệ điều chỉnh theo độ khó * Xác suất riêng của điểm spawn đó
                float finalChance = sp.SpawnChance;

                switch (sp.SpawnType)
                {
                    case SpawnPointType.Item:
                        if (Random.value < (adjustedItemRate * finalChance))
                        {
                            SpawnItem(sp.transform.position, chunk.transform, itemsDb);
                        }
                        break;

                    case SpawnPointType.Trap:
                        if (Random.value < (adjustedTrapRate * finalChance))
                        {
                            SpawnTrap(sp.transform.position, chunk.transform, trapsDb);
                        }
                        break;

                    case SpawnPointType.Boss:
                        if (Random.value < (adjustedBossRate * finalChance))
                        {
                            SpawnBossZone(sp.transform.position, chunk.transform, bossDb);
                        }
                        break;
                }
            }

            // Đánh dấu chunk đã được điền đầy vật phẩm để tránh sinh lại khi Player quay lui
            chunk.IsPopulated = true;
        }

        /// <summary>
        /// Thực hiện sinh vật phẩm làm con của Chunk.
        /// </summary>
        private static void SpawnItem(Vector3 position, Transform parent, ItemDatabase db)
        {
            if (db == null) return;

            var entry = db.GetRandomItemEntry();
            if (entry == null || entry.Value.prefab == null) return;

            // Sinh prefab làm con của chunk
            GameObject spawnedObj = Object.Instantiate(entry.Value.prefab, position, Quaternion.identity, parent);
            
            // Khởi tạo dữ liệu động
            ItemPickup pickup = spawnedObj.GetComponent<ItemPickup>();
            if (pickup != null && entry.Value.data != null)
            {
                pickup.Initialize(entry.Value.data);
            }
        }

        /// <summary>
        /// Thực hiện sinh bẫy làm con của Chunk.
        /// </summary>
        private static void SpawnTrap(Vector3 position, Transform parent, TrapDatabase db)
        {
            if (db == null) return;

            var entry = db.GetRandomTrap();
            if (entry == null || entry.Value.prefab == null) return;

            // Sinh prefab làm con của chunk
            GameObject spawnedObj = Object.Instantiate(entry.Value.prefab, position, Quaternion.identity, parent);

            // Khởi tạo dữ liệu động
            TrapBase trap = spawnedObj.GetComponent<TrapBase>();
            if (trap != null && entry.Value.data != null)
            {
                trap.Initialize(entry.Value.data);
            }
        }

        /// <summary>
        /// Thực hiện sinh Vùng kích hoạt Boss (BossZone) làm con của Chunk.
        /// </summary>
        private static void SpawnBossZone(Vector3 position, Transform parent, BossDatabase db)
        {
            if (db == null) return;

            var entry = db.GetRandomBossZone();
            if (entry == null || entry.Value.prefab == null) return;

            // Sinh prefab Zone làm con của chunk
            Object.Instantiate(entry.Value.prefab, position, Quaternion.identity, parent);
        }
    }
}
