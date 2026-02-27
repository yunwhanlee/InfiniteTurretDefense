using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class ArrowRain : MonoBehaviour
{
    const int DURATION = 5; // 지속시간
    const float SPAN = 1;   // 공격주기

    WaitForSeconds attackSpan;
    Transform enemyGroupTf;
    Coroutine corID;
    int dmg;

    void Awake()
    {
        attackSpan = new WaitForSeconds(SPAN);
    }

#region FUNC
    public void Init(int dmg)
    {
        enemyGroupTf = GM._.emm.enemyGroupTf;
        this.dmg = dmg;

        if(corID != null)
            StopCoroutine(corID);

        // 화살비 공격 코루틴
        corID = StartCoroutine(CoArrowRain());
    }

    IEnumerator CoArrowRain()
    {
        // 5초 동안 1초 간격으로 데미지 = 총 5번 반복
        float attackCount = DURATION / SPAN; 
        
        for (int i = 0; i < attackCount; i++)
        {
            // 1초(SPAN) 대기
            yield return attackSpan;
            Attack(dmg);
        }

        // 오브젝트 풀 회수
        GM._.spm.ReleasePoolDics(SK_IDX.SK_ArrowRain, gameObject);
    }

    void Attack(int dmg)
    {
        for (int i = enemyGroupTf.childCount - 1; i >= 0; i--)
        {
            Transform child = enemyGroupTf.GetChild(i);
            Enemy enemy = child.GetComponent<Enemy>();

            if (enemy != null && enemy.IsAlive)
            {
                enemy.OnHit(dmg, false);
            }
        }
    }
#endregion
}
