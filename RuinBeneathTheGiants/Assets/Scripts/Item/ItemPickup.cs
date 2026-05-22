using UnityEngine;

/// <summary>
/// Item hiện diện trong world. Khi Player chạm vào sẽ tự động kích hoạt
/// toàn bộ hiệu ứng rồi biến mất.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemData itemData;

    private float spawnTime;
    private bool isConsumed;
    
    public void Initialize(ItemData data)
    {
        this.itemData = data;
        this.spawnTime = Time.time;

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
        StatusEffectManager statusEffects = other.GetComponentInParent<StatusEffectManager>();
        
        if (playerVitals == null) return;

        ApplyEffects(playerVitals, statusEffects);
        Consume();
    }

    private void ApplyEffects(PlayerVitals vitals, StatusEffectManager status)
    {
        if (itemData.Effects == null) return;

        foreach (var effect in itemData.Effects)
        {
            switch (effect.EffectType)
            {
                case ItemEffectType.HealthRestore:
                    vitals.Heal(effect.Value);
                    break;
                case ItemEffectType.StaminaRestore:
                    vitals.RestoreStamina(effect.Value);
                    break;
                case ItemEffectType.PoisonOverTime:
                    if (status != null) status.ApplyPoison(effect.Value, effect.Duration);
                    break;
                case ItemEffectType.PoisonImmunity:
                    if (status != null) status.ApplyPoisonImmunity(effect.Duration);
                    break;
                case ItemEffectType.TrapResistance:
                    if (status != null) status.ApplyTrapResistance(effect.Duration);
                    break;
            }
        }
    }

    private void Consume()
    {
        isConsumed = true;
        Destroy(gameObject);
    }
}

