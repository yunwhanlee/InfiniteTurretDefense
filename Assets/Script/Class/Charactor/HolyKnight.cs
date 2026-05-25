using UnityEngine;
using static Config;

public class HolyKnight : Chara
{
    // 빛의 보호막
    const int HOLY_GUARD_COOLTIME = 26;
    [SerializeField] float holyGuardTime = 0;

    // 빛 폭발
    const int HOLY_BURST_COOLTIME = 26;
    [SerializeField] float holyBurstTime = 0;

    // 빛의 치유
    const int HOLY_HEAL_COOLTIME = 23;
    [SerializeField] float holyHealTime = 0;

    // 빛의 아우라
    const int HOLY_AURA_COOLTIME = 47;
    [SerializeField] float holyAuraTime = 0;

    // 빛의 기둥
    const int HOLY_BEAM_COOLTIME = 39;
    [SerializeField] float holyBeamTime = 0;

    // 신의 심판
    const int HOLY_SMITE_COOLTIME = 69;
    [SerializeField] float holySmiteTime = 0;

    
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

    public override void Attack(Enemy enemy)
    {
        base.Attack(enemy);
    }

#region SKILL
    private void Skill2_HolyGuard() {
        
    }
    private void Skill3_HolyBurst() {
        
    }
    private void Skill4_HolyHeal() {
        
    }
    private void Skill5_HolyAura() {
        
    }
    private void Skill6_HolyBeam() {
        
    }
    private void Skill7_HolySmite() {
        
    }
#endregion
}