using UnityEngine;
using static Config;
using static SkillPoolManager;

public class Magician : Chara
{
    public Transform shootTf;

    protected void Update()
    {
        base.Update();
    }

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy); // 공격 모션

        // 치명타 및 데미지 확률 설정
        CritPer = 0;
        // CritPer += 
        CritDmgPer = 1.5f;
        // CritDmgPer += 

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

        // 스킬2. 파이어볼
        bool isFireBallActive = SKill2_FireBall();

        // 일반 공격
        if(!isFireBallActive)
        {
            // 투사체 발사
            GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, isCritical);
        }
    }

#region SKILL
    private bool SKill2_FireBall()
    {
        if(Grade < CHR_GRADE.RARE)
            return false;
        
        const int ATK_PER = 0;
        const int DMG = 1;

        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];
        var skillValList = CharaSkill.skillAssetArr[gradeIdx].ValueList;

        // {0} 공격 확률
        float defPer = skillValList[ATK_PER].def;
        float unitPer = skillValList[ATK_PER].unit;
        float percent = defPer + unitPer * skillLv;
        percent *= 10; // unit 소수점단위 정수로 올리기

        int random = Random.Range(0, 1000);
        bool isActive = random < percent;
        Debug.Log($"SKill2_FireBall():: random({random}) < percent({percent}) = {isActive}");

        if(isActive)
        {
            // {1} 데미지
            defPer = skillValList[DMG].def;
            unitPer = skillValList[DMG].unit;
            float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

            int damage = Mathf.RoundToInt(Dmg * dmgPer);

            // 오브젝트 풀링리스트 관통샷 생성 및 초기화
            FireBall fireBall = GM._.spm.SpawnPoolDics(SK_IDX.SK_FireBall).GetComponent<FireBall>();
            fireBall.Init(shootTf.position, direction, damage);
        }

        return isActive;
    }
#endregion
}
