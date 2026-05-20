using UnityEngine;

/// <summary>
/// Lớp chịu trách nhiệm ĐỘC QUYỀN về Vật Lý:
/// - Rigidbody2D, Lực đẩy, Lực nhảy, Check Ground.
/// - Tuyệt đối không quan tâm máu me hay phím bấm.
/// </summary>
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Physics Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    public bool IsGrounded { get; private set; }
    
    private float moveInput;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 2 Việc luôn làm trong hàm Vật Lý
        CheckGrounded();
        ApplyMovement();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Nhận đầu vào di chuyển từ Controller
    /// </summary>
    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);

        // Quay mặt nhân vật (Flip Sprite)
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveInput > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Tính toán gia tốc và áp dụng vào Rigidbody2D
    /// </summary>
    private void ApplyMovement()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * rate * Time.fixedDeltaTime;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (!IsGrounded) return;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Các hàm phụ để PlayerController lấy thông tin truyền sang Animator
    public float GetCurrentSpeed() => Mathf.Abs(rb.linearVelocity.x);
    public float GetVerticalVelocity() => rb.linearVelocity.y;
}
