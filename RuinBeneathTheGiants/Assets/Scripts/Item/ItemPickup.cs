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

    private void Awake()
    {
        spawnTime = Time.time;
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
