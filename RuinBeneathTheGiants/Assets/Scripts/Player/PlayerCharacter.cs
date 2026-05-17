using UnityEngine;

/// <summary>
/// Lớp nhân vật cụ thể kế thừa từ PlayerCharacterBase.
/// Lớp này xử lý các trạng thái cơ bản và hành vi di chuyển/nhảy của nhân vật.
/// </summary>
public class PlayerCharacter : PlayerCharacterBase
{
    [Header("Runtime Movement")]
    [SerializeField] private float inputDeadZone = 0.01f;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string isMovingParameter = "IsMoving";

    private float moveInput;

    /// <summary>
    /// Lưu đầu vào di chuyển để áp dụng trong FixedUpdate.
    /// </summary>
    public override void Move(float input)
    {
        if (IsLocked || CurrentState == PlayerState.Dead)
        {
            moveInput = 0f;
            return;
        }

        moveInput = Mathf.Abs(input) < inputDeadZone ? 0f : Mathf.Clamp(input, -1f, 1f);

        if (moveInput != 0f)
        {
            UpdateFacingDirection(moveInput);
            UpdateMoveState(true);
        }
        else
        {
            UpdateMoveState(false);

            if (IsGrounded && CurrentState != PlayerState.Jump)
            {
                SetState(PlayerState.Idle);
            }
            return;
        }

        if (IsGrounded)
        {
            SetState(PlayerState.Move);
        }
    }

    /// <summary>
    /// Đổi hướng nhân vật theo hướng di chuyển để sprite luôn quay đúng chiều.
    /// </summary>
    private void UpdateFacingDirection(float input)
    {
        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);

        if (input > 0f)
        {
            scale.x = absX;
        }
        else if (input < 0f)
        {
            scale.x = -absX;
        }

        transform.localScale = scale;
    }

    /// <summary>
    /// Cập nhật trạng thái animation giữa đứng yên và chạy dựa trên input di chuyển.
    /// </summary>
    private void UpdateMoveState(bool isMoving)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(isMovingParameter, isMoving);
        }
    }

    /// <summary>
    /// Thực hiện nhảy nếu đang đứng trên mặt đất và không bị khóa.
    /// </summary>
    public override void Jump()
    {
        if (IsLocked || CurrentState == PlayerState.Dead || !IsGrounded || CurrentState == PlayerState.Jump)
        {
            return;
        }

        if (playerRigidbody == null)
        {
            return;
        }

        playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, jumpForce);
        SetState(PlayerState.Jump);
        NotifyActionTriggered(nameof(Jump));
    }

    /// <summary>
    /// Hành động chính của nhân vật, hiện được dùng làm chỗ mở rộng cho tấn công/tương tác.
    /// </summary>
    public override void PrimaryAction()
    {
        if (IsLocked || CurrentState == PlayerState.Dead)
        {
            return;
        }

        NotifyActionTriggered(nameof(PrimaryAction));
    }

    /// <summary>
    /// Hành động phụ của nhân vật, dùng cho các cơ chế mở rộng sau này.
    /// </summary>
    public override void SecondaryAction()
    {
        if (IsLocked || CurrentState == PlayerState.Dead)
        {
            return;
        }

        NotifyActionTriggered(nameof(SecondaryAction));
    }

    /// <summary>
    /// Nhận sát thương và chuyển sang trạng thái Dead nếu máu về 0.
    /// </summary>
    public override void TakeDamage(float damage)
    {
        if (damage <= 0f || CurrentState == PlayerState.Dead)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        NotifyHealthChanged();

        if (currentHealth <= 0f)
        {
            SetState(PlayerState.Dead);
            moveInput = 0f;
        }
    }

    /// <summary>
    /// Hồi máu cho nhân vật, không vượt quá giới hạn tối đa.
    /// </summary>
    public override void Heal(float amount)
    {
        if (amount <= 0f || CurrentState == PlayerState.Dead)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        NotifyHealthChanged();
    }

    /// <summary>
    /// Tiêu hao stamina và cập nhật trạng thái Exhausted nếu bị cạn thể lực.
    /// </summary>
    public override void ConsumeStamina(float amount)
    {
        if (amount <= 0f || CurrentState == PlayerState.Dead)
        {
            return;
        }

        currentStamina = Mathf.Max(0f, currentStamina - amount);
        NotifyStaminaChanged();

        if (currentStamina <= 0f && IsGrounded)
        {
            SetState(PlayerState.Exhausted);
        }
    }

    /// <summary>
    /// Hồi stamina và đưa nhân vật về trạng thái phù hợp khi đã có thể lực trở lại.
    /// </summary>
    public override void RestoreStamina(float amount)
    {
        if (amount <= 0f || CurrentState == PlayerState.Dead)
        {
            return;
        }

        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        NotifyStaminaChanged();

        if (currentStamina > 0f && IsGrounded && CurrentState == PlayerState.Exhausted)
        {
            SetState(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Áp dụng vận tốc di chuyển theo input đã lưu.
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerRigidbody == null || IsLocked || CurrentState == PlayerState.Dead)
        {
            return;
        }

        float targetSpeed = moveInput * moveSpeed;
        float velocityX = playerRigidbody.linearVelocity.x;
        float speedDifference = targetSpeed - velocityX;
        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDifference * rate * Time.fixedDeltaTime;

        playerRigidbody.linearVelocity = new Vector2(velocityX + movement, playerRigidbody.linearVelocity.y);

        if (IsGrounded && Mathf.Abs(moveInput) < inputDeadZone)
        {
            SetState(CurrentState == PlayerState.Exhausted ? PlayerState.Exhausted : PlayerState.Idle);
        }
    }

    /// <summary>
    /// Cập nhật thêm trạng thái khi đang ở trên không hoặc chạm đất.
    /// </summary>
    protected override void UpdateGroundedState()
    {
        bool wasGrounded = IsGrounded;
        base.UpdateGroundedState();

        if (!wasGrounded && IsGrounded && CurrentState == PlayerState.Jump)
        {
            SetState(Mathf.Abs(moveInput) > inputDeadZone ? PlayerState.Move : PlayerState.Idle);
        }
    }
}
