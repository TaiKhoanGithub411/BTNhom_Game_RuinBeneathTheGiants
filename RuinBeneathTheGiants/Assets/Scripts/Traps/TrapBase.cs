using UnityEngine;
using System.Collections;

/// <summary>
/// Lớp cơ bản cho các loại bẫy trong game.
/// Thiết kế Data-Driven: Có thể cấu hình sát thương và thời gian trực tiếp trên Inspector.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class TrapBase : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("Tổng lượng máu bị mất khi dẫm phải bẫy (ví dụ: 30)")]
    [SerializeField] private float totalDamage = 30f;
    
    [Tooltip("Thời gian bị kẹp chặt tại chỗ và mất máu (ví dụ: 2 giây)")]
    [SerializeField] private float trapDuration = 2f;
    
    [Tooltip("Thời gian bẫy tự động mở ra lại sau khi nhả con mồi (ví dụ: 3 giây)")]
    [SerializeField] private float resetTime = 3f;

    private Animator animator;
    private BoxCollider2D trapCollider;
    private bool isTriggered = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        trapCollider = GetComponent<BoxCollider2D>();
        trapCollider.isTrigger = true; // Đảm bảo luôn là Trigger
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return; // Nếu bẫy đang sập rồi thì bỏ qua

        if (other.CompareTag("Player"))
        {
            PlayerVitals vitals = other.GetComponent<PlayerVitals>();
            if (vitals != null && !vitals.IsDead)
            {
                StartCoroutine(TrapSequence(vitals));
            }
        }
    }

    private IEnumerator TrapSequence(PlayerVitals playerVitals)
    {
        isTriggered = true;

        // 1. Kích hoạt hoạt ảnh sập bẫy và giữ bẫy ở trạng thái ĐÓNG
        if (animator != null)
        {
            animator.SetBool("IsSprung", true);
        }

        // 2. Kẹp Player và trừ máu từ từ
        playerVitals.TakeDamageOverTime(totalDamage, trapDuration);

        // 3. Chờ cho đến khi thả Player ra
        yield return new WaitForSeconds(trapDuration);

        // 4. Bắt đầu thời gian Reset bẫy (Chờ thêm vài giây rồi mới mở ra)
        yield return new WaitForSeconds(resetTime);

        // 5. Mở bẫy ra lại (Báo cho Animator biết để chuyển về hoạt ảnh Idle mở mỏ)
        if (animator != null)
        {
            animator.SetBool("IsSprung", false);
        }
        isTriggered = false;
    }
}
