using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static Config;

public class EffectPoolManager : MonoBehaviour
{
    public enum EF_IDX
    {
        //* 전사스킬
        SlashEF, // 베기
        PowerStrikeEF, // 강타
        DoubleAttackEF, // 이중공격
        CheerUpEF, // 격려
        RageAuraEF, // 격려받아 불타는 이펙트
    }

    // 오브젝트
    public GameObject SlashEF;
    public GameObject PowerStrikeEF;
    public GameObject DoubleAttackEF;
    public GameObject CheerUpEF;
    public GameObject RageAuraEF;

    // 오브젝트 풀링
    public Transform effectGroupTf;
    Dictionary<EF_IDX, IObjectPool<GameObject>> poolDics;     public Dictionary<EF_IDX, IObjectPool<GameObject>> PoolDics {get => poolDics;}

    void Awake()
    {
        // 오브젝트 풀 등록
        poolDics = new Dictionary<EF_IDX, IObjectPool<GameObject>>();
        poolDics.Add(EF_IDX.SlashEF, Init(SlashEF.gameObject, 20));
        poolDics.Add(EF_IDX.PowerStrikeEF, Init(PowerStrikeEF.gameObject, 10));
        poolDics.Add(EF_IDX.DoubleAttackEF, Init(DoubleAttackEF.gameObject, 10));
        poolDics.Add(EF_IDX.CheerUpEF, Init(CheerUpEF.gameObject, 5));
        poolDics.Add(EF_IDX.RageAuraEF, Init(RageAuraEF.gameObject, 5));
        // 여기에 추가
    }

#region POOL
    GameObject Create(GameObject obj) => Instantiate(obj, effectGroupTf);
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
    public void SpawnPoolDics(EF_IDX enumIdx, Vector3 pos, WaitForSeconds waitSec = null)
    {
        Debug.Log($"SpawnPoolDics():: {enumIdx}, {pos}");
        GameObject obj = poolDics[enumIdx].Get();
        obj.transform.position = pos;

        // 회수 대기시간 (Default : 1초)
        if(waitSec == null)
            waitSec = WFS_1;

        StartCoroutine(CoReleasePoolDics(enumIdx, obj, waitSec));
    }

    /// <summary> 코루틴 오브젝트 풀링리스트 대기회수 </summary>
    IEnumerator CoReleasePoolDics(EF_IDX enumIdx, GameObject obj, WaitForSeconds waitSec)
    {
        yield return waitSec;
        poolDics[enumIdx].Release(obj);
        Debug.Log($"CoReleasePoolDics():: {enumIdx}, {obj}, {waitSec}");
    }

    /// <summary> 코루틴 오브젝트 풀링리스트 대기생성 </summary>
    // IEnumerator CoSpawnPoolDics(EF_IDX enumIdx, Vector3 pos, WaitForSeconds waitSec)
    // {
    //     yield return waitSec;
    //     Debug.Log($"CoSpawnPoolDics():: {enumIdx}, {pos}, {waitSec}");
    //     GameObject obj = poolDics[enumIdx].Get();
    //     obj.transform.position = pos;

    //     yield return CoReleasePoolDics(enumIdx, obj);
    // }
#endregion
}
