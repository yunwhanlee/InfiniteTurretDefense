using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 데미지텍스트 오브젝트풀링 매니저
/// </summary>
public class DmgTxtManager : MonoBehaviour
{
    public Transform groupTf;
    IObjectPool<DmgTxt> pool;   public IObjectPool<DmgTxt> Pool {get => pool;}

    public DmgTxt dmgTxtPref;

    void Start()
    {
        pool = new ObjectPool<DmgTxt>(
            Create, OnGet, OnRelease, OnDelete, maxSize: 100
        );
    }

#region POOL
    DmgTxt Create() => Instantiate(dmgTxtPref, groupTf);
    void OnGet(DmgTxt dmgTxt)
    {
        dmgTxt.gameObject.SetActive(true);
    }
    void OnRelease(DmgTxt dmgTxt)
    {
        dmgTxt.gameObject.SetActive(false);
    } 
    void OnDelete(DmgTxt obj) => Destroy(obj);
#endregion
#region FUNC
    /// <summary>
    /// 오브젝트 풀링 생성
    /// </summary>
    /// <param name="dmg">표시할 데미지</param>
    /// <param name="isCritical">크리티컬 여부</param>
    public void GetPool(int dmg, Vector2 pos, bool isCritical)
    {
        DmgTxt dmgTxt = pool.Get();
        dmgTxt.transform.position = pos;
        dmgTxt.PlayAnim(dmg, isCritical);
    }
    /// <summary>
    /// 오브젝트 풀링 회수
    /// </summary>
    /// <param name="dmgTxt">회수할 오브젝트</param>
    public void ReleasePool(DmgTxt dmgTxt)
    {
        pool.Release(dmgTxt);
    }
#endregion
}
