using UnityEngine;

/// <summary>
/// Lớp chịu trách nhiệm xử lý vật lý cho nhân vật Player.
///
/// Nhiệm vụ của lớp này là:
/// - Áp dụng di chuyển ngang lên Rigidbody2D
/// - Thực hiện nhảy
/// - Cập nhật trạng thái chạm đất
/// - Không xử lý input trực tiếp
/// </summary>
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCharacterBase playerCharacter;

    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Movement")]
    [SerializeField] private float movementSmoothTime = 0.08f;

    private float horizontalVelocity;
    private float velocityXRef;

    /// <summary>
    /// Tự động lấy reference cần thiết nếu chưa gán trong Inspector.
    /// </summary>
    private void Awake()
    {
        if (playerCharacter == null)
        {
            playerCharacter = GetComponent<PlayerCharacterBase>();
        }

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// Áp dụng vận tốc di chuyển ngang cho nhân vật.
    /// </summary>
    public void Move(float input, float moveSpeed)
    {
        if (playerRigidbody == null)
        {
            return;
        }

        horizontalVelocity = Mathf.SmoothDamp(
            playerRigidbody.linearVelocity.x,
            input * moveSpeed,
            ref velocityXRef,
            movementSmoothTime
        );

        playerRigidbody.linearVelocity = new Vector2(horizontalVelocity, playerRigidbody.linearVelocity.y);
    }

    /// <summary>
    /// Thực hiện lực nhảy nếu nhân vật đang chạm đất.
    /// </summary>
    public void Jump(float jumpForce)
    {
        if (playerRigidbody == null)
        {
            return;
        }

        if (playerCharacter != null && !playerCharacter.IsGrounded)
        {
            return;
        }

        Vector2 velocity = playerRigidbody.linearVelocity;
        velocity.y = 0f;
        playerRigidbody.linearVelocity = velocity;
        playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
