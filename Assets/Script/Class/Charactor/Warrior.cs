using UnityEngine;

public class Warrior : Chara
{
    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 전사는 근접이라서 투사체 X 바로 타겟 공격
        if(enemy.State == Enemy.STATE.DEAD)
            return;
        
        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Skill1_Dmg();

        // 타겟 공격
        enemy.OnHit(damage);
    }

#region SKILL
    // 나머지 스킬 관련 함수 작성
#endregion
}
