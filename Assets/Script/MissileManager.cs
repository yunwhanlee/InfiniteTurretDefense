using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum MISSILE_IDX
{
    PassArrow // 관통샷 (궁수스킬)
}

public class MissileManager : MonoBehaviour
{
    // 오브젝트 풀링
    public Transform missileGroupTf;

    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}
    List<IObjectPool<GameObject>> poolList;     public List<IObjectPool<GameObject>> PoolList {get => poolList;}

    public Missile missilePref;
    public PassArrow passArrowPref;

    void Awake()
    {
        // 일반 투사체 초기화
        pool = InitMissile(100);

        // 스킬 투사체 리스트 따로 관리필요한 것들 초기화
        poolList = new List<IObjectPool<GameObject>>();
        poolList.Add(InitObj(passArrowPref.gameObject, 2));
    }

#region POOL
    Missile Create() => Instantiate(missilePref, missileGroupTf);
    void OnGet(Missile missile) => missile.gameObject.SetActive(true);
    void OnRelease(Missile missile) => missile.gameObject.SetActive(false);
    void OnDelete(Missile missile) => Destroy(missile);

    /// <summary> 오브젝트 풀링 초기화 </summary>
    private ObjectPool<Missile> InitMissile(int max)
    {
        return  new ObjectPool<Missile>(
            Create, OnGet, OnRelease, OnDelete, maxSize: max
        );
    }

    /// <summary> 오브젝트 풀링 생성 </summary>
    public void SpawnMissile(Vector3 pos, Vector3 dir, int dmg, float angleOffset, bool isCritical)
    {
        Missile missile = pool.Get();
        missile.Init(pos, dir, angleOffset);
        missile.Dmg = dmg;
        missile.IsCritical = isCritical;
    }
#endregion
#region POOL LIST
    GameObject Create(GameObject obj) => Instantiate(obj, missileGroupTf);
    void OnGet(GameObject obj) => obj.gameObject.SetActive(true);
    void OnRelease(GameObject obj) => obj.gameObject.SetActive(false);
    void OnDelete(GameObject obj) => Destroy(obj);

    /// <summary> 오브젝트 풀링리스트 초기화 </summary>
    private ObjectPool<GameObject> InitObj(GameObject pref, int max)
    {
        return new ObjectPool<GameObject>(()=> 
            Create(pref), OnGet, OnRelease, OnDelete, maxSize: max
        );
    }

    /// <summary> 오브젝트 풀링리스트 생성 </summary>
    public GameObject SpawnMissilePoolList(MISSILE_IDX enumIdx) => poolList[(int)enumIdx].Get();
    /// <summary> 오브젝트 풀링리스트 회수 </summary>
    public void ReleaseMissilePoolList(MISSILE_IDX enumIdx, GameObject obj) => poolList[(int)enumIdx].Release(obj);
#endregion
}
