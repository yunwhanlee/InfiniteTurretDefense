using UnityEngine;
using static Config;

public class Chara : MonoBehaviour
{
    // 외부 클래스
    public TargetFinder targetFinder;
    public Missile missile;

    public bool isLocked; // 잠김 여부
    public CHR_PLACE place; // 배치 위치
    public GameObject rangeCircle; // 클릭시 보이는 공격범위 원

    // Status
    [SerializeField] CHR_GRADE grade = CHR_GRADE.NORMAL;    
    public CHR_GRADE Grade {
        get => grade;
        set => grade = value;
    }
    [SerializeField] int dmg = 10;  
    public int Dmg {
        get => dmg;
        set => dmg = value;
    }
    [SerializeField] float attackSpeed = 2.0f;  
    public float AttackSpeed {
        get => attackSpeed;
        set => attackSpeed = value;
    }
    [SerializeField] float range = 5f; 
    public float Range {
        get => range;
        set => range = value;
    }
    [SerializeField] float critPer = 0f;    
    public float CritPer {
        get => critPer;
        set => critPer = value;
    }
    [SerializeField] float critDmgPer = 1.5f;   
    public float CritDmgPer {
        get => critDmgPer;
        set => critDmgPer = value;
    }

    float time = 0;
    SpriteRenderer sprRdr;
    Animator anim;

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rangeCircle.SetActive(false);

        time = attackSpeed; // 공속 적용
        targetFinder.radius = range; // 범위 적용
        rangeCircle.transform.localScale = Vector3.one * range; // 범위 스케일 조정
    }

    void Update()
    {
        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        time += Time.deltaTime;

        // 공격
        if(time > attackSpeed)
        {
            Attack(target);
            time = 0;
        }
    }

#region FUNC
    public void Attack(Enemy enemy)
    {
        Debug.Log($"Attack():: {enemy.name}, HP: {enemy.hp}");

        Vector3 direction = (enemy.targetSpotTf.position - transform.position).normalized;
        sprRdr.flipX = direction.x < 0;
        anim.SetTrigger("IsAttack");
        // 투사체 발사
        GM._.msm.SpawnMissile(transform.position, direction, dmg);
    }
#endregion
}
