using UnityEngine;
using static EffectPoolManager;
using static SkillPoolManager;

public class Bazooka : MonoBehaviour
{
    private const int LIFE_TIME = 5;
    private float lifeTime;

    public float moveSpeed = 5;
    [SerializeField] float radius;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;

        lifeTime -= Time.deltaTime;
        // 일정시간 지나면 회수
        if(lifeTime <= 0)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_Bazooka, gameObject);
        // 인게임 외부로 나갔다면 회수
        else if(Util.CheckOutSidePos(transform.position))
            GM._.spm.ReleasePoolDics(SK_IDX.SK_Bazooka, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 적과 충돌
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy.State == Enemy.STATE.DEAD)
                return;

            // 이펙트
            GM._.epm.SpawnPoolDics(EF_IDX.BazookaExplosionEF, transform.position);

            // 구 범위 충돌
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                radius,
                Config.Layer.ENEMY
            );

            // 범위 피해
            foreach(Collider2D hit in hits)
            {
                Enemy hitEnemy = hit.GetComponent<Enemy>();
                hitEnemy.OnHit(dmg, isCritical: false);
            }

            GM._.spm.ReleasePoolDics(SK_IDX.SK_Bazooka, gameObject);
        }
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg)
    {
        lifeTime = LIFE_TIME;
        transform.position = pos;
        this.dir = dir;
        this.dmg = dmg;

        // 발사 방향(각도)
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion

    // (기즈모) 공격범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
