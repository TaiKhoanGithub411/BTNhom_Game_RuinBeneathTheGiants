using UnityEngine;

/// <summary>
/// Lớp cụ thể triển khai logic cho Bẫy Gấu (Bear Trap).
/// Bẫy hoạt động theo cơ chế gây sát thương tức thời 1 lần khi chạm, đổi hình ảnh sập bẫy và tự hủy sau 1.5 giây.
/// </summary>
public class BearTrap : TrapBase
{
    protected override void OnPlayerTrigger(PlayerVitals playerVitals)
    {
        // 1. Đánh dấu bẫy đã sập để vô hiệu hóa va chạm tiếp theo
        isTriggered = true;

        // 2. Gây sát thương tức thời lên chỉ số máu của người chơi
        playerVitals.TakeDamage(trapData.Damage);

        // 3. Xử lý hiển thị trạng thái sập bẫy (Ưu tiên Animation -> Dự phòng Sprite tĩnh)
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

        // 4. Bẫy gấu sập sẽ biến mất sau 1.5 giây để giải phóng bộ nhớ
        Invoke(nameof(DestroyTrap), 1.5f);
    }

    private void DestroyTrap()
    {
        Destroy(gameObject);
    }
}
