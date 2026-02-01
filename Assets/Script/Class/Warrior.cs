using UnityEngine;

public class Warrior : Chara
{
    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 전사는 근접이라서 투사체 X 바로 타겟 공격
        if(enemy.State == Enemy.STATE.DEAD)
            return;
        enemy.OnHit(Dmg);
    }
}
