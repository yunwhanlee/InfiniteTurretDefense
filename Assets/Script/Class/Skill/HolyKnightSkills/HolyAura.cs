using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillPoolManager;

public class HolyAura : MonoBehaviour
{
    float decPer;
    [SerializeField] List<Enemy> enemyList = new List<Enemy>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy.State == Enemy.STATE.DEAD)
                return;

            // 적 공격력 감소
            enemy.ExtraDmg -= Mathf.RoundToInt(enemy.Dmg * decPer);
            enemyList.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemyList.Contains(enemy))
            {
                // 적 공격력 원상복구 및 리스트에서 제거
                enemy.ExtraDmg += Mathf.RoundToInt(enemy.Dmg * decPer);
                enemyList.Remove(enemy);
            }
        }
    }

#region FUNC
    public void Init(Vector3 pos, float duration, float decPer)
    {
        transform.position = pos;
        this.decPer = decPer;

        StartCoroutine(CoRelease(duration));
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    /// <param name="duration">지속시간</param>
    IEnumerator CoRelease(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 적 공격력 원상복구
        for(int i = 0; i < enemyList.Count; i++)
        {
            if(enemyList[i] != null)
            {
                enemyList[i].ExtraDmg += Mathf.RoundToInt(enemyList[i].Dmg * decPer);
            }
        }

        // 회수
        GM._.spm.ReleasePoolDics(SK_IDX.SK_HolyAura, gameObject);
    }
#endregion
}
