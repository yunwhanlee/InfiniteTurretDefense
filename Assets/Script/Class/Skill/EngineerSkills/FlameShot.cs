using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillPoolManager;

public class FlameShot : MonoBehaviour
{
    const float dmgInterval = 0.75f; // 데미지 간격
    public int dmg;
    private Dictionary<Enemy, float> lastHitTimeDict = new Dictionary<Enemy, float>();

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            // 처음 공격당한 적이라면 딕셔너리 등록
            if(!lastHitTimeDict.ContainsKey(enemy))
                lastHitTimeDict[enemy] = 0f; // 0초 초기화

            // 데미지 간격시간까지 경과했다면
            if(Time.time >= lastHitTimeDict[enemy] + dmgInterval)
            {
                enemy.OnHit(dmg, isCritical: false);

                // 적이 죽었다면 해당 인덱스 지우고 종료
                if(enemy.State == Enemy.STATE.DEAD)
                {
                    lastHitTimeDict.Remove(enemy);
                    return;
                }

                // 피격시간을 현재로 갱신
                lastHitTimeDict[enemy] = Time.time;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // 메모리 관리를 위해 빔 범위에서 나간 적은 딕셔너리에서 제외
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && lastHitTimeDict.ContainsKey(enemy))
            {
                lastHitTimeDict.Remove(enemy);
            }
        }
    }

#region FUNC
    public void Init(Vector3 pos, float zRot, int dmg)
    {
        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(0, 0, zRot);
        this.dmg = dmg;
        lastHitTimeDict.Clear();
        // StartCoroutine(CoRelease());
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    // IEnumerator CoRelease()
    // {
    //     yield return Config.WFS_5;
    //     yield return Config.WFS_3;
    //     GM._.spm.ReleasePoolDics(SK_IDX.SK_FlameShot, gameObject);
    // }
#endregion
}
