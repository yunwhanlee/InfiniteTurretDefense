using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] int dmg; public int Dmg {get => dmg; set => dmg = value;}
    [SerializeField] float moveSpeed;

    Vector3 direction; public Vector3 Direction {get => direction; set => direction = value;}
    [SerializeField] bool isHit = false;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(isHit)
            return;

        //TODO Enemy를 Config 상수만들기
        if (col.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if(enemy.State == Enemy.STATE.DEAD)
                return;

            isHit = true;
            enemy.OnHit(dmg);
            GameManager._.msm.Pool.Release(this);
        }
    }

    // 오브젝트가 카메라 시야에서 완전히 사라지면 호출됨
    void OnBecameInvisible()
    {
        if(gameObject.activeSelf)
            GameManager._.msm.Pool.Release(this);
    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir)
    {
        isHit = false;

        transform.position = pos;

        // 발사 방향(각도)
        direction = dir;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion
}
