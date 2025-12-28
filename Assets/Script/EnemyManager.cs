using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] float span;
    [Header("몬스터 생성 원 크기")]
    [SerializeField] float spawnRadius;

    [Header("몬스터 생성 원 크기")]
    public GameObject enemyPref;

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

            float x = Random.Range(-2, 2);
            float y = Random.Range(-2, 2);

            Instantiate(enemyPref, GetRandomCirclePosition(spawnRadius), Quaternion.identity);
        }
    }

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