using UnityEngine;
using static Config;

public class TargetFinder : MonoBehaviour
{
    [Header("Search Settings")]
    public float radius;

    public Enemy CurrentTarget;

    void Update()
    {
        // 타겟이 없거나, 죽었거나, 범위를 벗어났을 때만 다시 찾기
        if (IsNeedToFindTarget(CurrentTarget))
        {
            CurrentTarget = FindNearestTarget();
        }
    }

#region FUNC
    private bool IsNeedToFindTarget(Enemy enemy)
    {
        return enemy == null
            || enemy.State == Enemy.STATE.DEAD
            || !IsInRange(enemy);
    }

    Enemy FindNearestTarget()
    {
        Debug.Log("Finding nearest target...");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            Layer.ENEMY
        );

        Enemy nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (IsNeedToFindTarget(enemy))
                continue;

            float curDistance = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;

            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    bool IsInRange(Enemy enemy)
    {
        float sqrDist = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
        return sqrDist <= radius * radius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endregion
}
