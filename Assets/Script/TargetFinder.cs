using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [Header("Search Settings")]
    public float radius = 10f;
    public LayerMask enemyLayer;

    public Enemy CurrentTarget;

    void Update()
    {
        // 타겟이 없거나, 죽었거나, 범위를 벗어났을 때만 다시 찾기
        if (CurrentTarget == null || !CurrentTarget.IsAlive || CurrentTarget.State == Enemy.STATE.DEAD)
        {
            CurrentTarget = FindNearestTarget();
        }
    }

    Enemy FindNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            enemyLayer
        );

        Enemy nearest = null;
        float minSqrDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || !enemy.IsAlive || enemy.State == Enemy.STATE.DEAD)
                continue;

            float sqrDist =
                ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    // bool IsInRange(Enemy enemy)
    // {
    //     float sqrDist = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
    //     return sqrDist <= radius * radius;
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
