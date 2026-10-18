using UnityEngine;
using static SkillPoolManager;

public class IceBlade : MonoBehaviour
{
    const int LIFE_TIME = 5;
    float lifeTime = 0;

    public float moveSpeed = 8;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;

        lifeTime -= Time.deltaTime;
        // 일정시간 지나면 회수
        if(lifeTime <= 0)
            GM._.spm.ReleasePoolDics(SK_IDX.SK_IceBlade, gameObject);
        // 인게임 외부로 나갔다면 회수
        else if(Util.CheckOutSidePos(transform.position))
            GM._.spm.ReleasePoolDics(SK_IDX.SK_IceBlade, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Enemy enemy = col.GetComponent<Enemy>();
        if(enemy.State == Enemy.STATE.DEAD)
            return;
        
        //TODO 이펙트

        enemy.Slow(5);
        enemy.OnHit(dmg, false);
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg, float angleOffset)
    {
        lifeTime = LIFE_TIME;

        transform.position = pos;
        this.dmg = dmg;

        // 발사 방향(각도)을 먼저 계산합니다.
        Vector3 rotatedDir = Quaternion.Euler(0, 0, angleOffset) * dir;
        // 계산된 방향을 클래스 멤버 변수에 저장합니다. (실제 이동 방향)
        this.dir = rotatedDir; 
        // 이미지(Transform)의 회전값을 맞춰줍니다.
        float angle = Mathf.Atan2(rotatedDir.y, rotatedDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion
}
