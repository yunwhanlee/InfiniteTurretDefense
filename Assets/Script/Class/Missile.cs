using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] int dmg; public int Dmg {get => dmg; set => dmg = value;}
    [SerializeField] bool isCritical; public bool IsCritical {get => isCritical; set => isCritical = value;}
    [SerializeField] float moveSpeed;

    Vector3 direction; public Vector3 Direction {get => direction; set => direction = value;}
    [SerializeField] bool isHit = false;

    [SerializeField] SpriteRenderer sprRdr;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
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

    // 오브젝트가 카메라 시야에서 완전히 사라지면 호출됨
    void OnBecameInvisible()
    {
        if(gameObject.activeSelf)
            GM._.mpm.Pool.Release(this);
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, float angleOffset, Sprite missileSpr)
    {
        if(!missileSpr)
            Debug.LogError("미사일 이미지 스프라이트가 NULL입니다.");

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
