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
    const int TURRET_TIME = 35;
    [SerializeField] float TurretTime;
    // SK4. 바주카
    const int BAZOOKA_TIME = 16;
    [SerializeField] float BazookaTime;
    // SK5. 화염방사
    const int FLAMESHOT_TIME = 32;
    [SerializeField] float FlameShotTime;
    // SK6. 유도탄
    const int HORMING_TIME = 6;
    [SerializeField] float HormingMissileTime;
    // SK7. 과부하
    const int OVERLOAD_MAX = 100;
    [SerializeField] int curOverLoadGuage;

    protected void Update()
    {
        base.Update();

        // SK3. 포탑설치
        if(Grade >= CHR_GRADE.EPIC) {
            TurretTime += Time.deltaTime;
            if(TurretTime >= TURRET_TIME && targetFinder.CurrentTarget) {
                Skill3_Turret();
                TurretTime = 0;
            }
        }

        // SK4. 바주카
        if(Grade >= CHR_GRADE.UNIQUE) {
            BazookaTime += Time.deltaTime;
            if(BazookaTime >= BAZOOKA_TIME) {
                Skill4_Bazooka();
                BazookaTime = 0;
            }
        }

        // SK5. 화염방사
        if(Grade >= CHR_GRADE.LEGEND) {
            FlameShotTime += Time.deltaTime;
            if(FlameShotTime >= FLAMESHOT_TIME) {
                // Skill5_FlameShot();
                FlameShotTime = 0;
            }
        }

        // SK6. 유도탄
        if(Grade >= CHR_GRADE.MYTHIC) {
            HormingMissileTime += Time.deltaTime;
            if(HormingMissileTime >= HORMING_TIME) {
                // Skill6_HormingMissile();
                HormingMissileTime = 0;
            }
        }

        // Skill7_OverLoad();
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

        // 스킬2. 샷건
        bool isShotGunActive = SKill2_ShotGun(isCritical);

        // 일반 공격
        if(!isShotGunActive)
        {
            // 투사체 발사
            // Vector2 pos = new Vector2(sprRdr.flipX? -shootTf.position.x : shootTf.position.x, shootTf.position.y);
            GM._.mpm.SpawnPool(shootTf.position, direction, damage, 0, missileSpr, isCritical);
        }
    }

#region SKILL
    private bool SKill2_ShotGun(bool isCritical)
    {
        if(Grade < CHR_GRADE.RARE)
            return false;
        
        const int ATK_PER = 0; // 발동% IDX
        const int DMG = 1; // 데미지 IDX

        const int gradeIdx = (int)CHR_GRADE.RARE;
        int skillLv = SkillLvArr[gradeIdx];
        var skillValList = CharaSkill.skillAssetArr[gradeIdx].ValueList;

        // {0} 공격 확률
        float defPer = skillValList[ATK_PER].def;
        float unitPer = skillValList[ATK_PER].unit;
        float percent = defPer + unitPer * (int)Grade;
        percent *= 10; // unit 소수점단위 정수로 올리기

        int random = Random.Range(0, 1000);
        bool isActive = random < percent;
        Debug.Log($"SKill2_ShotGun():: random({random}) < percent({percent}) = {isActive}");

        if(isActive)
        {
            // {1} 데미지
            defPer = skillValList[DMG].def;
            unitPer = skillValList[DMG].unit;
            float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

            int dmg = Mathf.RoundToInt(Dmg * dmgPer);

            // 샷건 투사체 발사
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, -60, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, -45, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, -30, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, -15, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, 0, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, 15, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, 30, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, 45, missileSpr, isCritical);
            GM._.mpm.SpawnPool(shootTf.position, direction, dmg, 60, missileSpr, isCritical);
        }

        return isActive;
    }

    private void Skill3_Turret()
    {
        const int gradeIdx = (int)CHR_GRADE.EPIC;
        int skillLv = SkillLvArr[gradeIdx];

        // 포탑 공격력
        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률
        int turretDmg = Mathf.RoundToInt(Dmg * dmgPer);

        // 포탑 체력
        float def2Per = CharaSkill.skillAssetArr[gradeIdx].ValueList[1].def;
        float unit2Per = CharaSkill.skillAssetArr[gradeIdx].ValueList[1].unit;
        float hpPer = (def2Per + unit2Per * skillLv) * 0.01f;
        int turretHp = 10 + Mathf.RoundToInt(hpPer);

        //TODO 소환 이펙트

        // 고정된 중심점
        Vector3 center = new Vector3(0, -0.8f, 0);
        Vector3 targetPos = targetFinder.CurrentTarget.transform.position;
        const float DISTANCE = 2f;

        // 2. 상하좌우 4개의 설치 후보 위치를 배열로 만들기
        Vector3[] candidatePositions = new Vector3[4]
        {
            center + Vector3.up * DISTANCE,    // 상 (0, 1.2, 0)
            center + Vector3.down * DISTANCE,  // 하 (0, -2.8, 0)
            center + Vector3.left * DISTANCE,  // 좌 (-2, -0.8, 0)
            center + Vector3.right * DISTANCE  // 우 (2, -0.8, 0)
        };

        // 3. 4개의 위치 중 타겟(적)과 가장 가까운 위치 찾기
        Vector3 bestPos = candidatePositions[0];
        float minDistance = float.MaxValue; // 최솟값을 찾기 위해 가장 큰 값으로 초기화

        foreach (Vector3 pos in candidatePositions)
        {
            float distanceToTarget = Vector3.Distance(pos, targetPos);
            
            // 현재 검사하는 위치가 지금까지 발견한 위치보다 타겟과 가깝다면 갱신
            if (distanceToTarget < minDistance)
            {
                minDistance = distanceToTarget;
                bestPos = pos;
            }
        }

        // 4. 최종 결정된 가장 가까운 위치(bestPos)에 터렛 생성
        Turret turret = GM._.spm.SpawnPoolDics(SK_IDX.SK_Turret).GetComponent<Turret>();
        turret.Init(turretDmg, turretHp, bestPos);
    }

    private void Skill4_Bazooka()
    {
        const int gradeIdx = (int)CHR_GRADE.UNIQUE;
        int skillLv = SkillLvArr[gradeIdx];

        float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률

        int dmg = Mathf.RoundToInt(Dmg * defPer);

        // 미사일 생성
        Bazooka bazooka = GM._.spm.SpawnPoolDics(SK_IDX.SK_Bazooka).GetComponent<Bazooka>();
        bazooka.Init(shootTf.position, direction, dmg);
    }

    private void Skill5_FlameShot()
    {
        // const int gradeIdx = (int)CHR_GRADE.LEGEND;
        // int skillLv = SkillLvArr[gradeIdx];

        // float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        // float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        // float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        // int damage = Mathf.RoundToInt(Dmg * dmgPer);

        // Tornado tornado = GM._.spm.SpawnPoolDics(SK_IDX.SK_Tornado).GetComponent<Tornado>();
        // tornado.Init(shootTf.position, direction, damage);
    }

    private void Skill6_Horming()
    {
        // const int gradeIdx = (int)CHR_GRADE.MYTHIC;
        // int skillLv = SkillLvArr[gradeIdx];

        // float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        // float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        // float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        // int damage = Mathf.RoundToInt(Dmg * dmgPer);

        // Thunder thunder = GM._.spm.SpawnPoolDics(SK_IDX.SK_Thunder).GetComponent<Thunder>();
        // Vector3 enemyPos = GetCurrentTargetEnemy().transform.position;
        // thunder.Init(enemyPos, damage);
    }

    private void Skill7_Overload()
    {
        // const int gradeIdx = (int)CHR_GRADE.PRIME;
        // int skillLv = SkillLvArr[gradeIdx];

        // float defPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].def;
        // float unitPer = CharaSkill.skillAssetArr[gradeIdx].ValueList[0].unit;
        // float dmgPer = (defPer + unitPer * skillLv) * 0.01f; // 백분률화

        // int damage = Mathf.RoundToInt(Dmg * dmgPer);

        // // 이펙트
        // GM._.epm.SpawnPoolDics(EF_IDX.BlizzardEF, transform.position, WFS_3);

        // // 모든 적 공격
        // StartCoroutine(CoBlizzardAttack(damage));
    }

    // IEnumerator CoBlizzardAttack(int damage)
    // {
    //     yield return WFS_1;

    //     GM._.emm.GetAllEnemies().ForEach(enemy => {
    //         enemy.OnHit(damage, false);
    //         enemy.Slow(5f); // 5초간 빙결 (슬로우)
    //     });
    // }
#endregion
}
