using System.Collections;
using UnityEngine;
using static SkillPoolManager;

/// <summary>
/// 마법사 : 천둥번개 스킬
/// </summary>
public class Thunder : MonoBehaviour
{
    int dmg;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            // 적과 충돌
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(dmg, isCritical: false);
            enemy.Stun(2);
        }
    }

#region FUNC
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init(Vector3 pos, int dmg)
    {
        transform.position = pos;
        this.dmg = dmg;

        StartCoroutine(CoRelease());
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    IEnumerator CoRelease()
    {
        yield return Config.WFS_1;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_Thunder, gameObject);
    }
#endregion
}
