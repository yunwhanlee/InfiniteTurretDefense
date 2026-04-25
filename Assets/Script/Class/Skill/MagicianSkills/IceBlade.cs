using UnityEngine;

public class IceBlade : MonoBehaviour
{
    public float moveSpeed = 8;
    Vector3 dir;
    int dmg;

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * dir;
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
