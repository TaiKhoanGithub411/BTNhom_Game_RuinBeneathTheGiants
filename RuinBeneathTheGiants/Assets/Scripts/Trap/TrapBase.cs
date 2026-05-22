using UnityEngine;

/// <summary>
/// Lớp cha trừu tượng cho toàn bộ hệ thống bẫy (Trap) trong game.
/// Quản lý va chạm va chạm trigger, vòng đời tự hủy của bẫy và tự động gán Sprite.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class TrapBase : MonoBehaviour
{
    [Header("Data Config")]
    [SerializeField] protected TrapData trapData;

    protected float spawnTime;
    protected bool isTriggered;

    protected virtual void Awake()
    {
        spawnTime = Time.time;
    }

    protected virtual void Start()
    {
        // Nếu trapData được gán sẵn qua Inspector, tự động gán Sprite khi bắt đầu game
        if (trapData != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = trapData.ActiveSprite;
            }
        }
    }

    protected virtual void Update()
    {
        if (isTriggered || trapData == null) return;

        // Tự động hủy bẫy sau một khoảng thời gian nếu không được kích hoạt (tránh rác bộ nhớ)
        if (trapData.Lifetime > 0f && Time.time - spawnTime >= trapData.Lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Hàm khởi tạo động cho bẫy khi sinh ra bằng code (Spawner).
    /// </summary>
    /// <param name="data">Dữ liệu bẫy cần cấu hình</param>
    public virtual void Initialize(TrapData data)
    {
        this.trapData = data;
        this.spawnTime = Time.time; // Reset thời gian đếm ngược tự hủy từ lúc spawn thực tế

        // Tự động gán Sprite từ dữ liệu mới
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data != null)
        {
            spriteRenderer.sprite = data.ActiveSprite;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu bẫy đã kích hoạt hoặc không có dữ liệu, bỏ qua va chạm
        if (isTriggered || trapData == null) return;

        // Kiểm tra xem đối tượng va chạm có chỉ số sinh tồn (PlayerVitals) hay không
        PlayerVitals playerVitals = other.GetComponentInParent<PlayerVitals>();
        if (playerVitals == null || playerVitals.IsDead) return;

        // Gọi logic kích hoạt bẫy cụ thể ở lớp con
        OnPlayerTrigger(playerVitals);
    }

    /// <summary>
    /// Phương thức trừu tượng buộc các lớp con phải tự triển khai logic sát thương riêng biệt.
    /// </summary>
    /// <param name="playerVitals">Component sinh tồn của Player</param>
    protected abstract void OnPlayerTrigger(PlayerVitals playerVitals);
}
