using UnityEngine;

public class Missile : MonoBehaviour
{
    private const int LIFE_TIME = 5;
    private float lifeTime;

    [SerializeField] int dmg; public int Dmg {get => dmg; set => dmg = value;}
    [SerializeField] bool isCritical; public bool IsCritical {get => isCritical; set => isCritical = value;}
    [SerializeField] float moveSpeed;

    Vector3 direction; public Vector3 Direction {get => direction; set => direction = value;}
    [SerializeField] bool isHit = false;

    [SerializeField] SpriteRenderer sprRdr;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;

        lifeTime -= Time.deltaTime;
        // 일정시간 지나면 회수
        if(lifeTime <= 0)
            GM._.mpm.Pool.Release(this);
        // 인게임 외부로 나갔다면 회수
        else if(Util.CheckOutSidePos(transform.position))
            GM._.mpm.Pool.Release(this);
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
            enemy.OnHit(dmg, isCritical);
            GM._.mpm.Pool.Release(this);
        }
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, float angleOffset, Sprite missileSpr)
    {
        if(!missileSpr)
            Debug.LogError("미사일 이미지 스프라이트가 NULL입니다.");

        lifeTime = LIFE_TIME;
        isHit = false;
        transform.position = pos;

        // 발사 방향(각도)
        direction = Quaternion.Euler(0, 0, angleOffset) * dir;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 미사일 이미지 적용
        sprRdr.sprite = missileSpr;
    }
#endregion
}
