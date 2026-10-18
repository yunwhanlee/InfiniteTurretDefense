using UnityEngine;
using static SkillPoolManager;

public class StormShuriken : MonoBehaviour
{
    private const int LIFE_TIME = 5;
    private float lifeTime;

    public float moveSpeed = 5;
    private Vector3 dir;
    private int dmg;


    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;

        // 일정시간 지나면 회수
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_StormShuriken, gameObject);
        // 인게임 외부로 나갔다면 회수
        else if(Util.CheckOutSidePos(transform.position))
            GM._.spm.ReleasePoolDics(SK_IDX.SK_StormShuriken, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(dmg, isCritical: false);
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
}
