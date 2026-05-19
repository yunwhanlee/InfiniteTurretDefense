using System;
using Unity.VisualScripting;
using UnityEngine; // MAIN_BUTA
using static Config;

/// <summary>
/// 캐릭터 (부모) :: CharaCard에 있는 데이터로부터 생성시 데이터 모두 적용
/// </summary>
public abstract class Chara : MonoBehaviour
{
    public static readonly int DEF_CRIT = 0;
    public static readonly float DEF_CRITDMG = 1.5f;

    // 외부 클래스
    public TargetFinder targetFinder;

    // Value (Read Only)
    public bool isLocked; // 잠김 여부
    public GameObject rangeCircle; // 클릭시 보이는 공격범위 원
    public Sprite defaultSpr; // 캐릭터 아이콘 이미지
    public Vector3 direction;

    // 저장 데이터 로드
    public CHR_PLACE Place; // 배치 위치
    public CHR_CATE Cate {get; private set;}
    public CHR_GRADE Grade {get; private set;}
    [field: SerializeField] public int[] SkillLvArr {get; private set;}

    // 공격력
    int dmg;
    public float BuffDmgPer {get; set;}
    public int Dmg { get => Skill1_Dmg();}
    public float DmgUpgUnit {get; private set;} // 스킬1 업그레이드 공격력 단위 증가량
    // 공격속도
    public virtual float AttackSpeed {get; private set;}
    // 범위
    public float Range {get; private set;}
    // 크리티컬 확률
    public float CritPer {get; protected set;}
    // 크리티컬 데미지
    public float CritDmgPer {get; protected set;}
    // 스킬 데이터 에셋
    [field: SerializeField] public CharaSkillAsset CharaSkill {get; private set;}

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
        time += Time.deltaTime;

        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

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
        Debug.Log("Init():: " + charaDataAsset + ", " + userData);

        const int SCALE_UNIT = 2; // 범위 원 스케일 단위

        // 저장 데이터 불러오기
        Cate = userData.cate;
        Grade = userData.grade;
        Place = userData.place;
        SkillLvArr = userData.skillLvArr;

        // 현재등급 에셋 데이터 불러오기 (DB)
        dmg = charaDataAsset.baseDmg;
        DmgUpgUnit = charaDataAsset.dmgUpgUnit;
        AttackSpeed = charaDataAsset.baseAttackSpeed;
        Range = charaDataAsset.baseRange;
        CritPer = DEF_CRIT;
        CritDmgPer = DEF_CRITDMG;
        CharaSkill = charaDataAsset.charaSkillAsset;

        time = AttackSpeed; // 공속 적용
        targetFinder.radius = Range; // 범위 적용
        rangeCircle.transform.localScale = Vector3.one * SCALE_UNIT * Range; // 범위 스케일 조정

        Debug.Log($"Init():: dmg({dmg}) = charaDataAsset.baseDmg({charaDataAsset.baseDmg})");
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

    /// <summary>
    /// 현재 타겟중인 적 반환
    /// </summary>
    public Enemy GetCurrentTargetEnemy()
    {
        return targetFinder.CurrentTarget;
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
        Debug.Log($"Skill1_Dmg():: dmg({dmg}) * DmgUpgUnit({DmgUpgUnit}) * SkillLvArr[0]({SkillLvArr[(int)CHR_GRADE.NORMAL]}) => {Mathf.RoundToInt(dmg * DmgUpgUnit * SkillLvArr[(int)CHR_GRADE.NORMAL])}");

        int skillLv = SkillLvArr[(int)CHR_GRADE.NORMAL];
        int damage = dmg + Mathf.RoundToInt(dmg * DmgUpgUnit * skillLv);

        // 버프 공격력
        if(BuffDmgPer > 0)
        {
            int totalDmg = Mathf.RoundToInt(damage * (1 + BuffDmgPer));
            // 퍼센티지가 낮아서 버프를 받아도 값이 같다면 +1이라도 보정해줌
            damage = (damage == totalDmg)? damage + 1 : totalDmg;
        }

        return damage;
    }
#endregion
}