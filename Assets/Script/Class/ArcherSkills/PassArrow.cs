using UnityEngine;
using static SkillPoolManager;

/// <summary>
/// 궁수 관통샷 스킬
/// </summary>
public class PassArrow : MonoBehaviour
{
    public float moveSpeed = 10;
    private Vector3 direction;
    private int damage;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Config.TAG.Enemy))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(damage, isCritical: false);
        }
    }

    // 오브젝트가 카메라 시야에서 완전히 사라지면 호출됨
    void OnBecameInvisible()
    {
        if(gameObject.activeSelf)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_PassArrow, gameObject);
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg)
    {
        transform.position = pos;
        direction = dir;
        damage = dmg;

        // 발사 방향(각도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion
}
