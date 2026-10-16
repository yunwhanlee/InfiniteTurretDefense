using System;
using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class ShadowPartner : MonoBehaviour
{
    SpriteRenderer sprRdr;
    Animator anim;
    Ninza ninzaParent; // 부모 닌자캐릭터

    const float OFFSET_X = 0.2f;
    const float OFFSET_Y = 0.1f;


    void Start()
    {
        anim = GetComponent<Animator>();
        sprRdr = GetComponent<SpriteRenderer>();
    }

    public void PlayAnim()
    {
        sprRdr.flipX = ninzaParent.sprRdr.flipX;

        Vector3 pos = ninzaParent.transform.position;
        float offsetX = sprRdr.flipX? OFFSET_X : -OFFSET_X;

        transform.position = new Vector3(pos.x + offsetX, pos.y, pos.z);

        anim.SetTrigger("IsAttack");
    }

    public void Init(Ninza ninza, float duration)
    {
        ninzaParent = ninza;

        Vector3 pos = ninzaParent.transform.position;
        float offsetX = ninzaParent.sprRdr.flipX? OFFSET_X : -OFFSET_X;

        transform.position = new Vector3(pos.x + offsetX, pos.y + OFFSET_Y, pos.z);

        StartCoroutine(CoRelease(duration));
    }

    private void OnDestroy() {
        ninzaParent.IsActiveShadowPartner = false;
        ninzaParent.shadowPartner = null;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_ShadowPartner, gameObject); // 회수
    }

#region FUNC
    /// <summary>
    /// 쉐도우파트너 추가공격 (외부)
    /// </summary>
    public void Attack(Action callback)
    {
        StartCoroutine(CoShadowPartnerAttack(callback));
    }

    /// <summary>
    /// 쉐도우파트너 추가공격 (내부 코루틴)
    /// </summary>
    IEnumerator CoShadowPartnerAttack(Action callback)
    {
        yield return Config.WFS_0_2;
        yield return Config.WFS_0_1;
        callback.Invoke();
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    /// <param name="duration">지속시간</param>
    IEnumerator CoRelease(float duration)
    {
        // 지속시간 대기 후
        yield return new WaitForSeconds(duration);
        ninzaParent.IsActiveShadowPartner = false;
        ninzaParent.shadowPartner = null;
        GM._.spm.ReleasePoolDics(SK_IDX.SK_ShadowPartner, gameObject); // 회수
    }
#endregion
}
