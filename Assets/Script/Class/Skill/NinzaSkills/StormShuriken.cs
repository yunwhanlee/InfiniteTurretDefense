using UnityEngine;
using static SkillPoolManager;

public class StormShuriken : MonoBehaviour
{
    public float moveSpeed = 5;
    private Vector3 dir;
    private int dmg;

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

            enemy.OnHit(dmg, isCritical: false);
        }
    }

    // 오브젝트가 카메라 시야에서 완전히 사라지면 호출됨
    void OnBecameInvisible()
    {
        if(gameObject.activeSelf)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_StormShuriken, gameObject);
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
}
