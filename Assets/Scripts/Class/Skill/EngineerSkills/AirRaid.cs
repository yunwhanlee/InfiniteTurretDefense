using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class AirRaid : MonoBehaviour
{
    public int dmg;
    Coroutine corID;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Debug.Log("KOKO");

            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(dmg, isCritical: false);
        }
    }

    public void Init(Vector3 pos, int dmg)
    {
        transform.position = pos;
        this.dmg = dmg;

        // 대기시간뒤 오브젝트 회수
        if(corID != null)
            StopCoroutine(corID);
        corID = StartCoroutine(CoRelease());
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    IEnumerator CoRelease()
    {
        yield return Config.WFS_1;
        yield return Config.WFS_0_5;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_AirRaid, gameObject);
    }
}
