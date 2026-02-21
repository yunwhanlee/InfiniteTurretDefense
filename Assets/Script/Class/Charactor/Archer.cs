using UnityEngine;
using static Config;

public class Archer : Chara
{
    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 치명타 확률 설정
        CritPer = 0;
        CritPer += Skill3_Critical();

        // TODO 치명타 데미지 설정

        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Skill1_Dmg();

        // 투사체 발사
        GM._.msm.SpawnMissile(transform.position, direction, damage, 0);
        Skill2_MultiShot(damage);
    }

#region SKILL
    // 나머지 스킬 관련 함수 작성
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
#endregion
}