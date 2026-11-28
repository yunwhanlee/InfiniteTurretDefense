using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 모든 드랍 아이템 오브젝트 풀링 매니저
/// </summary>
public class DropItemPoolManager : MonoBehaviour
{
    public enum DROPITEM_IDX
    {
        COIN,
        // 여기에 추가
    }

    [Header("드랍 아이템 프리팹")]
    public GameObject coin;

    [Header("드랍 연출 설정")]
    [SerializeField] int maxCoinCnt = 150;           // 필드 최대 코인 수 (풀 최대 보관 수)
    [SerializeField] float dropScatterRadius = 0.5f; // 적 사망 위치 주변 퍼짐 반경
    [SerializeField] float dropDuration = 0.35f;     // 포물선 착지 시간
    [SerializeField] float dropJumpPower = 0.6f;     // 포물선 점프 높이
    [SerializeField] int dropNumJumps = 1;           // 튕기는 횟수 (1~2 추천)

    [Header("자석 회수 설정")]
    public Transform collectSpotTf;                  // 드랍아이템 모이는 위치
    [SerializeField] float collectInterval = 2.5f;   // 회수 주기(초)
    [SerializeField] float magnetDuration = 0.45f;   // 날아가는 시간
    [SerializeField] float magnetMaxDelay = 0.2f;    // 코인마다 줄줄이 출발하는 랜덤 시차
    float collectTimer = 0f;

    // 오브젝트 풀링
    public Transform dropItemGroupTf;                // 드랍아이템 부모 그룹
    Dictionary<DROPITEM_IDX, IObjectPool<GameObject>> poolDics;     public Dictionary<DROPITEM_IDX, IObjectPool<GameObject>> PoolDics { get => poolDics; }
    Dictionary<DROPITEM_IDX, List<GameObject>> activeDics;          // 현재 필드에 활성화된 아이템 목록 (2단계 일괄 회수용)

    void Awake()
    {
        // 오브젝트 풀 및 활성 리스트 등록
        poolDics = new Dictionary<DROPITEM_IDX, IObjectPool<GameObject>>();
        activeDics = new Dictionary<DROPITEM_IDX, List<GameObject>>();

        RegisterPool(DROPITEM_IDX.COIN, coin, maxCoinCnt);
        // 여기에 추가
    }

    void Update()
    {
        collectTimer += Time.deltaTime;
        if (collectTimer >= collectInterval)
        {
            collectTimer = 0f;
            PullAllCoins();
        }
    }

    /// <summary>
    /// 바닥에 놓인 모든 IDLE 코인을 성 아래로 일괄 회수
    /// </summary>
    void PullAllCoins()
    {
        if (collectSpotTf == null)
            return;

        List<GameObject> activeCoins = activeDics[DROPITEM_IDX.COIN];
        if (activeCoins.Count == 0)
            return;

        Vector3 targetPos = collectSpotTf.position;

        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            if (!activeCoins[i].TryGetComponent(out Coin coinComp))
                continue;

            // 착지 완료(IDLE)된 코인만 회수 (방금 튀어오르는 중인 DROP 코인은 다음 주기에)
            if (coinComp.State != Coin.STATE.IDLE)
                continue;

            float randDelay = Random.Range(0f, magnetMaxDelay);
            coinComp.MoveToMagnet(targetPos, magnetDuration, randDelay, OnCoinArrived);
        }
    }

    /// <summary>
    /// 코인 도착
    /// </summary>
    void OnCoinArrived(Coin coinComp)
    {
        int gainVal = coinComp.Value;
        GM._.Coin += gainVal;

        // 코인 획득량 텍스트 애니메이션 표시
        GM._.coinTxtMng.ActiveAnim(gainVal);

        // 코인 오브젝트 회수
        ReleasePoolDics(DROPITEM_IDX.COIN, coinComp.gameObject);
    }

    // TODO 테스트용 (확인 후 삭제)
    // void Update()
    // {
    //     if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    //     {
    //         Vector3 randPos = Random.insideUnitCircle * 3f;
    //         SpawnPoolDics(DROPITEM_IDX.COIN, randPos, 10);
    //     }
    // }

    #region POOL
    GameObject Create(GameObject obj) => Instantiate(obj, dropItemGroupTf != null ? dropItemGroupTf : transform);
    void OnGet(GameObject obj) => obj.SetActive(true);
    void OnRelease(GameObject obj) => obj.SetActive(false);
    void OnDelete(GameObject obj) => Destroy(obj);

    /// <summary> 오브젝트 풀링리스트 초기화 </summary>
    private ObjectPool<GameObject> Init(GameObject pref, int max)
    {
        int safeMax = Mathf.Max(1, max);
        return new ObjectPool<GameObject>(() =>
            Create(pref), OnGet, OnRelease, OnDelete, maxSize: safeMax
        );
    }

    /// <summary> 풀 + 활성 리스트 동시 등록 </summary>
    private void RegisterPool(DROPITEM_IDX idx, GameObject pref, int max)
    {
        poolDics.Add(idx, Init(pref, max));
        activeDics.Add(idx, new List<GameObject>(Mathf.Min(max, 150)));
    }

    /// <summary>
    /// 드랍 아이템 생성 (적 사망 위치에서 포물선 드랍)
    /// </summary>
    /// <param name="enumIdx">DROPITEM_IDX</param>
    /// <param name="spawnPos">드랍 시작 위치</param>
    /// <param name="val">아이템 수치 (코인 금액 등)</param>
    public GameObject SpawnPoolDics(DROPITEM_IDX enumIdx, Vector3 spawnPos, int val = 1)
    {
        if (!poolDics.ContainsKey(enumIdx))
        {
            Debug.LogError($"[DropItemPoolManager] 등록되지 않은 DROPITEM_IDX: {enumIdx}");
            return null;
        }

        Vector2 scatter = Random.insideUnitCircle * dropScatterRadius;
        Vector3 targetPos = new Vector3(
            spawnPos.x + scatter.x,
            spawnPos.y + scatter.y,
            spawnPos.z
        );

        switch (enumIdx)
        {
            case DROPITEM_IDX.COIN:
            {
                if (val <= 0)
                    return null;

                GameObject obj = poolDics[enumIdx].Get();
                activeDics[enumIdx].Add(obj);

                if (obj.TryGetComponent(out Coin coinComp))
                {
                    coinComp.Init(val, spawnPos, targetPos, dropDuration, dropJumpPower, dropNumJumps);
                }
                else
                {
                    obj.transform.position = targetPos;
                }
                return obj;
            }
            // 여기에 추가
            default:
            {
                GameObject obj = poolDics[enumIdx].Get();
                activeDics[enumIdx].Add(obj);
                obj.transform.position = targetPos;
                return obj;
            }
        }
    }

    /// <summary>
    /// 드랍 아이템 풀 회수 (2단계 자석 도착 시 호출)
    /// </summary>
    public void ReleasePoolDics(DROPITEM_IDX enumIdx, GameObject obj)
    {
        if (obj == null || !activeDics.ContainsKey(enumIdx))
            return;

        // 중복 Release 방지
        if (!activeDics[enumIdx].Remove(obj))
            return;

        if (enumIdx == DROPITEM_IDX.COIN && obj.TryGetComponent(out Coin coinComp))
        {
            coinComp.ResetCoin();
        }

        poolDics[enumIdx].Release(obj);
    }

    /// <summary>
    /// 현재 필드에 활성화된 특정 타입 아이템 리스트 반환 (2단계 일괄 회수용)
    /// </summary>
    public List<GameObject> GetActiveList(DROPITEM_IDX enumIdx)
    {
        return activeDics.TryGetValue(enumIdx, out List<GameObject> list) ? list : null;
    }
#endregion
}
