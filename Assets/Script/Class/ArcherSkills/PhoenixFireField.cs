using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillPoolManager;

public class PhoenixFireField : MonoBehaviour
{
    const int DURATION = 10; // 지속시간
    const float SPAN = 1;   // 공격주기

    [SerializeField] List<Enemy> enemyList;
    WaitForSeconds attackSpan;
    private Vector3 direction;
    [SerializeField] int damage;
    Coroutine corID;

    void Awake()
    {
        attackSpan = new WaitForSeconds(SPAN);
    }

    #region FUNC
    public void Init(Vector3 dir, int dmg)
    {
        enemyList = new List<Enemy>();

        direction = dir;
        damage = dmg;

        // 발사 방향(각도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if(corID != null)
            StopCoroutine(corID);

        corID = StartCoroutine(CoFireField());
    }

    /// <summary>
    /// 불장판 깔기
    /// </summary>
    /// <returns></returns>
    IEnumerator CoFireField()
    {
        // 5초 동안 1초 간격으로 데미지 = 총 5번 반복
        float attackCount = DURATION / SPAN; 
        
        for (int i = 0; i < attackCount; i++)
        {
            // 1초(SPAN) 대기
            yield return attackSpan;
            Attack(damage);
        }

        // 오브젝트 풀 회수
        GM._.spm.ReleasePoolDics(SK_IDX.SK_PhoenixFireField, gameObject);
    }

    void Attack(int dmg)
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemyList[i];

            if (enemy != null)
            {
                if(enemy.IsAlive)
                    enemy.OnHit(dmg, false); // 적에게 공격
                else
                    enemyList.RemoveAt(i); // 죽은 적은 리스트에서 안전하게 제거
            }
            else
            {
                // 이미 파괴된 오브젝트(null)인 경우도 리스트에서 정리
                enemyList.RemoveAt(i);
            }
        }
    }
#endregion
#region COLLIDE
void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.CompareTag(Config.TAG.Enemy))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !enemyList.Contains(enemy))
            {
                enemyList.Add(enemy); //* 敵リスト追加
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D col) 
    {
        if(col.gameObject.CompareTag(Config.TAG.Enemy))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy); //* 敵リスト削除
            }
        }
    }
#endregion
}
