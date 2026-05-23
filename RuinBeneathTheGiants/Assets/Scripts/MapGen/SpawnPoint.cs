using UnityEngine;

namespace BTNhom.MapGen
{
    /// <summary>
    /// Component dùng để đánh dấu điểm spawn vật thể (Item, Trap, Boss) bên trong các Prefab Chunk.
    /// Script này chỉ chứa dữ liệu cấu hình và vẽ Gizmo trực quan trong Unity Editor.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Loại đối tượng sẽ sinh ra tại điểm này")]
        [SerializeField] private SpawnPointType spawnType;

        [Tooltip("Xác suất sinh vật thể tại điểm này (từ 0.0 đến 1.0)")]
        [SerializeField, Range(0f, 1f)] private float spawnChance = 1f;

        // Các thuộc tính Getter để truy xuất an toàn từ ObjectSpawner
        public SpawnPointType SpawnType => spawnType;
        public float SpawnChance => spawnChance;

        /// <summary>
        /// Vẽ Gizmo trực quan trong Unity Editor để dễ thiết kế màn chơi.
        /// </summary>
        private void OnDrawGizmos()
        {
            Color gizmoColor = Color.white;

            switch (spawnType)
            {
                case SpawnPointType.Item:
                    gizmoColor = Color.green; // Màu xanh lá cho vật phẩm
                    break;
                case SpawnPointType.Trap:
                    gizmoColor = Color.red; // Màu đỏ cho bẫy
                    break;
                case SpawnPointType.Boss:
                    gizmoColor = new Color(0.5f, 0f, 0.5f); // Màu tím cho Boss
                    break;
                case SpawnPointType.Decoration:
                    gizmoColor = Color.cyan; // Màu xanh lam cho trang trí
                    break;
            }

            // Vẽ vòng tròn nhỏ tại vị trí spawn
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            
            // Vẽ biểu tượng mũi tên hoặc nhãn chữ đơn giản
            Gizmos.DrawLine(transform.position + Vector3.left * 0.2f, transform.position + Vector3.right * 0.2f);
            Gizmos.DrawLine(transform.position + Vector3.down * 0.2f, transform.position + Vector3.up * 0.2f);
        }
    }
}
