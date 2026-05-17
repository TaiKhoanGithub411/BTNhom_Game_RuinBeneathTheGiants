using System;
using UnityEngine;

/// <summary>
/// Lớp trừu tượng nền cho mọi nhân vật Player.
///
/// Mục tiêu của lớp này là gom các phần dùng chung của nhân vật:
/// - Sự kiện để hệ thống khác lắng nghe thay đổi
/// - Thuộc tính di chuyển và vật lý
/// - Trạng thái hiện tại của nhân vật
/// - Chỉ số sinh lực và thể lực
/// - Các hành động cơ bản mà lớp con phải tự định nghĩa
/// </summary>
public abstract class PlayerCharacterBase : MonoBehaviour
{
    // =========================
    // Events
    // =========================

    /// <summary>
    /// Phát ra khi trạng thái của nhân vật thay đổi.
    /// Ví dụ: Idle, Move, Jump, Exhausted, Dead.
    /// </summary>
    public event Action<PlayerState> OnStateChanged;

    /// <summary>
    /// Phát ra khi máu thay đổi để UI hoặc hệ thống khác cập nhật.
    /// </summary>
    public event Action<float> OnHealthChanged;

    /// <summary>
    /// Phát ra khi stamina thay đổi để đồng bộ với HUD hoặc logic game.
    /// </summary>
    public event Action<float> OnStaminaChanged;

    /// <summary>
    /// Phát ra khi nhân vật thực hiện một hành động đặc biệt.
    /// Dùng để báo hiệu cho animation, âm thanh hoặc combat system.
    /// </summary>
    public event Action<string> OnActionTriggered;

    // =========================
    // Movement & Physics
    // =========================

    /// <summary>
    /// Tốc độ di chuyển ngang của nhân vật.
    /// </summary>
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 5f;

    /// <summary>
    /// Lực nhảy của nhân vật.
    /// </summary>
    [SerializeField] protected float jumpForce = 12f;

    /// <summary>
    /// Tốc độ tăng gia tốc khi bắt đầu di chuyển.
    /// </summary>
    [SerializeField] protected float acceleration = 20f;

    /// <summary>
    /// Tốc độ giảm gia tốc khi ngừng di chuyển.
    /// </summary>
    [SerializeField] protected float deceleration = 25f;

    /// <summary>
    /// Rigidbody2D dùng để xử lý lực và va chạm vật lý.
    /// </summary>
    [Header("Physics")]
    [SerializeField] protected Rigidbody2D playerRigidbody;

    /// <summary>
    /// Collider chính của nhân vật.
    /// </summary>
    [SerializeField] protected Collider2D playerCollider;

    /// <summary>
    /// Điểm kiểm tra mặt đất khi nhảy hoặc rơi.
    /// </summary>
    [SerializeField] protected Transform groundCheck;

    /// <summary>
    /// Layer được xem là mặt đất.
    /// </summary>
    [SerializeField] protected LayerMask groundLayer;

    /// <summary>
    /// Bán kính kiểm tra va chạm ở vị trí groundCheck.
    /// </summary>
    [SerializeField] protected float groundCheckRadius = 0.15f;

    // =========================
    // State
    // =========================

    /// <summary>
    /// Trạng thái hiện tại của nhân vật.
    /// </summary>
    public PlayerState CurrentState { get; protected set; } = PlayerState.Idle;

    /// <summary>
    /// Cho biết nhân vật đang chạm đất hay không.
    /// </summary>
    public bool IsGrounded { get; protected set; }

    /// <summary>
    /// Cho biết nhân vật đang bị khóa hành động hay không.
    /// </summary>
    public bool IsLocked { get; protected set; }

    // =========================
    // Stats
    // =========================

    /// <summary>
    /// Máu tối đa của nhân vật.
    /// </summary>
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 100f;

    /// <summary>
    /// Máu hiện tại của nhân vật.
    /// </summary>
    [SerializeField] protected float currentHealth = 100f;

    /// <summary>
    /// Stamina tối đa của nhân vật.
    /// </summary>
    [SerializeField] protected float maxStamina = 100f;

    /// <summary>
    /// Stamina hiện tại của nhân vật.
    /// </summary>
    [SerializeField] protected float currentStamina = 100f;

    // =========================
    // Actions
    // =========================

    /// <summary>
    /// Di chuyển theo giá trị đầu vào.
    /// Lớp con sẽ quyết định cách áp dụng vào Rigidbody2D hoặc animation.
    /// </summary>
    public abstract void Move(float input);

    /// <summary>
    /// Thực hiện hành động nhảy.
    /// </summary>
    public abstract void Jump();

    /// <summary>
    /// Thực hiện hành động chính của nhân vật.
    /// Ví dụ: tấn công, tương tác, hoặc kích hoạt cơ chế đặc biệt.
    /// </summary>
    public abstract void PrimaryAction();

    /// <summary>
    /// Thực hiện hành động phụ của nhân vật.
    /// </summary>
    public abstract void SecondaryAction();

    /// <summary>
    /// Nhận sát thương và xử lý giảm máu.
    /// </summary>
    public abstract void TakeDamage(float damage);

    /// <summary>
    /// Hồi máu cho nhân vật.
    /// </summary>
    public abstract void Heal(float amount);

    /// <summary>
    /// Tiêu hao stamina khi thực hiện các hành động tốn thể lực.
    /// </summary>
    public abstract void ConsumeStamina(float amount);

    /// <summary>
    /// Hồi phục stamina theo thời gian hoặc từ vật phẩm.
    /// </summary>
    public abstract void RestoreStamina(float amount);

    // =========================
    // Unity Lifecycle
    // =========================

    /// <summary>
    /// Tự động lấy các thành phần cần thiết nếu chưa được gán trong Inspector.
    /// </summary>
    protected virtual void Awake()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }
    }

    /// <summary>
    /// Cập nhật trạng thái chạm đất ở mỗi frame vật lý.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        UpdateGroundedState();
    }

    /// <summary>
    /// Kiểm tra nhân vật có đang chạm đất hay không.
    /// </summary>
    protected virtual void UpdateGroundedState()
    {
        if (groundCheck == null)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // =========================
    // Protected Helpers
    // =========================

    /// <summary>
    /// Cập nhật trạng thái hiện tại và phát sự kiện khi có thay đổi.
    /// </summary>
    protected void SetState(PlayerState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }

    /// <summary>
    /// Thông báo cho hệ thống bên ngoài rằng máu đã thay đổi.
    /// </summary>
    protected void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Thông báo cho hệ thống bên ngoài rằng stamina đã thay đổi.
    /// </summary>
    protected void NotifyStaminaChanged()
    {
        OnStaminaChanged?.Invoke(currentStamina);
    }

    /// <summary>
    /// Phát sự kiện khi nhân vật kích hoạt một hành động.
    /// </summary>
    protected void NotifyActionTriggered(string actionName)
    {
        OnActionTriggered?.Invoke(actionName);
    }
}

/// <summary>
/// Danh sách trạng thái cơ bản của Player.
/// Tách riêng enum này giúp các lớp con và hệ thống khác dùng chung dễ hơn.
/// </summary>
public enum PlayerState
{
    Idle,
    Move,
    Jump,
    Exhausted,
    Dead
}