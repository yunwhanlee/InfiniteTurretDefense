using UnityEngine;
using static SkillPoolManager;
using System.Collections;

public class DoubleThrow : MonoBehaviour
{
    const float ORG_MOVE_SPEED = 8;
    [SerializeField] float moveSpeed = ORG_MOVE_SPEED;
    [SerializeField] SpriteRenderer firstSprRdr;
    [SerializeField] SpriteRenderer secondSprRdr;

    [SerializeField] bool isHit = false;
    [SerializeField] bool isCritical;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(isHit)
            return;

        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            isHit = true;
            StartCoroutine(CorOnHits(enemy));
        }
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg, bool isCritical, Sprite missileSpr)
    {
        isHit = false;
        this.isCritical = isCritical;
        moveSpeed = ORG_MOVE_SPEED;

        firstSprRdr.sprite = missileSpr;
        secondSprRdr.sprite = missileSpr;
        firstSprRdr.enabled = true;
        secondSprRdr.enabled = true;

        transform.position = pos;
        this.dir = dir;
        this.dmg = dmg;

        // 발사 방향(각도)
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 디증 공격
    /// </summary>
    IEnumerator CorOnHits(Enemy enemy)
    {
        enemy.OnHit(dmg, isCritical);
        firstSprRdr.enabled = false;
        secondSprRdr.enabled = false;
        moveSpeed = 0;

        yield return Config.WFS_0_1;
        enemy.OnHit(dmg, isCritical);

        // 반환
        GM._.spm.ReleasePoolDics(SK_IDX.SK_DoubleThrow, gameObject);
    }
#endregion
}
