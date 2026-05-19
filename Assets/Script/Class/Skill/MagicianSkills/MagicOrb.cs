using System.Collections;
using UnityEngine;

public class MagicOrb : MonoBehaviour
{
    // 외부 클래스
    public TargetFinder targetFinder;

    public Sprite missileSpr;

    public int dmg;
    public float attackSpeed;

    float time = 0;

    void Update()
    {
        time += Time.deltaTime;

        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        if(time > attackSpeed)
        {
            // Attack
            Vector3 dir = target.transform.position.normalized;
            GM._.mpm.SpawnPool(transform.position, dir, dmg, 0, missileSpr, false);
            time = 0;
        }
    }

    public void Init(int dmg, Vector3 pos)
    {
        this.dmg = dmg;
        transform.position = pos;
        StartCoroutine(CorKeepTime());
    }

    IEnumerator CorKeepTime()
    {
        yield return new WaitForSeconds(20);

        // 회수
        GM._.spm.ReleasePoolDics(SkillPoolManager.SK_IDX.SK_MagicOrb, gameObject);
    }
}
