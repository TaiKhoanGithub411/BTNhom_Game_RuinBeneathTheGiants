using UnityEngine;

/// <summary>
/// Item hiện diện trong world. Khi Player chạm vào sẽ tự động kích hoạt
/// hiệu ứng rồi biến mất.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemData itemData;

    private PlayerVitals cachedPlayerVitals;
    private float spawnTime;
    private bool isConsumed;
    
    public void Initialize(ItemData data)
    {
        this.itemData = data;
        this.spawnTime = Time.time;// Tính lại thời gian tồn tại từ lúc được spawn ra

        // Tự động cập nhật hình ảnh của vật phẩm dựa trên dữ liệu mới
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data != null)
        {
            spriteRenderer.sprite = data.Icon;
        }
    }

    private void Awake()
    {
        spawnTime = Time.time;
    }

    private void Start()
    {
        // Nếu itemData đã được gán sẵn trong Inspector (không qua Initialize), tự động cập nhật hình ảnh khi bắt đầu game
        if (itemData != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = itemData.Icon;
            }
        }
    }

    private void Update()
    {
        if (isConsumed || itemData == null) return;

        if (itemData.Lifetime > 0f && Time.time - spawnTime >= itemData.Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConsumed || itemData == null) return;

        PlayerVitals playerVitals = other.GetComponentInParent<PlayerVitals>();
        if (playerVitals == null) return;

        ApplyEffect(playerVitals);
        Consume();
    }

    private void ApplyEffect(PlayerVitals playerVitals)
    {
        switch (itemData.EffectType)
        {
            case ItemEffectType.HealthRestore:
                playerVitals.Heal(itemData.Value);
                break;
            case ItemEffectType.StaminaRestore:
                playerVitals.RestoreStamina(itemData.Value);
                break;
        }
    }

    private void Consume()
    {
        isConsumed = true;
        Destroy(gameObject);
    }
}
