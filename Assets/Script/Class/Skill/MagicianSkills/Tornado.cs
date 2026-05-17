using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class Tornado : MonoBehaviour
{
    public float moveSpeed = 5;
    public float knockbackPower = 10;
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
        transform.position = pos;
        this.dir = dir;
        this.dmg = dmg;
        StartCoroutine(CoRelease());
    }

    IEnumerator CoRelease()
    {
        yield return Config.WFS_5;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_Tornado, gameObject);
    }
#endregion
}
