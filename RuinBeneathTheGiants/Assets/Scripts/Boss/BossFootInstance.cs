using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý vòng đời của một Bàn Chân duy nhất.
/// Gắn script này vào Prefab của BossFoot.
/// </summary>
public class BossFootInstance : MonoBehaviour
{
    public enum FootAction { Scare, Stomp }

    [SerializeField] private Animator animator;
    [SerializeField] private BossStompAttack stompLogic;

    public void Initialize(BossAttackData attackData, FootAction action)
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (stompLogic == null) stompLogic = GetComponentInChildren<BossStompAttack>();
        
        if (stompLogic != null && attackData != null) 
        {
            stompLogic.SetDamage(attackData.damage);
        }

        StartCoroutine(ActionSequence(attackData, action));
    }

    private IEnumerator ActionSequence(BossAttackData data, FootAction action)
    {
        // 1. Hiện ra cảnh báo
        animator.SetTrigger("Prepare");
        
        float waitTime = data != null ? data.telegraphTime : 1f;
        yield return new WaitForSeconds(waitTime);

        // 2. Nếu là đòn thật thì nện xuống
        if (action == FootAction.Stomp)
        {
            animator.SetTrigger("Stomp");
            yield return new WaitForSeconds(1.5f); // Đợi animation nện (có thể chỉnh)
        }

        // 3. Rút chân về
        animator.SetTrigger("Retract");
        yield return new WaitForSeconds(1f); // Đợi animation rút

        // 4. Hủy object để giải phóng bộ nhớ
        Destroy(gameObject);
    }
}
