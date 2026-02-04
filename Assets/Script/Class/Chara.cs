using UnityEngine;
using static Config;

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

    // Status
    public CHR_CARD_IDX CardIdx {get; private set;}
    public CHR_GRADE Grade {get; private set;}
    public int Dmg {get; private set;}
    public float AttackSpeed {get; private set;}
    public float Range {get; private set;}
    public float CritPer {get; private set;}
    public float CritDmgPer {get; private set;}

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

    void Update()
    {
        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        time += Time.deltaTime;

        // 공격
        if(time > AttackSpeed)
        {
            Attack(target);
            time = 0;
        }
    }

#region FUNC
    public void Init(CharaDataAsset charaDataAsset)
    {
        const int SCALE_UNIT = 2; // 범위 원 스케일 단위

        CardIdx = CardIdx;
        Grade = charaDataAsset.grade;
        Dmg = charaDataAsset.baseDmg;
        AttackSpeed = charaDataAsset.baseAttackSpeed;
        Range = charaDataAsset.baseRange;
        CritPer = charaDataAsset.baseRange;
        CritDmgPer = charaDataAsset.baseRange;

        time = AttackSpeed; // 공속 적용
        targetFinder.radius = Range; // 범위 적용
        rangeCircle.transform.localScale = Vector3.one * SCALE_UNIT * Range; // 범위 스케일 조정
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
}
