using UnityEngine;
using static Config;

public class Warrior : Chara
{
    // 강타
    const int POWER_STRIKE_COOLTIME = 6;
    [SerializeField] float powerStrikeTime = 0;
    bool IsPowerStrikeActive {get => powerStrikeTime > POWER_STRIKE_COOLTIME;} // 강타 활성화 트리거
    // 격려
    const int WARCRY_COOLTIME = 0;
    [SerializeField] float warCryTime = 0;
    // 휠윈드
    const int WHIRLWIND_COOLTIME = 0;
    [SerializeField] float whirlWindTime = 0;
    // 충격파
    const int SHOCKWAVE_COOLTIME = 0;
    [SerializeField] float shockWaveTime = 0;

    protected void Update()
    {
        base.Update();

        // 강타
        powerStrikeTime += Time.deltaTime;

        // 격려
        if(Grade >= Config.CHR_GRADE.LEGEND) {
            warCryTime += Time.deltaTime;
            if(warCryTime >= WARCRY_COOLTIME) {
                Skill5_WarCry();
                warCryTime = 0;
            }
        }

        // 휠원드
        if(Grade >= Config.CHR_GRADE.MYTHIC) {
            whirlWindTime += Time.deltaTime;
            if(whirlWindTime >= WHIRLWIND_COOLTIME) {
                Skill6_WhirlWind();
                whirlWindTime = 0;
            }
        }
        // 충격파
        if(Grade >= Config.CHR_GRADE.PRIME) {
            shockWaveTime += Time.deltaTime;
            if(shockWaveTime >= SHOCKWAVE_COOLTIME) {
                Skill7_ShockWave();
                shockWaveTime = 0;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 전사는 근접이라서 투사체 X 바로 타겟 공격
        if(enemy.State == Enemy.STATE.DEAD)
            return;

        // 치명타 및 데미지 확률 설정
        CritPer = 0;
        // CritPer += 
        CritDmgPer = 1.5f;
        // CritDmgPer += 

        // 등급에따른 공격력 업글당 증가비율 배열
        int damage = Skill1_Dmg();

        // 치명타 확률 적용
        bool isCritical = false;
        if(CritPer > 0)
        {
            int random = Random.Range(0, 100);
            isCritical = random <= CritPer;
            if(isCritical)
                damage = Mathf.RoundToInt(damage * CritDmgPer);
        }

        // 스킬2. 강타 활성화 경우
        if(Grade < CHR_GRADE.RARE && IsPowerStrikeActive)
        {
            damage = Skill2_PowerStrike(damage); // 강타 데미지 추가 계산
            // TODO 강타 이펙트
        }
        else
        {
            // TODO 일반공격 이펙트
        }

        // 타겟 공격
        enemy.OnHit(damage, isCritical);
    }

#region SKILL
    /// <summary>
    /// 강타
    /// </summary>
    private int Skill2_PowerStrike(int dmg)
    {
        Debug.Log($"전사:: 강타! dmg= {dmg}");
        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(dmg * dmgPer);
        return damage;
    }

    /// <summary>
    /// 광전사
    /// </summary>
    private void Skill3_Berserker()
    {
        
    }

    /// <summary>
    /// 이중 공격
    /// </summary>
    private void Skill4_DoubleAttack()
    {
        
    }

    /// <summary>
    /// 격려
    /// </summary>
    private void Skill5_WarCry()
    {
        
    }

    /// <summary>
    /// 휠윈드
    /// </summary>
    private void Skill6_WhirlWind()
    {
        
    }

    /// <summary>
    /// 충격파
    /// </summary>
    private void Skill7_ShockWave()
    {
        
    }
#endregion
}
