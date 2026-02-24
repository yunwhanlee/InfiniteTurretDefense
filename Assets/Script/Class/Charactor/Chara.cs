using System;
using UnityEngine; // MAIN_BUTA
using static Config;

/// <summary>
/// 캐릭터 (부모) :: CharaCard에 있는 데이터로부터 생성시 데이터 모두 적용
/// </summary>
public abstract class Chara : MonoBehaviour
{
    // 외부 클래스
    public TargetFinder targetFinder;
    public Missile missile;

    // Value (Read Only)
    public bool isLocked; // 잠김 여부
    public CHR_PLACE place; // 배치 위치
    public GameObject rangeCircle; // 클릭시 보이는 공격범위 원
    public Sprite defaultSpr; // 캐릭터 아이콘 이미지
    public Vector3 direction;

    // 저장 데이터 로드
    public CHR_CATE Cate {get; private set;}
    public CHR_GRADE Grade {get; private set;}
    public int[] SkillLvArr {get; private set;}

    // 데이터 에셋 DB 가져오기
    public int Dmg {get; private set;}
    public float DmgUpgUnit {get; private set;} // 스킬1 업그레이드 공격력 단위 증가량
    public float AttackSpeed {get; private set;}
    public float Range {get; private set;}
    public float CritPer {get; protected set;}
    public float CritDmgPer {get; protected set;}
    public CharaSkillAsset CharaSkill {get; private set;} // 스킬 데이터 에셋

    float time = 0;
    SpriteRenderer sprRdr;
    Animator anim;

    // 최적화를 위한 프로퍼티 블록 변수
    MaterialPropertyBlock mtPropBlock;

    // 쉐이더의 Color 프로퍼티 레퍼런스 ID 캐싱 (문자열 검색보다 훨씬 빠름)
    private static readonly int OutlineColorId = Shader.PropertyToID("_Color");

    protected void Awake()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rangeCircle.SetActive(false);
        defaultSpr = GetComponent<SpriteRenderer>().sprite; // Default 캐릭터 이미지 넣기

        // PropertyBlock 초기화 및 머테리얼 기본 설정
        mtPropBlock = new MaterialPropertyBlock();
        SetOutline(false); // 아웃라인 비활성화 (셰이더)
    }

    protected void Update()
    {
        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        time += Time.deltaTime;

        // 공격
        if(time > GetAttackPerSecond(AttackSpeed))
        {
            Attack(target);
            time = 0;
        }
    }

#region FUNC
    public void Init(CharaDataAsset charaDataAsset, UserCharaData userData)
    {
        const int SCALE_UNIT = 2; // 범위 원 스케일 단위

        // 저장 데이터 불러오기
        Cate = userData.cate;
        Grade = userData.grade;
        SkillLvArr = userData.skillLvArr;

        // 현재등급 에셋 데이터 불러오기 (DB)
        Dmg = charaDataAsset.baseDmg;
        DmgUpgUnit = charaDataAsset.dmgUpgUnit;
        AttackSpeed = charaDataAsset.baseAttackSpeed;
        Range = charaDataAsset.baseRange;
        CritPer = 0;
        CritDmgPer = 1.5f;
        CharaSkill = charaDataAsset.charaSkillAsset;

        time = AttackSpeed; // 공속 적용
        targetFinder.radius = Range; // 범위 적용
        rangeCircle.transform.localScale = Vector3.one * SCALE_UNIT * Range; // 범위 스케일 조정

        Debug.Log($"Init():: Dmg({Dmg}) = charaDataAsset.baseDmg({charaDataAsset.baseDmg})");
    }

    public virtual void Attack(Enemy enemy)
    {
        // Debug.Log($"Attack():: {enemy.name}, HP: {enemy.hp}");
        direction = (enemy.targetSpotTf.position - transform.position).normalized;
        sprRdr.flipX = direction.x < 0;
        anim.SetTrigger("IsAttack");

        // 이후 공격방식은 각각 자식클래스에서 오버라이딩으로 추가할 것!
    }

    /// <summary>
    /// 1초당 공격 속도단위를 실제 공격속도로 변환
    /// </summary>
    /// <param name="speed">공격속도 단위 (예) 1.5 → 1초당 1.5번 공격</param>
    public float GetAttackPerSecond(float speed)
    {
        return (float)Math.Round(1f / speed * 1000f) / 1000f;
    }

    /// <summary>
    /// 선택시 캐릭터 아웃라인 
    /// </summary>
    public void SetOutline(bool active)
    {
        // 현재 렌더러의 상태(프로퍼티)를 블록에 가져옵니다.
        sprRdr.GetPropertyBlock(mtPropBlock);

        int alpha = active ? 1 : 0;

        // 블록의 "_Color" 값을 변경하여 아웃라인 표시
        mtPropBlock.SetColor(OutlineColorId, new Color(1, 1, 0, alpha));

        // 변경된 블록을 렌더러에 다시 반영
        sprRdr.SetPropertyBlock(mtPropBlock);
    }
#endregion
#region SKILL
    /// <summary>
    /// 스킬레벨업 시 호출하여 데이터 동기화 업데이트
    /// </summary>
    /// <param name="grade"></param>
    public void LevelUpSkill(CHR_GRADE grade)
    {
        SkillLvArr[(int)grade]++;
    }
    
    /// <summary> 스킬1. 평타 (통일) </summary>
    /// <param name="gradeUnitArr">등급에 따른 공격력 증가비율 배열</param>
    /// <returns>스킬레벨에 따른 데미지</returns>
    public int Skill1_Dmg()
    {
        int skillLv = SkillLvArr[(int)CHR_GRADE.NORMAL];
        int damage = Dmg + Mathf.RoundToInt(Dmg * DmgUpgUnit * skillLv);

        // 치명타 확률
        int random = UnityEngine.Random.Range(0, 100);
        Debug.Log($"Skill1_Dmg():: {random} <= {CritPer} = {random <= CritPer}, CritDmgPer={CritDmgPer}");
        if(random <= CritPer)
        {
            damage = Mathf.RoundToInt(damage * CritDmgPer);
        }
        
        return Mathf.RoundToInt(damage);
    }
#endregion
}
