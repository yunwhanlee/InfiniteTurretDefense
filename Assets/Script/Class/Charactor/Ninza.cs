using System;
using System.Collections;
using UnityEngine;
using static Config;
using static SkillPoolManager;
using Random = UnityEngine.Random;

public class Ninza : Chara
{
    [Header("자식 변수")]
    public Transform shootTf;
    public Sprite missileSpr;

    // 스킬4 풍차수리검
    const int STORM_SHURIKEN_COOLTIME = 21;
    [SerializeField] float stormShurikenTime = 0;

    // 스킬6 쉐도우 파트너
    public bool IsActiveShadowPartner;
    public ShadowPartner shadowPartner;
    const int SHADOW_PARTNER_COOLTIME = 57;
    [SerializeField] bool isActiveShadowPartner; // 쉐도우 파트너 발동여부
    [SerializeField] float shadowPartnerTime = 0;

    // 스킬7 칼춤
    const int BLADE_DANCE_COOLTIME = 63;
    [SerializeField] float bladeDanceTime = 0;

    protected void Update()
    {
        base.Update();

        // 스킬4 풍차수리검
        if(Grade >= CHR_GRADE.UNIQUE) {
            stormShurikenTime += Time.deltaTime;
            if(stormShurikenTime >= STORM_SHURIKEN_COOLTIME) {
                Skill4_StormShuriken();
                stormShurikenTime = 0;

                // 쉐도우 파트너 추가 공격
                if(IsActiveShadowPartner)
                {
                    shadowPartner.PlayAnim();
                    shadowPartner.Attack(
                        () => Skill4_StormShuriken(decDmgPer: 0.6f)
                    );
                }
            }
        }

        // 스킬6 쉐도우 파트너
        if(Grade >= CHR_GRADE.MYTHIC) {
            shadowPartnerTime += Time.deltaTime;
            if(shadowPartnerTime >= SHADOW_PARTNER_COOLTIME) {
                Skill6_ShadowPartner();
                IsActiveShadowPartner = true;
                shadowPartnerTime = 0;
            }
        }

        // 스킬7 칼춤
        if(Grade >= CHR_GRADE.PRIME) {
            bladeDanceTime += Time.deltaTime;
            if(bladeDanceTime >= BLADE_DANCE_COOLTIME) {
                Skill7_BladeDance();
                bladeDanceTime = 0;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy); // 공격 모션

        // 치명타 및 데미지 확률 설정
        CritPer = 0;
        CritPer += Skill3_Critical();
        CritDmgPer = 1.5f;
        CritDmgPer += Skill5_CriticalDamage();

        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Dmg;

        // 치명타 확률 적용
        bool isCritical = false;
        if(CritPer > 0)
        {
            int random = Random.Range(0, 100);
            isCritical = random <= CritPer;
            if(isCritical)
                damage = Mathf.RoundToInt(damage * CritDmgPer);
        }

        bool isActiveSkill = false; // 액티브 스킬 발동 여부

        // 더블 쓰로우
        if(Grade >= CHR_GRADE.RARE)
        {
            isActiveSkill = Skill2_DoubleThrow(damage, isCritical);

            // 쉐도우 파트너 추가 공격
            if(IsActiveShadowPartner)
            {
                shadowPartner.PlayAnim();
                shadowPartner.Attack(
                    () => Skill2_DoubleThrow(Mathf.RoundToInt(damage * 0.6f), isCritical, true)
                );

            }
        }

        if(!isActiveSkill)
        {
            // 투사체 발사
            GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, missileSpr, isCritical);

            // 쉐도우 파트너 추가 공격
            if(IsActiveShadowPartner)
            {
                shadowPartner.PlayAnim();
                shadowPartner.Attack(
                    () => GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, missileSpr, isCritical)
                );
            }
        }
    }

    /// <summary>
    /// 쉐도우파트너 추가공격
    /// </summary>
    public IEnumerator CoShadowPartnerAttack(Action callback)
    {
        yield return WFS_0_5;
        callback.Invoke();
    }

#region SKILL
    /// <summary>
    /// 더블 쓰로우
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="isCritical">치명타 여부</param>
    /// <param name="isActive">무조건 발동</param>
    /// <returns>발동 여부</returns>
    private bool Skill2_DoubleThrow(int damage, bool isCritical, bool isActive = false)
    {
        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def; // 5%
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit; // 0.5%

        float percent = defPer + unitPer * skillLv;
        percent *= 10; // unit 소수점단위 정수로 올리기

        int random = Random.Range(0, 1000);
        if(isActive || random <= percent)
        {
            Debug.Log("Skill2_DoubleThrow():: 발동!");
            DoubleThrow doubleThrow = GM._.spm.SpawnPoolDics(SK_IDX.SK_DoubleThrow).GetComponent<DoubleThrow>();
            doubleThrow.Init(shootTf.position, direction, damage, isCritical, missileSpr);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 크리티컬 샷
    /// </summary>
    /// <returns>발동확률</returns>
    private float Skill3_Critical()
    {
        if(Grade < CHR_GRADE.EPIC)
            return 0;

        const int gradeIdx = (int)CHR_GRADE.EPIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;

        return defPer + unitPer * skillLv;
    }

    /// <summary>
    /// 풍차수리검
    /// </summary>
    private void Skill4_StormShuriken(float decDmgPer = 1.0f)
    {
        if(Grade < CHR_GRADE.UNIQUE)
            return;
        
        const int STIKE_CNT_IDX = 0;

        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];
        var skillValList = CharaSkill.skillAssetArr[gradeIdx].ValueList;

        // {0} 데미지
        float defPer = skillValList[STIKE_CNT_IDX].def;
        float unitPer = skillValList[STIKE_CNT_IDX].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Dmg * dmgPer * decDmgPer);

        // 오브젝트 풀링리스트 관통샷 생성 및 초기화
        StormShuriken stormShuriken = GM._.spm.SpawnPoolDics(SK_IDX.SK_StormShuriken).GetComponent<StormShuriken>();
        stormShuriken.Init(shootTf.position, direction, damage);
    }

    /// <summary>
    /// 크리티컬 데미지
    /// </summary>
    private float Skill5_CriticalDamage()
    {
        if(Grade < CHR_GRADE.LEGEND)
            return 0;

        const int gradeIdx = (int)CHR_GRADE.LEGEND;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float result = (float)Math.Round((defPer + unitPer * skillLv) * 0.01f, 1);
        // Debug.Log($"Skill5_CriticalDamage():: skillLv= {skillLv}, unitPer={unitPer}, result= {result}");

        return result; // 백분률
    }

    /// <summary>
    /// 쉐도우 파트너 소환
    /// </summary>
    private void Skill6_ShadowPartner()
    {
        const int gradeIdx = (int)CHR_GRADE.MYTHIC;
        int skillLv = SkillLvArr[gradeIdx];

        // {0} 지속시간
        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float duration = defPer + unitPer * skillLv;

        //쉐도우파트너 소환
        shadowPartner = GM._.spm.SpawnPoolDics(SK_IDX.SK_ShadowPartner).GetComponent<ShadowPartner>();
        shadowPartner.Init(this, duration);
    }

    private void Skill7_BladeDance()
    {
        const int DMG = 0;

        const int gradeIdx = (int)CHR_GRADE.PRIME;
        int skillLv = SkillLvArr[gradeIdx];
        var skillValList = CharaSkill.skillAssetArr[gradeIdx].ValueList;

        // {0} 데미지
        float defPer = skillValList[DMG].def;
        float unitPer = skillValList[DMG].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        int dmg = Mathf.RoundToInt(Dmg * dmgPer);

        //TODO 칼춤 처리
    }
#endregion
}
