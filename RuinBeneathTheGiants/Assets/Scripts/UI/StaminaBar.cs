using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lớp quản lý giao diện Thanh Thể lực (Stamina Bar).
/// Lắng nghe sự kiện từ PlayerVitals để cập nhật UI.
/// </summary>
public class StaminaBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage; // Dùng để đổi màu nếu cần (ví dụ lúc kiệt sức chuyển sang màu đỏ)

    [Header("References")]
    [SerializeField] private PlayerVitals playerVitals;

    private void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        
        // Tự động tìm PlayerVitals nếu chưa được gán trong Inspector
        if (playerVitals == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerVitals = player.GetComponent<PlayerVitals>();
            }
        }

        // Đăng ký sự kiện
        if (playerVitals != null)
        {
            slider.maxValue = playerVitals.MaxStamina;
            slider.value = playerVitals.MaxStamina;
            
            playerVitals.OnStaminaChanged += UpdateStaminaBar;
        }
        else
        {
            Debug.LogWarning("StaminaBar: Không tìm thấy PlayerVitals!");
        }
    }

    private void OnDestroy()
    {
        // Nhớ gỡ sự kiện khi UI bị hủy
        if (playerVitals != null)
        {
            playerVitals.OnStaminaChanged -= UpdateStaminaBar;
        }
    }

    /// <summary>
    /// Hàm này tự động chạy mỗi khi có sự thay đổi Stamina ở code PlayerVitals
    /// </summary>
    private void UpdateStaminaBar(float currentStamina)
    {
        if (slider != null)
        {
            slider.value = currentStamina;
        }
    }
}
