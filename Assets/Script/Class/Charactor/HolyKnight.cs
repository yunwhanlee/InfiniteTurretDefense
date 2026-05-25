using UnityEngine;
using static Config;
using static EffectPoolManager;

/// <summary>
/// 성기사
/// </summary>
public class HolyKnight : Chara
{
    // 빛의보호막
    const int HOLY_GUARD_COOLTIME = 26;
    [SerializeField] float holyGuardTime = 0;

    // 빛폭발
    const int HOLY_BURST_COOLTIME = 26;
    [SerializeField] float holyBurstTime = 0;

    // 빛의치유
    const int HOLY_HEAL_COOLTIME = 23;
    [SerializeField] float holyHealTime = 0;

    // 빛의아우라
    const int HOLY_AURA_COOLTIME = 47;
    [SerializeField] float holyAuraTime = 0;

    // 빛의기둥
    const int HOLY_BEAM_COOLTIME = 39;
    [SerializeField] float holyBeamTime = 0;

    // 신의심판
    const int HOLY_SMITE_COOLTIME = 69;
    [SerializeField] float holySmiteTime = 0;

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 근접이라서 투사체 X 바로 타겟 공격
        if(enemy.State == Enemy.STATE.DEAD)
            return;

        //TODO 치명타 및 데미지 확률 설정
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

        //* 일반공격
        GM._.epm.SpawnPoolDics(EF_IDX.SmashEF, enemy.transform.position); // 일반공격 이펙트
        enemy.OnHit(damage, isCritical); // 타겟 공격
    }

    protected void Update()
    {
        base.Update();

        // 빛의 가호
        if(Grade >= CHR_GRADE.RARE)
        {
            holyGuardTime += Time.deltaTime;
            if(holyGuardTime >= HOLY_GUARD_COOLTIME)
            {
                //TODO Skill2_HolyGuard();
                holyGuardTime = 0;
            }
        }
        // 빛 폭발
        if(Grade >= CHR_GRADE.EPIC)
        {
            holyBurstTime += Time.deltaTime;
            if(holyBurstTime >= HOLY_BURST_COOLTIME)
            {
                //TODO Skill3_HolyBurst();
                holyBurstTime = 0;
            }
        }
        // 빛의 치유
        if(Grade >= CHR_GRADE.UNIQUE)
        {
            holyHealTime += Time.deltaTime;
            if(holyHealTime >= HOLY_HEAL_COOLTIME)
            {
                //TODO Skill4_HolyHeal();
                holyHealTime = 0;
            }
        }
        // 빛의 장막
        if(Grade >= CHR_GRADE.LEGEND)
        {
            holyAuraTime += Time.deltaTime;
            if(holyAuraTime >= HOLY_AURA_COOLTIME)
            {
                //TODO Skill5_HolyAura();
                holyAuraTime = 0;
            }
        }
        // 빛의 기둥
        if(Grade >= CHR_GRADE.MYTHIC)
        {
            holyBeamTime += Time.deltaTime;
            if(holyBeamTime >= HOLY_BEAM_COOLTIME)
            {
                //TODO Skill6_HolyBeam();
                holyBeamTime = 0;
            }
        }
        // 신의 심판
        if(Grade >= CHR_GRADE.PRIME)
        {
            holySmiteTime += Time.deltaTime;
            if(holySmiteTime >= HOLY_SMITE_COOLTIME)
            {
                //TODO Skill7_HolySmite();
                holySmiteTime = 0;
            }
        }
    }

#region SKILL
    /// <summary>
    /// 빛의보호막
    /// </summary>
    private void Skill2_HolyGuard() {
        
    }
    /// <summary>
    /// 빛폭발
    /// </summary>
    private void Skill3_HolyBurst() {
        
    }
    /// <summary>
    /// 빛의치유
    /// </summary>
    private void Skill4_HolyHeal() {
        
    }
    /// <summary>
    /// 빛의아우라
    /// </summary>
    private void Skill5_HolyAura() {
        
    }
    /// <summary>
    /// 빛의기둥
    /// </summary>
    private void Skill6_HolyBeam() {
        
    }
    /// <summary>
    /// 신의심판
    /// </summary>
    private void Skill7_HolySmite() {
        
    }
#endregion
}