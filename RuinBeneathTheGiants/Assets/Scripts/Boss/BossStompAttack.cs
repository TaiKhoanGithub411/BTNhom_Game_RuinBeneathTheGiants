using UnityEngine;

/// <summary>
/// Hitbox của Bàn chân Boss.
/// Gắn script này vào một object con có Collider2D (IsTrigger = true) nằm dưới đế giày.
/// Nhớ dùng Animation Window (hoặc Animation Event) để bật/tắt cái Collider này đúng thời điểm đạp xuống.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BossStompAttack : MonoBehaviour
{
    private float stompDamage = 9999f; // Mặc định giết ngay

    public void SetDamage(float damage)
    {
        stompDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi bàn chân đạp trúng Player
        if (other.CompareTag("Player"))
        {
            PlayerVitals vitals = other.GetComponent<PlayerVitals>();
            if (vitals != null && !vitals.IsDead)
            {
                // Giết người chơi ngay lập tức
                vitals.TakeDamage(stompDamage);
                
                // Tùy chọn: Thêm hiệu ứng rung màn hình, bụi đất bay mù mịt ở đây
                Debug.Log("Boss: Bẹp! Đã đạp bẹp Player.");
            }
        }
    }
}
