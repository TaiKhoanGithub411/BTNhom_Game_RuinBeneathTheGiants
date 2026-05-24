using System.Collections;
using UnityEngine;

/// <summary>
/// Lớp cụ thể triển khai logic cho Bẫy Gấu (Bear Trap).
/// Gây khóa di chuyển ngắn (1.5s), hiệu ứng rỉ máu và giảm thể lực (10s), và mở lại sau 3s.
/// </summary>
public class BearTrap : TrapBase
{
    protected override void OnPlayerTrigger(PlayerVitals playerVitals)
    {
        // 1. Đánh dấu bẫy đã sập để vô hiệu hóa va chạm tiếp theo
        isTriggered = true;

        // 2. Ép Player đứng yên chịu đòn (1.5s) và kích hoạt Hurt animation
        playerVitals.TakeTrapImpact(1.5f);

        // 3. Gây sát thương từ từ (30% Max HP = 0.3 * MaxHealth) và giảm 20% hồi Stamina trong 10 giây
        StatusEffectManager statusEffectManager = playerVitals.GetComponent<StatusEffectManager>();
        if (statusEffectManager != null)
        {
            float bleedDamage = playerVitals.MaxHealth * 0.3f;
            statusEffectManager.ApplyBleedAndCripple(bleedDamage, 10f, 0.8f);
        }

        // 4. Kích hoạt hoạt ảnh kẹp của bẫy
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsSprung", true);
        }
        else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && trapData.TriggeredSprite != null)
            {
                spriteRenderer.sprite = trapData.TriggeredSprite;
            }
        }

        // 5. Bẫy gấu mở lại sau 3 giây để chờ nạn nhân tiếp theo
        StartCoroutine(ResetTrapRoutine());
    }

    private IEnumerator ResetTrapRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        isTriggered = false; // Bật lại hệ thống va chạm
        
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsSprung", false); // Mở bẫy ra
        }
        else
        {
            // Dự phòng nếu xài Sprite tĩnh (chuyển về ảnh ban đầu)
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && trapData.ActiveSprite != null)
            {
                spriteRenderer.sprite = trapData.ActiveSprite;
            }
        }
    }
}
