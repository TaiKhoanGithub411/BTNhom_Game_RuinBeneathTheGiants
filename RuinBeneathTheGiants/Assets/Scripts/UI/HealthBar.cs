using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lớp quản lý giao diện Thanh Máu (Health Bar).
/// Lắng nghe sự kiện từ PlayerVitals để cập nhật UI.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage; 

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
            slider.maxValue = playerVitals.MaxHealth;
            slider.value = playerVitals.MaxHealth;
            
            playerVitals.OnHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.LogWarning("HealthBar: Không tìm thấy PlayerVitals!");
        }
    }

    private void OnDestroy()
    {
        // Nhớ gỡ sự kiện khi UI bị hủy
        if (playerVitals != null)
        {
            playerVitals.OnHealthChanged -= UpdateHealthBar;
        }
    }

    /// <summary>
    /// Hàm này tự động chạy mỗi khi có sự thay đổi Máu ở code PlayerVitals
    /// </summary>
    private void UpdateHealthBar(float currentHealth)
    {
        if (slider != null)
        {
            slider.value = currentHealth;
        }
    }
}
