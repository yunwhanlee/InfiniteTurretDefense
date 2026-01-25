using System;
using UnityEngine;

public class GM : MonoBehaviour
{
    // 싱글톤 패턴
    public static GM _;

    //* 이벤트
    public event Action<int> OnCoinChanged;
    public event Action<int> OnDiamondChanged;

    //TODO DB 재화
    [SerializeField] int coin;  public int Coin {
        get => coin;
        set {
            coin = value;
            OnCoinChanged?.Invoke(coin); // 이벤트 호출
        }
    }
    [SerializeField] int diamond;   public int Diamond
    {
        get => diamond;
        set
        {
            diamond = value;
            OnDiamondChanged?.Invoke(diamond); // 이벤트 호출
        }
    }

    // 컴포넌트
    public Tower tower;
    public CharaManager crm;
    public EnemyManager emm;
    public MissileManager msm;

    void Awake()
    {
        _ = this;
        tower = GameObject.Find("Tower").GetComponent<Tower>();
        crm = GameObject.Find("CharaManager").GetComponent<CharaManager>();
        emm = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        msm = GameObject.Find("MissileManager").GetComponent<MissileManager>();
    }

    void Start()
    {
        //* 이벤트 등록 먼저
        // (이벤트 등록) 코인 변경시 UI 업데이트
        OnCoinChanged = (_coin) => UI._.coinTxt.text = $"{_coin}";
        // (이벤트 등록) 다이아몬드 변경시 UI 업데이트
        OnDiamondChanged = (_diamond) => UI._.diamondTxt.text = $"{_diamond}";

        //TODO DB 재화 로드
        Coin = 10000;
        Diamond = 100;
    }

    //! 어차피 하나의 씬에서 플레이 될거라서 필요X
    // void Singleton()
    // {
    //     // 1. 인스턴스가 비어있다면(처음 생성된 거라면)
    //     if (_ == null)
    //     {
    //         _ = this;
    //         DontDestroyOnLoad(gameObject); // 핵심: 씬이 넘어가도 파괴되지 않게 설정
    //     }
    //     else // 2. 이미 누군가(진짜)가 존재한다면
    //     {
    //         Destroy(gameObject); // 나는 가짜니까 내 게임 오브젝트를 통째로 파괴
    //     }
    // }
}
