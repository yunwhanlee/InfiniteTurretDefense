using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;
using static EffectPoolManager;
using static SkillPoolManager;

public class Enginner : Chara
{
    [Header("자식 변수")]
    public Transform shootTf;
    public Sprite missileSpr;

    // SK3. 포탑설치
    const int TURRET_TIME = 30;
    [SerializeField] float TurretTime;
    // SK4. 바주카
    const int BAZOOKA_TIME = 14;
    [SerializeField] float BazookaTime;
    // SK5. 화염방사
    const int FLAMESHOT_TIME = 32;
    [SerializeField] float FlameShotTime;
    // SK6. 유도탄
    const int HORMINGMISSILE_TIME = 9;
    [SerializeField] float HormingMissileTime;
    // SK7. 과부하
    const int OVERLOAD_MAX = 100;
    [SerializeField] int curOverLoadGuage;

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
            GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, missileSpr, isCritical);
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

            int dmg = Mathf.RoundToInt(Dmg * dmgPer);

            // 오브젝트 풀링리스트 관통샷 생성 및 초기화
            FireBall fireBall = GM._.spm.SpawnPoolDics(SK_IDX.SK_FireBall).GetComponent<FireBall>();
            fireBall.Init(shootTf.position, direction, dmg);
        }

        return isActive;
    }

    private void Skill3_MagicOrb()
    {
        const int gradeIdx = (int)CHR_GRADE.EPIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        int dmg = Mathf.RoundToInt(Dmg * dmgPer);

        // 매직오브 소환 이펙트
        Vector3 pos = new Vector3(transform.position.x + 0.35f, transform.position.y + 0.6f, transform.position.z);
        GM._.epm.SpawnPoolDics(EF_IDX.MagicOrbSpawnEF, pos);

        // 매직오브 소환
        MagicOrb magicOrb = GM._.spm.SpawnPoolDics(SK_IDX.SK_MagicOrb).GetComponent<MagicOrb>();
        magicOrb.Init(dmg, pos);
    }

    IEnumerator CorSkill4_IceBlade()
    {
        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];

        // 현재 등급의 스킬 데이터 에셋 가져오기
        SkillAsset skillAsset = CharaSkill.skillAssetArr[gradeIdx];

        float dmgPer = 0;
        int bladeCount = 0;

        // 인스펙터에 세팅된 ValueList를 순회하며 타입별로 값 가져오기
        foreach (var val in skillAsset.ValueList)
        {
            if (val.type == SkillValue.Type.SkillLv)
            {
                dmgPer = (val.def + val.unit * skillLv) * 0.01f;
            }
            else if (val.type == SkillValue.Type.GradeLv)
            {
                int gradeDiff = (int)Grade - (int)skillAsset.Grade;
                bladeCount = Mathf.RoundToInt(val.def + gradeDiff * val.unit);
            }
        }

        int dmg = Mathf.RoundToInt(Dmg * dmgPer);

        // 발사 간격 및 각도 계산
        const float angleInterval = 15f; 
        float startAngle = -((bladeCount - 1) * angleInterval / 2f);
        Vector3 dir = direction; // 방향이 바뀌지 않도록 발사시 방향 변수에 저장

        // 칼날얼음 순차적 생성
        for(int i = 0; i < bladeCount; i++)
        {
            float currentAngle = startAngle + (angleInterval * i);

            IceBlade iceBlade = GM._.spm.SpawnPoolDics(SK_IDX.SK_IceBlade).GetComponent<IceBlade>();
            iceBlade.Init(shootTf.position, dir, dmg, currentAngle);
            
            yield return WFS_0_1;
        }
    }

    private void Skill5_Tornado()
    {
        const int gradeIdx = (int)CHR_GRADE.LEGEND;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        Tornado tornado = GM._.spm.SpawnPoolDics(SK_IDX.SK_Tornado).GetComponent<Tornado>();
        tornado.Init(shootTf.position, direction, damage);
    }

    private void Skill6_Thunder()
    {
        const int gradeIdx = (int)CHR_GRADE.MYTHIC;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        Thunder thunder = GM._.spm.SpawnPoolDics(SK_IDX.SK_Thunder).GetComponent<Thunder>();
        Vector3 enemyPos = GetCurrentTargetEnemy().transform.position;
        thunder.Init(enemyPos, damage);
    }

    private void Skill7_Blizzard()
    {
        const int gradeIdx = (int)CHR_GRADE.PRIME;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        int damage = Mathf.RoundToInt(Dmg * dmgPer);

        // 이펙트
        GM._.epm.SpawnPoolDics(EF_IDX.BlizzardEF, transform.position, WFS_3);

        // 모든 적 공격
        StartCoroutine(CoBlizzardAttack(damage));
    }

    IEnumerator CoBlizzardAttack(int damage)
    {
        yield return WFS_1;

        GM._.emm.GetAllEnemies().ForEach(enemy => {
            enemy.OnHit(damage, false);
            enemy.Slow(5f); // 5초간 빙결 (슬로우)
        });
    }
#endregion
}
