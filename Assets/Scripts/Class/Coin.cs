using UnityEngine;
using DG.Tweening;

/// <summary>
/// 필드에 드랍되는 코인
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour
{
    public enum STATE
    {
        DROP,   // 적 위치에서 튀어 올라 착지 중
        IDLE,   // 바닥에 착지해 회수 대기 중
        MAGNET  // (2단계 예정) 성 아래로 끌려가는 중
    }

    [SerializeField] STATE state;
    public STATE State => state;

    [SerializeField] int value;
    public int Value => value;

    // 드랍 포물선 연출용 내부 상태
    Vector3 startPos;
    Vector3 landPos;
    float elapsed;
    float dropDuration;
    float jumpHeight;

    /// <summary>
    /// 코인 드랍 초기화
    /// </summary>
    public void Init(int coinValue, Vector3 spawnPos, Vector3 targetLandPos, float duration, float height)
    {
        value = coinValue;
        startPos = spawnPos;
        landPos = targetLandPos;
        dropDuration = Mathf.Max(0.01f, duration);
        jumpHeight = height;
        elapsed = 0f;
        state = STATE.DROP;

        transform.position = spawnPos;
    }

    /// <summary>
    /// 풀 상한 초과 시 기존 코인에 금액 합산 (메모리 상한 고정용)
    /// </summary>
    public void AddValue(int amount)
    {
        value += amount;
    }

    /// <summary>
    /// 드랍 포물선 이동 갱신 (매니저의 Update에서 호출)
    /// </summary>
    public void TickDrop(float deltaTime)
    {
        if (state != STATE.DROP)
            return;

        elapsed += deltaTime;
        float t = Mathf.Clamp01(elapsed / dropDuration);

        // 수평 선형 이동 + 포물선(4t(1-t)) 높이
        Vector3 pos = Vector3.Lerp(startPos, landPos, t);
        pos.y += 4f * t * (1f - t) * jumpHeight;
        transform.position = pos;

        if (t >= 1f)
        {
            transform.position = landPos;
            state = STATE.IDLE;
        }
    }

    /// <summary>
    /// 풀 반환 시 상태 초기화
    /// </summary>
    public void ResetCoin()
    {
        value = 0;
        elapsed = 0f;
        state = STATE.IDLE;
    }
}
