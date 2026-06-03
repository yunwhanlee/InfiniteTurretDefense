using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 오브젝트 클래스로 생성되는 스킬 매니저
/// </summary>
public class SkillPoolManager : MonoBehaviour
{
    public enum SK_IDX
    {
        //* 궁수 스킬
        SK_PassArrow, // 관통샷
        SK_ArrowRain, // 화살비
        SK_PhoenixArrow, // 피닉스화살
        SK_PhoenixFireField, // 피닉스 불장판
        //* 법사 스킬
        SK_FireBall, // 파이어볼
        SK_MagicOrb, // 매직오브
        SK_IceBlade, // 칼날얼음
        SK_Thunder, // 천둥번개
        SK_Tornado, // 토네이도
        //* 성기사 스킬
        SK_HolyBurst, // 빛폭발
        SK_HolyAura, // 빛의아우라
        SK_HolyBeam, // 빛의기둥
        SK_HolySmite, // 빛의심판
        //* 닌자 스킬
        SK_DoubleThrow, // 더블쓰로우
        SK_StormShuriken, // 폭풍수리검
    }

    // 오브젝트 풀링
    public Transform skillGroupTf;
    Dictionary<SK_IDX, IObjectPool<GameObject>> poolDics;     public Dictionary<SK_IDX, IObjectPool<GameObject>> PoolDics {get => poolDics;}
    [Header("궁수 스킬")]
    public PassArrow passArrowPf;
    public ArrowRain arrowRainPf;
    public PhoenixArrow phoenixArrowPf;
    public PhoenixFireField phoenixFireField;
    [Header("법사 스킬")]
    public FireBall fireBallPf;
    public MagicOrb magicOrbPf;
    public IceBlade iceBladePf;
    public Thunder thunderPf;
    public Tornado tornadoPf;
    [Header("성기사 스킬")]
    public HolyBurst holyBurst;
    public HolyAura holyAura;
    public HolyBeam holyBeam;
    public HolySmite holySmite;
    [Header("닌자 스킬")]
    public DoubleThrow doubleThrowPf;
    public StormShuriken stormShurikenPf;

    void Awake()
    {
        poolDics = new Dictionary<SK_IDX, IObjectPool<GameObject>>();
        // 전사
        poolDics.Add(SK_IDX.SK_PassArrow, Init(passArrowPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_ArrowRain, Init(arrowRainPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_PhoenixArrow, Init(phoenixArrowPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_PhoenixFireField, Init(phoenixFireField.gameObject, 2));
        // 마법사
        poolDics.Add(SK_IDX.SK_FireBall, Init(fireBallPf.gameObject, 3));
        poolDics.Add(SK_IDX.SK_MagicOrb, Init(magicOrbPf.gameObject, 1));
        poolDics.Add(SK_IDX.SK_IceBlade, Init(iceBladePf.gameObject, 14));
        poolDics.Add(SK_IDX.SK_Thunder, Init(thunderPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_Tornado, Init(tornadoPf.gameObject, 2));
        // 성기사
        poolDics.Add(SK_IDX.SK_HolyBurst, Init(holyBurst.gameObject, 1));
        poolDics.Add(SK_IDX.SK_HolyAura, Init(holyAura.gameObject, 1));
        poolDics.Add(SK_IDX.SK_HolyBeam, Init(holyBeam.gameObject, 1));
        poolDics.Add(SK_IDX.SK_HolySmite, Init(holySmite.gameObject, 1));
        // 닌자
        poolDics.Add(SK_IDX.SK_DoubleThrow, Init(doubleThrowPf.gameObject, 10));
        poolDics.Add(SK_IDX.SK_StormShuriken, Init(stormShurikenPf.gameObject, 5));
    }

#region POOL
    GameObject Create(GameObject obj) => Instantiate(obj, skillGroupTf);
    void OnGet(GameObject obj) => obj.gameObject.SetActive(true);
    void OnRelease(GameObject obj) => obj.gameObject.SetActive(false);
    void OnDelete(GameObject obj) => Destroy(obj);

    /// <summary> 오브젝트 풀링리스트 초기화 </summary>
    private ObjectPool<GameObject> Init(GameObject pref, int max)
    {
        return new ObjectPool<GameObject>(() => 
            Create(pref), OnGet, OnRelease, OnDelete, maxSize: max
        );
    }

    /// <summary> 오브젝트 풀링리스트 생성 </summary>
    public GameObject SpawnPoolDics(SK_IDX enumIdx) => poolDics[enumIdx].Get();
    /// <summary> 오브젝트 풀링리스트 회수 </summary>
    public void ReleasePoolDics(SK_IDX enumIdx, GameObject obj) => poolDics[enumIdx].Release(obj);
#endregion
}