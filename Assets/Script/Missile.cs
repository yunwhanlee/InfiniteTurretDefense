using UnityEngine;

public class Missile : MonoBehaviour
{
    int dmg;
    float moveSpeed;

    Vector3 playerPos = Vector3.zero;
    Vector3 direction;

    void Start()
    {
        direction = (playerPos - transform.position).normalized;
    }

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //TODO Enemy를 Config 상수만들기
        if (col.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            enemy.OnHit(dmg);
            DestroyImmediate(gameObject);
        }
    }
}
