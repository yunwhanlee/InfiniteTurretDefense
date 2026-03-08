using System.Collections;
using UnityEngine;
using static SkillPoolManager;

/// <summary>
/// 궁수 피닉스 스킬
/// </summary>
public class PhoenixArrow : MonoBehaviour
{
    public float moveSpeed = 4;
    Vector3 direction;
    int damage;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            enemy.OnHit(damage, isCritical: false);
        }
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

        // 일정시간뒤 제거 및 불장판 생성
        StartCoroutine(CoDestroyMe());
    }

    IEnumerator CoDestroyMe()
    {
        // 불장판 생성
        yield return new WaitForSeconds(3.5f);
        PhoenixFireField fireField = GM._.spm.SpawnPoolDics(SK_IDX.SK_PhoenixFireField).GetComponent<PhoenixFireField>();
        fireField.Init(direction, Mathf.RoundToInt(damage * 0.1f));

        // 오브젝트 회수
        yield return new WaitForSeconds(0.5f);
        GM._.spm.ReleasePoolDics(SK_IDX.SK_PassArrow, gameObject);
    }
#endregion
}
