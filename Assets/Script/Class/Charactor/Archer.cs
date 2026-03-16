using System;
using UnityEngine;
using static Config;
using Random = UnityEngine.Random;
using static SkillPoolManager;
using System.Collections;

public class Archer : Chara
{
    public Transform shootTf;

    // 관통샷
    const int PASS_ARROW_COOLTIME = 10;
    [SerializeField] float passArrowTime = 0;
    // 화살비
    const int ARROW_RAIN_COOLTIME = 35;
    [SerializeField] float arrowRainTime = 0;
    // 불사조 화살
    const int PHOENIX_ARROW_COOLTIME = 63;
    [SerializeField] float phoenixArrowTime = 0;

    protected void Update()
    {
        base.Update();

        // 스킬4 관통샷
        if(Grade >= CHR_GRADE.UNIQUE) {
            passArrowTime += Time.deltaTime;
            if(passArrowTime >= PASS_ARROW_COOLTIME) {
                Skill4_PassArrow();
                passArrowTime = 0;
            }
        }

        // 스킬5 화살비
        if(Grade >= CHR_GRADE.MYTHIC) {
            arrowRainTime += Time.deltaTime;
            if(arrowRainTime > ARROW_RAIN_COOLTIME) {
                Skill6_ArrowRain();
                arrowRainTime = 0;
            }
        }

        // 스킬7 불사조 화살
        if(Grade >= CHR_GRADE.PRIME) {
            phoenixArrowTime += Time.deltaTime;
            if(phoenixArrowTime > PHOENIX_ARROW_COOLTIME) {
                Skill7_PhoenixArrow();
                phoenixArrowTime = 0;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

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
        GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, isCritical);
        Skill2_MultiShot(damage, isCritical);
    }

#region SKILL 
    /// <summary>
    /// 멀티샷
    /// </summary>
    /// <param name="damage">데미지</param>
    private void Skill2_MultiShot(int damage, bool isCritical)
    {
        if(Grade < CHR_GRADE.RARE)
            return;

        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def; // 5%
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit; // 0.5%

        float percent = defPer + unitPer * skillLv;
        percent *= 10; // 소수점단위 정수로 올리기

        int random = Random.Range(0, 1000);
        if(random < percent)
        {
            Vector3 pos = shootTf.position;

            switch (Grade)
            {
                case CHR_GRADE.RARE:
                case CHR_GRADE.EPIC:
                    GM._.mpm.SpawnPool(pos, direction, damage, -22.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +22.5f, isCritical);
                    break;
                case CHR_GRADE.UNIQUE:
                case CHR_GRADE.LEGEND:
                    GM._.mpm.SpawnPool(pos, direction, damage, -22.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +22.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, -45, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +45, isCritical);
                    break;
                case CHR_GRADE.MYTHIC:
                case CHR_GRADE.PRIME:
                    GM._.mpm.SpawnPool(pos, direction, damage, -22.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +22.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, -45, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +45, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, -67.5f, isCritical);
                    GM._.mpm.SpawnPool(pos, direction, damage, +67.5f, isCritical);
                    break;
            }
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
    /// 관통샷
    /// </summary>
    private void Skill4_PassArrow()
    {
        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        // 오브젝트 풀링리스트 관통샷 생성 및 초기화
        PassArrow passArrow = GM._.spm.SpawnPoolDics(SK_IDX.SK_PassArrow).GetComponent<PassArrow>();
        passArrow.Init(transform.position, direction, damage);
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
    /// 화살비
    /// </summary>
    private void Skill6_ArrowRain()
    {
        const int gradeIdx = (int)CHR_GRADE.MYTHIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (float)Math.Round((defPer + unitPer * skillLv) * 0.01f, 1);

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        ArrowRain arrowRain = GM._.spm.SpawnPoolDics(SK_IDX.SK_ArrowRain).GetComponent<ArrowRain>();
        arrowRain.Init(damage);
    }

    /// <summary>
    /// 불사조 화살
    /// </summary>
    private void Skill7_PhoenixArrow()
    {
        const int gradeIdx = (int)CHR_GRADE.PRIME;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (float)Math.Round((defPer + unitPer * skillLv) * 0.01f, 1);

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        PhoenixArrow phoenix = GM._.spm.SpawnPoolDics(SK_IDX.SK_PhoenixArrow).GetComponent<PhoenixArrow>();
        phoenix.Init(transform.position, direction, damage);
    }
#endregion
}