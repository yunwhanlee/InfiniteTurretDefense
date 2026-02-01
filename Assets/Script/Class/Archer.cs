using UnityEngine;

public class Archer : Chara
{
    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 투사체 발사
        GM._.msm.SpawnMissile(transform.position, direction, Dmg);
    }
}
