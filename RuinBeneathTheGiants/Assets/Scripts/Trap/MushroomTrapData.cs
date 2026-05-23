using UnityEngine;

/// <summary>
/// Dữ liệu cấu hình riêng cho bẫy nấm độc (Mushroom Trap).
/// Kế thừa từ TrapData để sử dụng lại các thuộc tính cơ bản (sát thương tức thời, sprite, v.v.)
/// và bổ sung thêm các thông số về sát thương độc theo thời gian (Damage over Time).
/// </summary>
[CreateAssetMenu(menuName = "BTNhom/Traps/Mushroom Trap Data", fileName = "NewMushroomTrapData")]
public class MushroomTrapData : TrapData
{
    [Header("Mushroom Toxic Settings")]
    [SerializeField, Min(0f)] private float dotDamage = 15f;      // Tổng sát thương độc gây ra theo thời gian
    [SerializeField, Min(0f)] private float dotDuration = 3f;     // Thời gian độc kéo dài (giây)

    // Các thuộc tính Getter để truy xuất dữ liệu an toàn từ bên ngoài
    public float DotDamage => dotDamage;
    public float DotDuration => dotDuration;
}
