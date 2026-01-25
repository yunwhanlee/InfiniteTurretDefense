using UnityEngine;
using UnityEngine.Pool;

public class MissileManager : MonoBehaviour
{
    // 오브젝트 풀링
    public Transform missileGroupTf;
    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}

    public Missile missilePref; 

    void Start()
    {
        pool = new ObjectPool<Missile>(
            Create, OnGet, OnRelease, OnDelete, maxSize: 50
        );
    }

#region OBJECT POOL
    Missile Create() => Instantiate(missilePref, missileGroupTf);

    void OnGet(Missile missile)
    {
        missile.gameObject.SetActive(true);
    }

    void OnRelease(Missile missile)
    {
        missile.gameObject.SetActive(false);
    }

    void OnDelete(Missile missile) => Destroy(missile);
#endregion
#region FUNC
    public void SpawnMissile(Vector3 pos, Vector3 dir, int dmg)
    {
        Missile missile = pool.Get();
        missile.Init(pos, dir);
        missile.Dmg = dmg;
    }
#endregion
}
