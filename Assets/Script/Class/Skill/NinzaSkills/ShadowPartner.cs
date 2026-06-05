using System;
using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class ShadowPartner : MonoBehaviour
{
    Animator anim;
    Vector3 targetPos;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnim(string animName, bool isFlipX)
    {
        anim.SetTrigger(animName);
    }

    public void Init(Vector3 pos, float duration, Action onShadowPartnerFinish)
    {
        targetPos = pos;
        StartCoroutine(CoRelease(duration, onShadowPartnerFinish));
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    /// <param name="duration">지속시간</param>
    IEnumerator CoRelease(float duration, Action onShadowPartnerFinish)
    {
        // 지속시간 대기 후
        yield return new WaitForSeconds(duration);

        // callback: 쉐도우 파트너 상태 여부 False로 변경
        onShadowPartnerFinish.Invoke();

        // 회수
        GM._.spm.ReleasePoolDics(SK_IDX.SK_ShadowPartner, gameObject);
    }
}
