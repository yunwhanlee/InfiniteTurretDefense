using UnityEngine;

/// <summary>
/// 엔지니어 LV3. 터렛
/// </summary>
public class Turret : MonoBehaviour
{
    // 외부 클래스
    public TargetFinder targetFinder;

    // Value
    public Transform shootTf;
    public Sprite missileSpr;
    [SerializeField] private int maxHp;
    [SerializeField] private int hp;     public int Hp { get => hp;}
    [SerializeField] private int damage;    public int Damage { get => damage;}
    [SerializeField] private float attackSpeed; public float AttackSpeed { get => attackSpeed;}
    [SerializeField] private float time = 0;
    private Vector3 direction;

    // UI

    // Component
    private SpriteRenderer sprRdr;
    private Animator anim;

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        Init();
    }

    void Update()
    {
        time += Time.deltaTime;

        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        // 공격
        if(time > Util.GetAttackPerSecond(AttackSpeed))
        {
            Attack(target);
            time = 0;
            attackSpeed = 1;
        }
    }

#region FUNC
    public void Init()
    {
        attackSpeed = 1;
        damage = 5;
    }

    public void Attack(Enemy enemy)
    {
        // Debug.Log($"Attack():: {enemy.name}, HP: {enemy.hp}");
        direction = (enemy.targetSpotTf.position - transform.position).normalized;
        sprRdr.flipX = direction.x < 0;
        anim.SetTrigger("IsAttack");

        // 오브젝트 중심과 총구 사이의 절대적인 X거리(Offset)를 구함
        float shootOffsetX = Mathf.Abs(shootTf.position.x - transform.position.x);
        float posX = transform.position.x + (sprRdr.flipX ? -shootOffsetX : shootOffsetX);
        Vector2 pos = new Vector2(posX, shootTf.position.y);

        // 투사체 발사
        GM._.mpm.SpawnPool(pos, direction, damage, 0, missileSpr, false);
    }
#endregion
}
