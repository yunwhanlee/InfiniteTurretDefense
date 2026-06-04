using System.Collections;
using UnityEngine;
using static SkillPoolManager;

public class ShadowPartner : MonoBehaviour
{
    Animator anim;
    Vector3 targetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnim(string animName, bool isFlipX)
    {
        anim.SetTrigger(animName);
    }

    public void Init(Vector3 pos, float duration)
    {
        targetPos = pos;
        StartCoroutine(CoRelease(duration));
    }

    /// <summary>
    /// 스킬 오브젝트 회수
    /// </summary>
    /// <param name="duration">지속시간</param>
    IEnumerator CoRelease(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 회수
        GM._.spm.ReleasePoolDics(SK_IDX.SK_ShadowPartner, gameObject);
    }
}
