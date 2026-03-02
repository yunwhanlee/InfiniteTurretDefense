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
    }

    // 오브젝트
    public GameObject slashEF;
    public GameObject powerStrikeEF;
    public GameObject doubleAttackEF;

    // 오브젝트 풀링
    public Transform effectGroupTf;
    Dictionary<EF_IDX, IObjectPool<GameObject>> poolDics;     public Dictionary<EF_IDX, IObjectPool<GameObject>> PoolDics {get => poolDics;}
    WaitForSeconds releaseWaitSec;

    void Awake()
    {
        releaseWaitSec = new WaitForSeconds(1); // 오브젝트 회수 대기시간

        // 오브젝트 풀 등록
        poolDics = new Dictionary<EF_IDX, IObjectPool<GameObject>>();
        poolDics.Add(EF_IDX.SlashEF, Init(slashEF.gameObject, 20));
        poolDics.Add(EF_IDX.PowerStrikeEF, Init(powerStrikeEF.gameObject, 10));
        poolDics.Add(EF_IDX.DoubleAttackEF, Init(doubleAttackEF.gameObject, 10));
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
    public void SpawnPoolDics(EF_IDX enumIdx, Vector3 pos)
    {
        Debug.Log($"SpawnPoolDics():: {enumIdx}, {pos}");
        GameObject obj = poolDics[enumIdx].Get();
        obj.transform.position = pos;

        StartCoroutine(CoReleasePoolDics(enumIdx, obj));
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

    /// <summary> 코루틴 오브젝트 풀링리스트 대기회수 </summary>
    IEnumerator CoReleasePoolDics(EF_IDX enumIdx, GameObject obj)
    {
        yield return releaseWaitSec;
        poolDics[enumIdx].Release(obj);
        Debug.Log($"CoReleasePoolDics():: {enumIdx}, {obj}");
    }
#endregion
}
