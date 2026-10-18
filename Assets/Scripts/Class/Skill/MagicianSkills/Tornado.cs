using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class Tornado : MonoBehaviour
{
    private const int LIFE_TIME = 6;
    private float lifeTime;

    public float moveSpeed = 5;
    public float knockbackPower = 10;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;

        lifeTime -= Time.deltaTime;
        // 일정시간 지나면 회수
        if(lifeTime <= 0)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_Tornado, gameObject);
        // 인게임 외부로 나갔다면 회수
        else if(Util.CheckOutSidePos(transform.position))
            GM._.spm.ReleasePoolDics(SK_IDX.SK_Tornado, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(dmg, false);

            if(enemy.State != Enemy.STATE.KNOCKBACK)
                enemy.KnockBack(5, dir);
        }
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg)
    {
        lifeTime = LIFE_TIME;
        transform.position = pos;
        this.dir = dir;
        this.dmg = dmg;
    }
#endregion
}
