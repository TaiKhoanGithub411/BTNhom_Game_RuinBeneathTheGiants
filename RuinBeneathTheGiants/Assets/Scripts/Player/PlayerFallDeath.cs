using UnityEngine;

/// <summary>
/// Chết khi rơi vực: không thấy ground phía dưới trong một khoảng thời gian.
/// Dùng cùng groundLayer với PlayerMotor.
/// </summary>
[RequireComponent(typeof(PlayerVitals), typeof(PlayerMotor))]
public class PlayerFallDeath : MonoBehaviour
{
    [Header("Ground check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundProbeDistance = 6f;   // Tầm nhìn xuống (đơn vị Unity)
    [SerializeField] private Vector2 probeOriginOffset = new Vector2(0f, 0f); // Tâm player, hoặc chỉnh lên/xuống

    [Header("Fall death")]
    [SerializeField] private float fallDeathDelay = 0.4f;      // Giây không có đất → chết
    [SerializeField] private float minFallSpeed = 0.5f;      // Chỉ đếm khi đang rơi (vy < -minFallSpeed)

    [Header("Safety net")]
    [SerializeField] private float killY = -10f;             // Dự phòng, không cần -50

    private PlayerVitals vitals;
    private PlayerMotor motor;
    private Rigidbody2D rb;
    private float noGroundTimer;

    private void Awake()
    {
        vitals = GetComponent<PlayerVitals>();
        motor = GetComponent<PlayerMotor>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (vitals.IsDead)
            return;

        // Lưới an toàn: quá sâu vẫn chết
        if (transform.position.y < killY)
        {
            KillPlayer();
            return;
        }

        bool hasGroundBelow = HasGroundBelow();
        bool isFalling = rb != null && rb.linearVelocity.y < -minFallSpeed;

        // Có đất dưới chân (raycast) hoặc đang đứng trên đất → reset
        if (hasGroundBelow || motor.IsGrounded)
        {
            noGroundTimer = 0f;
            return;
        }

        // Đang rơi + không thấy đất trong tầm probe → đếm giờ
        if (isFalling)
        {
            noGroundTimer += Time.deltaTime;
            if (noGroundTimer >= fallDeathDelay)
                KillPlayer();
        }
        else
        {
            noGroundTimer = 0f;
        }
    }

    private bool HasGroundBelow()
    {
        Vector2 origin = (Vector2)transform.position + probeOriginOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundProbeDistance, groundLayer);
        return hit.collider != null;
    }

    private void KillPlayer()
    {
        // Gây chết qua hệ máu → OnDeath → GameOverPanel
        vitals.TakeDamage(vitals.MaxHealth);
    }

    // Gizmos để debug trong Scene (tia xuống màu vàng)
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + probeOriginOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector2.down * groundProbeDistance);
    }
}