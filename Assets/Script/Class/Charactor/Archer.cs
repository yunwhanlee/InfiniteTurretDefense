using UnityEngine;
using static Config;

public class Archer : Chara
{
    static readonly float[] gradeAtkUnitArr = new float[] {0.4f, 0.2f, 0.15f, 0.12f, 0.11f, 0.1f, 0.09f};

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Skill1_Dmg(gradeAtkUnitArr);

        // 투사체 발사
        GM._.msm.SpawnMissile(transform.position, direction, damage);
    }

#region SKILL
    // 나머지 스킬 관련 함수 작성
#endregion
}
