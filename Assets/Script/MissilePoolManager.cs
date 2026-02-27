using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



/// <summary>
/// 투사체 매니저 (스킬 포함)
/// </summary>
public class MissilePoolManager : MonoBehaviour
{
    // 오브젝트 풀링
    public Transform missileGroupTf;
    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}
    public Missile missilePref;

    void Awake()
    {
        // 일반 투사체 초기화
        pool = Init(100);
    }

#region POOL
    Missile Create() => Instantiate(missilePref, missileGroupTf);
    void OnGet(Missile missile) => missile.gameObject.SetActive(true);
    void OnRelease(Missile missile) => missile.gameObject.SetActive(false);
    void OnDelete(Missile missile) => Destroy(missile);

    /// <summary> 오브젝트 풀링 초기화 </summary>
    private ObjectPool<Missile> Init(int max)
    {
        return  new ObjectPool<Missile>(
            Create, OnGet, OnRelease, OnDelete, maxSize: max
        );
    }

    /// <summary> 오브젝트 풀링 생성 </summary>
    public void SpawnPool(Vector3 pos, Vector3 dir, int dmg, float angleOffset, bool isCritical)
    {
        Missile missile = pool.Get();
        missile.Init(pos, dir, angleOffset);
        missile.Dmg = dmg;
        missile.IsCritical = isCritical;
    }
#endregion
}
