using UnityEngine;
using static Config;
using System;

public class TargetFinder : MonoBehaviour
{
    [Header("Search Settings")]
    public float radius;
    public Enemy CurrentTarget;
    public Action OnTargetChanged; // 타겟이 바뀌었을때 호출되는 이벤트 액션

    void Awake()
    {
        OnTargetChanged = () => {};
    }

    void Update()
    {
        // 타겟이 없거나, 죽었거나, 범위를 벗어났을 때만 다시 찾기
        if (IsNeedToFindTarget(CurrentTarget))
        {
            // 먼저 새로운 타겟을 찾음
            Enemy newTarget = FindNearestTarget();

            // 새로 찾은 타겟이 기존 타겟과 다르고, 유효한 타겟이라면 이벤트 호출
            if (newTarget != CurrentTarget && newTarget != null)
                OnTargetChanged?.Invoke();

            CurrentTarget = newTarget;
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
        // Debug.Log("Finding nearest target...");

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
