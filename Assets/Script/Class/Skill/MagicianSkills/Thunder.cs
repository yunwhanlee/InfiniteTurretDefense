using System.Collections;
using UnityEngine;
using static EffectPoolManager;
using static SkillPoolManager;

public class Thunder : MonoBehaviour
{
    [SerializeField] float radius;
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

            // // 구 범위 충돌
            // Collider2D[] hits = Physics2D.OverlapCircleAll(
            //     transform.position,
            //     radius,
            //     Config.Layer.ENEMY
            // );

            // // 범위 피해
            // foreach(Collider2D hit in hits)
            // {
            //     Enemy hitEnemy = hit.GetComponent<Enemy>();
            //     hitEnemy.OnHit(dmg, isCritical: false);
            //     hitEnemy.Stun(2);
            // }
        }
    }

#region FUNC
    public void Init(Vector3 pos, int dmg)
    {
        transform.position = pos;
        this.dmg = dmg;

        StartCoroutine(CoRelease());
    }

    IEnumerator CoRelease()
    {
        yield return Config.WFS_1;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_Thunder, gameObject);
    }
#endregion
}
