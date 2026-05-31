using System;
using UnityEngine;
using static Config;
using static SkillPoolManager;
using Random = UnityEngine.Random;

public class Ninza : Chara
{
    [Header("자식 변수")]
    public Transform shootTf;
    public Sprite missileSpr;

    // 스킬6 쉐도우 파트너
    const int SHADOW_PARTNER_COOLTIME = 57;
    [SerializeField] float shadowPartnerTime = 0;

    // 스킬7 칼춤
    const int BLADE_DANCE_COOLTIME = 63;
    [SerializeField] float bladeDanceTime = 0;

    protected void Update()
    {
        base.Update();

        // 스킬6 쉐도우 파트너
        if(Grade >= CHR_GRADE.MYTHIC) {
            shadowPartnerTime += Time.deltaTime;
            if(shadowPartnerTime >= SHADOW_PARTNER_COOLTIME) {
                Skill6_ShadowPartner();
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

        // 투사체 발사
        GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, missileSpr, isCritical);
        Skill2_DoubleThrow(damage, isCritical);
    }

#region SKILL
    /// <summary>
    /// 더블쓰로우
    /// </summary>
    /// <param name="damage">데미지</param>
    private void Skill2_DoubleThrow(int damage, bool isCritical)
    {
        if(Grade < CHR_GRADE.RARE)
            return;
        
        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def; // 5%
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit; // 0.5%

        float percent = defPer + unitPer * skillLv;
        percent *= 10; // unit 소수점단위 정수로 올리기

        int random = Random.Range(0, 1000);
        if(random <= percent)
        {
            //TODO 두번 공격
        }
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
    /// 급소 타격 : 같은 대상을 연속 x번 공격할 경우 추가 체력%데미지 타격
    /// </summary>
    private void Skill4_VitalStrike()
    {
        if(Grade < CHR_GRADE.UNIQUE)
            return;
        
        const int STIKE_CNT_IDX = 0;
        const int DMG_IDX = 1;

        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];
        var skillValList = CharaSkill.skillAssetArr[gradeIdx].ValueList;

        // {0} 타격 횟수
        int defCnt = (int)skillValList[STIKE_CNT_IDX].def;
        int unitCnt = (int)skillValList[STIKE_CNT_IDX].unit;
        int strikeCnt = defCnt + unitCnt * (int)Grade;

        // {1} 적 체력% 추가데미지
        float defPer = skillValList[DMG_IDX].def;
        float unitPer = skillValList[DMG_IDX].unit;
        float enemyHpDmgPer = defPer + unitPer * skillLv;
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

        //TODO 쉐도우파트너 소환
        // ShadowPartner shadowPartner = GM._.spm.SpawnPoolDics(SK_IDX.SK_HolyAura).GetComponent<HolyAura>();
        // Vector3 pos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        // shadowPartner.Init(pos, duration);
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
