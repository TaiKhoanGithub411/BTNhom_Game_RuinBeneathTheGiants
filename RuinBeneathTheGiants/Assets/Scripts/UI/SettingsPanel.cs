using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelr : MonoBehaviour
{
    [Header("UI Elements Root")]
    public GameObject panelRoot;      // Khung chứa bảng Settings vuông
    public GameObject overlay;        // Tấm nền đen mờ che toàn màn hình

    [Header("Controls")]
    public Button closeButton;        // Nút X để đóng
    public Slider volumeSlider;        // Thanh trượt âm lượng

    private const string VOLUME_KEY = "volume_master";

    private void Awake()
    {
        // 1. Tự động đăng ký sự kiện cho nút X bằng code (không cần kéo thả trong Inspector)
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        // 2. Tự động đăng ký sự kiện cho Slider khi người chơi kéo
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }

        // 3. Đọc lại cấu hình âm lượng cũ mà người chơi đã lưu từ trước
        LoadVolumeSettings();
    }

    // Hàm dùng để mở bảng Settings
    public void Show()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        if (overlay != null) overlay.SetActive(true);
    }

    // Hàm dùng để đóng bảng Settings
    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (overlay != null) overlay.SetActive(false);
    }

    // Hàm xử lý thay đổi âm lượng và lưu lại vào máy
    private void ApplyVolume(float value)
    {
        AudioListener.volume = value; // Thay đổi âm lượng tổng của hệ thống Unity
        PlayerPrefs.SetFloat(VOLUME_KEY, value); // Ghi nhớ vào "sổ tay" PlayerPrefs
        PlayerPrefs.Save(); // Lưu lại thiết lập vào ổ cứng
    }

    // Hàm đọc dữ liệu cũ khi vừa mở game
    private void LoadVolumeSettings()
    {
        // Nếu là lần đầu mở game (chưa có dữ liệu), mặc định âm lượng là tối đa (1.0f)
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1.0f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        AudioListener.volume = savedVolume;
    }
}