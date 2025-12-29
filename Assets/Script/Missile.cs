using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] int dmg; public int Dmg {get => dmg; set => dmg = value;}
    [SerializeField] float moveSpeed;

    Vector3 direction; public Vector3 Direction {get => direction; set => direction = value;}

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
            Destroy(gameObject);
        }
    }

#region FUNC
#endregion
}
