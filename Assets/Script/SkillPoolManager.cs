using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
        SK_IceBlade, // 칼날얼음
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
    public IceBlade iceBladePf;

    void Awake()
    {
        poolDics = new Dictionary<SK_IDX, IObjectPool<GameObject>>();
        poolDics.Add(SK_IDX.SK_PassArrow, Init(passArrowPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_ArrowRain, Init(arrowRainPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_PhoenixArrow, Init(phoenixArrowPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_PhoenixFireField, Init(phoenixFireField.gameObject, 2));
        poolDics.Add(SK_IDX.SK_FireBall, Init(fireBallPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_IceBlade, Init(iceBladePf.gameObject, 14));
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