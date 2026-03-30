using Assets.PixelFantasy.Common.Scripts.CollectionScripts;
using UnityEngine;
using static EffectPoolManager;
using static SkillPoolManager;

public class FireBall : MonoBehaviour
{
    public float moveSpeed = 10;
    [SerializeField] float radius = 2;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy hitEnemy = col.GetComponent<Enemy>();
            if(hitEnemy.State == Enemy.STATE.DEAD)
                return;

            // 이펙트
            GM._.epm.SpawnPoolDics(EF_IDX.FireBallExplosionEF, transform.position);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                radius,
                Config.Layer.ENEMY
            );

            foreach(Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                enemy.OnHit(dmg, isCritical: false);
            }

            GM._.spm.ReleasePoolDics(SK_IDX.SK_FireBall, gameObject);
        }
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg)
    {
        transform.position = pos;
        this.dir = dir;
        this.dmg = dmg;

        // 발사 방향(각도)
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion

        // (기즈모) 휠윈드 공격범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
