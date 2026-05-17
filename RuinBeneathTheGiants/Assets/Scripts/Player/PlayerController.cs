using UnityEngine;

/// <summary>
/// Lớp đọc input của người chơi và chuyển lệnh sang `PlayerCharacter`.
///
/// Mục tiêu của lớp này là chỉ xử lý input, không xử lý vật lý hay chỉ số.
/// Nhờ đó code sạch hơn và tách rõ trách nhiệm:
/// - `PlayerController`: đọc input
/// - `PlayerCharacterBase` / `PlayerCharacter`: xử lý hành vi nhân vật
/// - `PlayerMotor`: xử lý vật lý nếu dự án muốn tách riêng
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCharacterBase playerCharacter;

    private void Awake()
    {
        if (playerCharacter == null)
        {
            playerCharacter = GetComponent<PlayerCharacterBase>();
        }
    }

    private void Update()
    {
        if (playerCharacter == null)
        {
            return;
        }

        if (playerCharacter.IsLocked || playerCharacter.CurrentState == PlayerState.Dead)
        {
            return;
        }

        ReadMovementInput();
        ReadActionInput();
    }

    /// <summary>
    /// Đọc input di chuyển ngang và chuyển cho lớp nhân vật xử lý.
    /// </summary>
    private void ReadMovementInput()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput += 1f;
        }

        playerCharacter.Move(horizontalInput);
    }

    /// <summary>
    /// Đọc input hành động cơ bản của nhân vật.
    /// </summary>
    private void ReadActionInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerCharacter.Jump();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            playerCharacter.PrimaryAction();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            playerCharacter.SecondaryAction();
        }
    }
}
