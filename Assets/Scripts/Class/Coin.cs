using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour
{
    public enum STATE
    {
        DROP,   // 적 위치에서 튀어 올라 착지 중
        IDLE,   // 바닥에 착지해 회수 대기 중
        MAGNET  // (2단계) 성 아래로 끌려가는 중
    }

    [SerializeField] int value;
    public int Value => value;

    [SerializeField] STATE state;
    public STATE State => state;

    TweenCallback onDropComplete;
    Action<Coin> onMagnetArrived;
    TweenCallback onMagnetComplete;

    void Awake()
    {
        // 매 드랍마다 람다(GC)가 생기지 않도록 콜백 캐싱
        onDropComplete = () => state = STATE.IDLE;
        onMagnetComplete = () => onMagnetArrived?.Invoke(this);
    }

    void OnDisable()
    {
        transform.DOKill();
    }

    /// <summary>
    /// 코인 드랍 (DOJump 포물선 연출)
    /// </summary>
    public void Init(int coinValue, Vector3 spawnPos, Vector3 targetPos, float duration = 0.35f, float jumpPower = 0.6f, int numJumps = 1)
    {
        transform.DOKill();

        value = coinValue;
        state = STATE.DROP;
        transform.position = spawnPos;

        transform
            .DOJump(targetPos, jumpPower, numJumps, duration)
            .SetEase(Ease.OutQuad)
            .SetRecyclable(true)     // DOTween 내부 트윈 재사용 (GC 절감)
            .SetLink(gameObject)     // 오브젝트 파괴 시 자동 Kill
            .OnComplete(onDropComplete);
    }

    /// <summary>
    /// 풀 반환 시 정리
    /// </summary>
    public void ResetCoin()
    {
        transform.DOKill();
        onMagnetArrived = null;
        value = 0;
        state = STATE.IDLE;
    }

    /// <summary>
    /// 성 아래 목표 지점으로 자석처럼 이동
    /// </summary>
    public void MoveToMagnet(Vector3 targetPos, float duration, float delay, Action<Coin> onArrived)
    {
        if (state == STATE.MAGNET)
            return;

        transform.DOKill();

        state = STATE.MAGNET;
        onMagnetArrived = onArrived;

        transform
            .DOMove(targetPos, duration)
            .SetDelay(delay)
            .SetEase(Ease.InBack)    // 살짝 뒤로 당겨졌다가 성으로 꽂히는 자석 느낌
            .SetRecyclable(true)
            .SetLink(gameObject)
            .OnComplete(onMagnetComplete);
    }

}