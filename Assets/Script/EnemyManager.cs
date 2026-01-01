using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] float span;
    [Header("몬스터 생성 원 크기")]
    [SerializeField] float spawnRadius;

    public Enemy enemyPref;

    // 오브젝트 풀링
    public Transform enemyGroupTf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}

    void Awake()
    {
        pool = new ObjectPool<Enemy>(
            Create, OnGet, OnRelease, OnDelete, maxSize: 20
        );
    }

    void Start()
    {
        time = span;
    }

    void Update()
    {
        time += Time.deltaTime;

        if(time > span)
        {
            time = 0;

            // 적 생성
            Enemy enemy = pool.Get();
            // 죽었을시 오브젝트풀링 회수 이벤트 구독
            enemy.OnDeadEvent = (enemy) => pool.Release(enemy);
        }
    }

#region OBJECT POOL
    Enemy Create() => Instantiate(enemyPref, enemyGroupTf);

    void OnGet(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.transform.position = GetRandomCirclePosition(spawnRadius);
        enemy.Init();
    }

    void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    void OnDelete(Enemy enemy) => Destroy(enemy);
    #endregion

    #region FUNC
    Vector3 GetRandomCirclePosition(float radius)
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0
        );

        return Vector3.zero + dir * radius;
    }
#endregion
#region GIZMOS
    void OnDrawGizmosSelected() {
        const int SEGMENTS = 64;

        Gizmos.color = Color.green;

        Vector2 prev = new Vector2(spawnRadius, 0f);

        for (int i = 1; i <= SEGMENTS; i++)
        {
            float angle = (360f / SEGMENTS) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 next = new Vector2(
                Mathf.Cos(rad) * spawnRadius,
                Mathf.Sin(rad) * spawnRadius
            );

            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endregion
}