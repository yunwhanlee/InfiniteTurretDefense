using UnityEngine;
using static Config;
using static EffectPoolManager;
using static SkillPoolManager;

/// <summary>
/// 성기사
/// </summary>
public class HolyKnight : Chara
{
    // 빛의보호막
    const int HOLY_GUARD_COOLTIME = 5;
    [SerializeField] float holyGuardTime = 0;

    // 빛폭발
    const int HOLY_BURST_COOLTIME = 19;
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

    Tower tower;

    void Start()
    {
        tower = GameObject.Find("Tower").GetComponent<Tower>();
    }

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

        //* 빛폭발
        if(holyBurstTime >= HOLY_BURST_COOLTIME)
        {
            Skill3_HolyBurst(damage);
            holyBurstTime = 0;
            return;
        }

        //* 일반공격
        GM._.epm.SpawnPoolDics(EF_IDX.SmashEF, enemy.transform.position); // 일반공격 이펙트
        enemy.OnHit(damage, isCritical); // 타겟 공격
    }

    protected void Update()
    {
        base.Update();

        // 빛의 보호막
        if(Grade >= CHR_GRADE.RARE)
        {
            holyGuardTime += Time.deltaTime;
            if(holyGuardTime >= HOLY_GUARD_COOLTIME)
            {
                Skill2_HolyGuard(tower.HealVal);
                holyGuardTime = 0;
            }
        }
        // 빛 폭발
        if(Grade >= CHR_GRADE.EPIC)
        {
            holyBurstTime += Time.deltaTime;
            //? Attack()에서 처리
        }
        // 빛의 치유
        if(Grade >= CHR_GRADE.UNIQUE)
        {
            holyHealTime += Time.deltaTime;
            if(holyHealTime >= HOLY_HEAL_COOLTIME)
            {
                Skill4_HolyHeal(tower.HealVal);
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
    /// <param name="healVal">성벽 회복량</param>
    private void Skill2_HolyGuard(int healVal) {
        Debug.Log("Skill2_HolyGuard():: healVal= " + healVal);

        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        // {0} 성벽 회복력
        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float healPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        // 쉴드 추가
        int val = Mathf.RoundToInt(healVal * healPer);
        GM._.tower.Sheild = val;

        // 이펙트
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GM._.epm.SpawnPoolDics(EF_IDX.HolyGuardIconEF, pos);
    }

    /// <summary>
    /// 빛폭발
    /// </summary>
    private void Skill3_HolyBurst(int damage) {
        const int gradeIdx = (int)CHR_GRADE.EPIC;
        int skillLv = SkillLvArr[gradeIdx];

        // {0} 공격력 %
        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        int dmg = Mathf.RoundToInt(damage * dmgPer);

        HolyBurst holyBurst = GM._.spm.SpawnPoolDics(SK_IDX.SK_HolyBurst).GetComponent<HolyBurst>();
        Vector3 enemyPos = GetCurrentTargetEnemy().transform.position;
        holyBurst.Init(enemyPos, dmg);
    }

    /// <summary>
    /// 빛의치유
    /// </summary>
    private void Skill4_HolyHeal(int healVal) {
        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];

        // {0} 성벽 회복력
        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float healPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        // 체력 회복
        int val = Mathf.RoundToInt(healVal * healPer);
        tower.Hp += val;

        // 이펙트
        GM._.epm.SpawnPoolDics(EF_IDX.HolyHealEF, tower.transform.position, WFS_2);

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