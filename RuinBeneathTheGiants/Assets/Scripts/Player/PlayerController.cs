using UnityEngine;

/// <summary>
/// Lớp chịu trách nhiệm đọc Input của người chơi và điều phối các lệnh
/// sang các lớp hệ thống tương ứng (PlayerMotor cho vật lý, PlayerVitals cho máu/stamina).
///
/// Lưu ý: lớp này không phát âm thanh nữa. Âm thanh đã được tách hoàn toàn sang
/// `AudioManager` để giữ đúng nguyên tắc phân tách trách nhiệm.
/// </summary>
[RequireComponent(typeof(PlayerMotor), typeof(PlayerVitals), typeof(StatusEffectManager))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private PlayerVitals vitals;
    [SerializeField] private Animator animator;
    [SerializeField] private StatusEffectManager statusEffects;

    private void Awake()
    {
        // Cache lại component theo đúng quy ước.
        // Mỗi component xử lý một nhiệm vụ rõ ràng để code dễ đọc và dễ bảo trì.
        if (motor == null) motor = GetComponent<PlayerMotor>();
        if (vitals == null) vitals = GetComponent<PlayerVitals>();
        if (animator == null) animator = GetComponent<Animator>();
        if (statusEffects == null) statusEffects = GetComponent<StatusEffectManager>();

        // Đăng ký lắng nghe sự kiện mất máu từ Vitals.
        // Controller chỉ phản ứng về mặt animation, không can thiệp vào âm thanh.
        if (vitals != null)
        {
            vitals.OnTakeDamage += TriggerHurtAnimation;
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký khi script bị xóa để tránh lỗi bộ nhớ (Memory Leak).
        // Đây là bước quan trọng khi làm việc với event để tránh object cũ
        // vẫn bị giữ tham chiếu sau khi đã bị destroy.
        if (vitals != null)
        {
            vitals.OnTakeDamage -= TriggerHurtAnimation;
        }
    }

    private void TriggerHurtAnimation()
    {
        // Kích hoạt biến Trigger "Hurt" trong Animator.
        // Animation là phản ứng hình ảnh, còn âm thanh sẽ được AudioManager xử lý riêng.
        if (animator != null) animator.SetTrigger("Hurt");
    }

    private float idleTimer = 0f;

    private void Update()
    {
        // Luôn đồng bộ dữ liệu sang Animator trước (kể cả khi đã chết để nó biết mà chạy animation Dead).
        UpdateAnimator();

        // Nếu nhân vật chết thì không cho thao tác gì cả.
        if (vitals.IsDead)
        {
            motor.SetMoveInput(0f);
            return;
        }

        ReadMovementInput();
        ReadActionInput();
    }

    private void ReadMovementInput()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A)) horizontalInput -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput += 1f;

        // Nếu đang kiệt sức (Exhausted) hoặc bị kẹp bẫy, ép buộc dừng lại không cho di chuyển
        if (vitals.IsExhausted || vitals.IsTrapped)
        {
            horizontalInput = 0f;
        }

        // Xử lý chạy nhanh (Shift) và Stamina
        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(horizontalInput) > 0.1f;
        
        if (isTryingToRun && !vitals.IsExhausted)
        {
            motor.SetIsRunning(true);
            vitals.DrainStaminaContinuous();
        }
        else
        {
            motor.SetIsRunning(false);
            vitals.RegenStaminaContinuous();
        }

        // Tính toán thời gian đứng yên
        if (Mathf.Abs(horizontalInput) < 0.1f && motor.IsGrounded)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0f;
        }

        // Chuyển giá trị ngang sang Motor xử lý vật lý
        motor.SetMoveInput(horizontalInput);
    }

    private void ReadActionInput()
    {
        // Khóa hành động nhảy nếu đang kiệt sức hoặc bị kẹp bẫy
        if (vitals.IsExhausted || vitals.IsTrapped) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            // Ví dụ: Nhảy tốn 10 Stamina. Chỉ nhảy được khi còn Stamina.
            // Việc phát âm thanh nhảy, nếu có, nên do AudioManager nghe event phù hợp
            // hoặc do một event riêng cho hành động nhảy.
            if (vitals.ConsumeStamina(10f))
            {
                motor.Jump();
            }
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        // Cập nhật các biến cơ bản.
        animator.SetFloat("Speed", motor.GetCurrentSpeed());
        animator.SetBool("IsGrounded", motor.IsGrounded);
        animator.SetFloat("IdleTime", idleTimer); // Truyền thời gian đứng yên sang Animator
        
        // Cập nhật các biến trạng thái mới (Sinh tồn)
        animator.SetBool("IsDead", vitals.IsDead);
        animator.SetBool("IsExhausted", vitals.IsExhausted);
        animator.SetBool("IsTrapped", vitals.IsTrapped);
    }
}
