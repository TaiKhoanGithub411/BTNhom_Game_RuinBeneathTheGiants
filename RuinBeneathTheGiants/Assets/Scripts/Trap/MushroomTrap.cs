using UnityEngine;

/// <summary>
/// Lớp cụ thể triển khai logic cho Bẫy Nấm Độc (Mushroom Trap).
/// Bẫy hoạt động theo cơ chế gây sát thương tức thời khi chạm, 
/// sau đó giải phóng bào tử gây sát thương độc rút máu từ từ qua StatusEffectManager.
/// </summary>
public class MushroomTrap : TrapBase
{
    protected override void OnPlayerTrigger(PlayerVitals playerVitals)
    {
        // 1. Đánh dấu bẫy đã sập để tránh kích hoạt va chạm tiếp theo
        isTriggered = true;

        // 2. Ép kiểu dữ liệu cấu hình sang MushroomTrapData để lấy thông số độc tố
        MushroomTrapData mushroomData = trapData as MushroomTrapData;

        // 3. Gây sát thương tức thời lên người chơi (sử dụng thuộc tính sát thương cơ bản từ lớp cha)
        if (trapData != null && trapData.Damage > 0f)
        {
            playerVitals.TakeDamage(trapData.Damage);
        }

        // 4. Gây sát thương độc kéo dài theo thời gian (Damage over Time) qua StatusEffectManager
        if (mushroomData != null && mushroomData.DotDamage > 0f && mushroomData.DotDuration > 0f)
        {
            StatusEffectManager statusEffectManager = playerVitals.GetComponent<StatusEffectManager>();
            if (statusEffectManager != null)
            {
                // Áp dụng hiệu ứng ngộ độc (Poison) không bị giữ chân tại chỗ
                statusEffectManager.ApplyPoison(mushroomData.DotDamage, mushroomData.DotDuration);
            }
        }

        // 5. Xử lý hiển thị trạng thái kích hoạt bẫy (Ưu tiên Animation -> Dự phòng Sprite tĩnh)
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Trigger");
        }
        else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && trapData.TriggeredSprite != null)
            {
                spriteRenderer.sprite = trapData.TriggeredSprite;
            }
        }

        // 6. Nấm nổ sẽ tự hủy sau 1.5 giây để giải phóng bộ nhớ (đủ thời gian chạy animation/hiệu ứng)
        Invoke(nameof(DestroyTrap), 1.5f);
    }

    private void DestroyTrap()
    {
        Destroy(gameObject);
    }
}
