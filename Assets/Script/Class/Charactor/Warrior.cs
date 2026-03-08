using System;
using System.Collections;
using UnityEngine;
using static Config;
using static EffectPoolManager;
using Random = UnityEngine.Random;

public class Warrior : Chara
{
    // 강타
    const int POWER_STRIKE_COOLTIME = 6;
    [SerializeField] float powerStrikeTime = 0;
    bool IsPowerStrikeActive {get => powerStrikeTime > POWER_STRIKE_COOLTIME;} // 강타 활성화 트리거
    // 격려
    const int CHEERUP_COOLTIME = 31;
    [SerializeField] float cheerUpTime = 0;
    // 휠윈드
    const float WHIRLWIND_RADIUS = 3;
    const int WHIRLWIND_COOLTIME = 17;
    [SerializeField] float whirlWindTime = 0;
    // 충격파
    const int SHOCKWAVE_COOLTIME = 57;
    [SerializeField] float shockWaveTime = 0;

    public override float AttackSpeed
    {
        get
        {
            // 추가 공격속도
            float extraVal = 0;

            // 스킬3. 버서커 (공격속도 증가)
            if(Grade >= CHR_GRADE.EPIC)
            {
                var (_, berserkerSpd) = Skill3_Berserker();
                extraVal = berserkerSpd;
            }

            return base.AttackSpeed + extraVal;
        }
    }


    protected void Update()
    {
        base.Update();

        // 강타 카운트팅만 => 처리는 Attack()에서
        if(Grade >= CHR_GRADE.RARE)
            powerStrikeTime += Time.deltaTime;

        // 격려
        if(Grade >= CHR_GRADE.LEGEND) {
            cheerUpTime += Time.deltaTime;
            if(cheerUpTime >= CHEERUP_COOLTIME) {
                Skill5_CheerUp();
                cheerUpTime = 0;
            }
        }

        // 휠원드
        if(Grade >= CHR_GRADE.MYTHIC) {
            whirlWindTime += Time.deltaTime;
            if(whirlWindTime >= WHIRLWIND_COOLTIME) {
                Skill6_WhirlWind();
                whirlWindTime = 0;
            }
        }
        // 충격파
        if(Grade >= CHR_GRADE.PRIME) {
            shockWaveTime += Time.deltaTime;
            if(shockWaveTime >= SHOCKWAVE_COOLTIME) {
                StartCoroutine(CorSkill7_ShockWave());
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

        // 스킬3. 버서커 (공격력 증가)
        if(Grade >= CHR_GRADE.EPIC)
        {
            var (berserkerDmg, _) = Skill3_Berserker();
            damage += berserkerDmg;
        }

        // 스킬2. 강타 활성화 경우
        if(Grade >= CHR_GRADE.RARE && IsPowerStrikeActive)
        {
            powerStrikeTime = 0;
            damage = Skill2_PowerStrike(damage); // 강타 데미지 추가 계산
            GM._.epm.SpawnPoolDics(EF_IDX.PowerStrikeEF, enemy.transform.position);
            enemy.OnHit(damage, isCritical); // 타겟 공격
        }
        else
        {
            // 스킬4. 이중공격 활성화 경우
            if(Grade >= CHR_GRADE.UNIQUE && Skill4_DoubleAttack())
            {
                // 이중 공격 코루틴 실행
                StartCoroutine(CorDoubleAttack(enemy, damage, isCritical));
            }
            // 일반공격
            else
            {
                GM._.epm.SpawnPoolDics(EF_IDX.SlashEF, enemy.transform.position); // 일반공격 이펙트
                enemy.OnHit(damage, isCritical); // 타겟 공격
            }
        }
    }

#region SKILL
    /// <summary> 강타 </summary>
    private int Skill2_PowerStrike(int dmg)
    {
        Debug.Log($"Skill2_PowerStrike():: dmg= {dmg}");
        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(dmg * dmgPer);
        return damage;
    }

    /// <summary> 광전사 : 성벽HP가 10%씩 낮아질때마다 공격력, 공속 배로 증가 (합연산) </summary>
    private (int dmg, float atkSpd) Skill3_Berserker()
    {
        const int gradeIdx = (int)CHR_GRADE.EPIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def; // 10(%)
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit; // 1(%)
        float extraPer = (defPer + unitPer * skillLv) * 0.01f;

        // 성벽 HP가 몇% 깎였는지 중첩횟수 게산
        float hpPer = (float)GM._.tower.Hp / GM._.tower.MaxHp;
        float lostHpPer = 1f - hpPer;
        int times = Mathf.FloorToInt(lostHpPer / 0.1f);
        extraPer *= times;

        // 공격력 증가
        int resDmg = Mathf.RoundToInt(Skill1_Dmg() * extraPer);
        // 공격속도 증가 (100% => 1증가 방식)
        float resAtkSpd = (float)Math.Round(extraPer, 1);

        // Debug.Log($"Skill3_Berserker():: extraPer={extraPer}, resDmg={resDmg}, resAtkSpd={resAtkSpd}");
        return (resDmg, resAtkSpd);
    }

    /// <summary> 이중공격 : 공격시 {0}%로 발동 트리거 </summary>
    private bool Skill4_DoubleAttack()
    {
        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def; // 5(%)
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit; // 0.5(%)
        float activePer = defPer + unitPer * skillLv;

        int rand = Random.Range(0, 100);

        // Debug.Log($"Skill4_DoubleAttack():: rand({rand}) <= activePer({activePer}) = {rand <= activePer}");
        return rand <= activePer;
    }

    /// <summary> 이중공격 코루틴 처리 함수 </summary>
    IEnumerator CorDoubleAttack(Enemy enemy, int damage, bool isCritical)
    {
        // 1타
        GM._.epm.SpawnPoolDics(EF_IDX.DoubleAttackEF, enemy.transform.position);
        enemy.OnHit(damage, isCritical);

        // 대기
        yield return WFS_0_2;

        // 2타 (이미 죽었다면 텍스트만 띄우고 나머지 처리 자동으로 안함)
        enemy.OnHit(damage, isCritical);
    }

    /// <summary> 격려 : 아군 공격력 % 증가 버프 </summary>
    private void Skill5_CheerUp()
    {
        GM._.epm.SpawnPoolDics(EF_IDX.CheerUpEF, transform.position); 
        StartCoroutine(CorCheerUp());
    }

    /// <summary> 격려 코루틴 처리 함수 </summary>
    IEnumerator CorCheerUp()
    {
        const int gradeIdx = (int)CHR_GRADE.LEGEND;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 공격증가률 (백분률)

        // 모든 캐릭터에게 버프 적용 및 이펙트 띄우기
        foreach (Chara chara in GM._.crm.curCharaList)
        {
            if (chara != null && chara.gameObject.activeSelf) // 활성화된 캐릭터만
            {
                chara.BuffDmgPer += dmgPer; // += 로 해야 버프 중첩 시 꼬이지 않음
                // 캐릭터 격려 버프 이펙트 띄우기
                GM._.epm.SpawnPoolDics(EF_IDX.RageAuraEF, chara.transform.position, WFS_5);
            }
        }

        yield return WFS_5;

        // 모든 캐릭터 공격력 원래대로 되돌리기
        foreach (Chara chara in GM._.crm.curCharaList)
        {
            // 🚨 [매우 중요] 5초 대기하는 동안 캐릭터가 합성되거나 팔려서 사라졌을 수 있으므로 null 체크 필수!
            if (chara != null) 
            {
                chara.BuffDmgPer -= dmgPer;
            }
        }
    }

    /// <summary> 휠윈드 </summary>
    private void Skill6_WhirlWind()
    {
        const int gradeIdx = (int)CHR_GRADE.MYTHIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 공격증가률 (백분률)

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            WHIRLWIND_RADIUS,
            Layer.ENEMY
        );

        foreach(Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            enemy.OnHit(Mathf.RoundToInt(Skill1_Dmg() * dmgPer), false);
        }

        // 이펙트
        GM._.epm.SpawnPoolDics(EF_IDX.WheelWindEF, transform.position);
    }

    /// <summary> 충격파 </summary>
    IEnumerator CorSkill7_ShockWave()
    {
        const int gradeIdx = (int)CHR_GRADE.PRIME;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (float)Math.Round((defPer + unitPer * skillLv) * 0.01f, 1);

        // 데미지
        int damage = Mathf.RoundToInt(Skill1_Dmg() * dmgPer);

        yield return WFS_1; // 이펙트 마법구현 대기시간

        // 모든 적 공격
        Transform enemyGroupTf = GM._.emm.enemyGroupTf;
        for (int i = enemyGroupTf.childCount - 1; i >= 0; i--)
        {
            Transform child = enemyGroupTf.GetChild(i);
            Enemy enemy = child.GetComponent<Enemy>();

            if (enemy != null && enemy.IsAlive)
            {
                enemy.OnHit(damage, false);
            }
        }

        // 이펙트 (3초뒤 회수)
        GM._.epm.SpawnPoolDics(EF_IDX.ShockWaveEF, transform.position, deleteSec: WFS_3);
    }
#endregion

    // (기즈모) 휠윈드 공격범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, WHIRLWIND_RADIUS);
    }

}
