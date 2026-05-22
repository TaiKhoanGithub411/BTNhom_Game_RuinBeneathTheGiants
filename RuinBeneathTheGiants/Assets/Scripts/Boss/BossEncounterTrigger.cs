using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý từng khu vực kích hoạt Boss trên bản đồ.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BossEncounterTrigger : MonoBehaviour
{
    public enum ZoneType { Scare_Only, Rampage_Zone, Stop_And_Retract }

    [Header("Zone Settings")]
    [Tooltip("Chọn loại hoạt động cho khu vực này")]
    [SerializeField] private ZoneType zoneType;
    
    [Header("Spawner Settings (Cần điền cho Scare và Rampage)")]
    [SerializeField] private BossFootInstance bossFootPrefab;
    [SerializeField] private BossAttackData attackData;
    [SerializeField] private float spawnYOffset = 5f;
    [SerializeField] private float randomXRange = 3f; // Spawn ngẫu nhiên quanh Player

    // Dùng static để tất cả các Zone cùng chia sẻ trạng thái này (đảm bảo không bị lặp đè)
    private static bool isRampaging = false; 
    private Coroutine rampageCoroutine;
    private Transform playerTransform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;

            if (zoneType == ZoneType.Scare_Only)
            {
                SpawnFoot(BossFootInstance.FootAction.Scare);
            }
            else if (zoneType == ZoneType.Rampage_Zone)
            {
                if (!isRampaging)
                {
                    isRampaging = true;
                    rampageCoroutine = StartCoroutine(RampageRoutine());
                }
            }
            else if (zoneType == ZoneType.Stop_And_Retract)
            {
                StopRampage();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tự động dừng nếu Player chạy ra khỏi vùng Rampage
        if (other.CompareTag("Player") && zoneType == ZoneType.Rampage_Zone)
        {
            StopRampage();
        }
    }

    private void StopRampage()
    {
        isRampaging = false;
        if (rampageCoroutine != null)
        {
            StopCoroutine(rampageCoroutine);
            rampageCoroutine = null;
        }
    }

    private IEnumerator RampageRoutine()
    {
        while (isRampaging)
        {
            SpawnFoot(BossFootInstance.FootAction.Stomp);
            
            // Chờ một khoảng thời gian random (Cooldown)
            float waitTime = attackData != null ? Random.Range(attackData.cooldownMin, attackData.cooldownMax) : 2f;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnFoot(BossFootInstance.FootAction action)
    {
        if (bossFootPrefab == null || playerTransform == null)
        {
            Debug.LogWarning("BossZone: Thiếu Prefab hoặc chưa nhận diện được Player!");
            return;
        }

        // Tính toán vị trí sinh ra bàn chân
        Vector3 spawnPos = playerTransform.position;
        spawnPos.y += spawnYOffset;
        
        // Random dịch sang trái/phải một chút để Player khó đoán
        spawnPos.x += Random.Range(-randomXRange, randomXRange);

        // Đúc ra một cái bàn chân mới từ khuôn
        BossFootInstance foot = Instantiate(bossFootPrefab, spawnPos, Quaternion.identity);
        
        // Khởi động bàn chân đó
        foot.Initialize(attackData, action);
    }
}
