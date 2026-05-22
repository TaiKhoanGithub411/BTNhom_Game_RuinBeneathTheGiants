using UnityEngine;

/// <summary>
/// Script đơn giản giúp Camera bám theo nhân vật một cách mượt mà.
/// Gắn script này vào Main Camera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("Vật thể mà Camera sẽ bám theo (Kéo Player vào đây)")]
    [SerializeField] private Transform target;
    
    [Tooltip("Độ lệch của Camera so với nhân vật (Z luôn phải là số âm như -10 để nhìn thấy game 2D)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);
    
    [Tooltip("Độ mượt khi bám theo. Số càng nhỏ càng mượt nhưng bám càng chậm.")]
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        // Nếu quên kéo Player vào ô Target, code sẽ tự động đi tìm thằng nào mang Tag "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            return;
        }

        // Tính toán vị trí đích đến
        Vector3 desiredPosition = target.position + offset;
        
        // Dùng hàm Lerp để Camera bay đến vị trí đích một cách mượt mà thay vì giật cục
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        transform.position = smoothedPosition;
    }
}
