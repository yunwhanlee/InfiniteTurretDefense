using UnityEngine;
using UnityEngine.UIElements;

public enum MonsterType
{
    Normal, Elite, Boss
}

public class Enemy : MonoBehaviour
{
    Vector3 playerPos;

    public MonsterType type;
    public int maxHp;
    public float hp;
    public bool IsAlive => hp > 0;
    public float moveSpeed;

    void Start()
    {
        playerPos = Vector3.zero;
        hp = maxHp;
    }

    void Update()
    {
        Vector3 dir = (playerPos - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * dir;
    }
}
