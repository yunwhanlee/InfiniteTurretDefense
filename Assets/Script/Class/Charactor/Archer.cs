using System;
using UnityEngine;
using static Config;
using Random = UnityEngine.Random;

// ARCHER

public class Archer : Chara
{
    const int PASS_ARROW_COOLTIME = 10;
    public float passArrowTime = 0;

    protected void Update()
    {
        base.Update();

        // 스킬4 관통샷
        if(Grade >= CHR_GRADE.UNIQUE)
        {
            passArrowTime += Time.deltaTime;
            if(passArrowTime >= PASS_ARROW_COOLTIME)
            {
                Skill4_PassArrow();
                passArrowTime = 0;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 치명타 확률 설정
        CritPer = 0;
        CritPer += Skill3_Critical();

        CritDmgPer = 1.5f;
        CritDmgPer += Skill5_CriticalDamage();

        // TODO 치명타 데미지 설정

        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Skill1_Dmg();

        // 투사체 발사
        GM._.msm.SpawnMissile(transform.position, direction, damage, 0);
        Skill2_MultiShot(damage);
    }

#region SKILL 
    /// <summary>
    /// 멀티샷
    /// </summary>
    /// <param name="damage">데미지</param>
    private void Skill2_MultiShot(int damage)
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
            switch (Grade)
            {
                case CHR_GRADE.RARE:
                case CHR_GRADE.EPIC:
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -22.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +22.5f);
                    break;
                case CHR_GRADE.UNIQUE:
                case CHR_GRADE.LEGEND:
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -22.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +22.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -45);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +45);
                    break;
                case CHR_GRADE.MYTHIC:
                case CHR_GRADE.PRIME:
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -22.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +22.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -45);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +45);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, -67.5f);
                    GM._.msm.SpawnMissile(transform.position, direction, damage, +67.5f);
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
        float dmgPercent = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Skill1_Dmg() * dmgPercent);

        // 오브젝트 풀링리스트 관통샷 생성 및 초기화
        PassArrow passArrow = GM._.msm.SpawnMissilePoolList(MISSILE_IDX.PassArrow).GetComponent<PassArrow>();
        passArrow.Init(transform.position, direction, damage);
    }

    private float Skill5_CriticalDamage()
    {
        if(Grade < CHR_GRADE.LEGEND)
            return 0;

        const int gradeIdx = (int)CHR_GRADE.LEGEND;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;

        float result = (float)Math.Round((defPer + unitPer * skillLv) * 0.01f, 1);
        Debug.Log($"Skill5_CriticalDamage():: skillLv= {skillLv}, unitPer={unitPer}, result= {result}");

        return result; // 백분률
    }
#endregion
}