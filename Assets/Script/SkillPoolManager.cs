using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkillPoolManager : MonoBehaviour
{
    public enum SK_IDX
    {
        SK_PassArrow, // 관통샷 (궁수스킬),
        SK_ArrowRain, // 화살비 (궁수스킬),
        SK_PhoenixArrow, // 피닉스화살 (궁수스킬),
    }

    // 오브젝트 풀링
    public Transform skillGroupTf;
    Dictionary<SK_IDX, IObjectPool<GameObject>> poolDics;     public Dictionary<SK_IDX, IObjectPool<GameObject>> PoolDics {get => poolDics;}

    public PassArrow passArrowPf;
    public ArrowRain arrowRainPf;
    public PhoenixArrow phoenixArrowPf;

    void Awake()
    {
        poolDics = new Dictionary<SK_IDX, IObjectPool<GameObject>>();
        poolDics.Add(SK_IDX.SK_PassArrow, Init(passArrowPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_ArrowRain, Init(arrowRainPf.gameObject, 2));
        poolDics.Add(SK_IDX.SK_PhoenixArrow, Init(phoenixArrowPf.gameObject, 2));
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