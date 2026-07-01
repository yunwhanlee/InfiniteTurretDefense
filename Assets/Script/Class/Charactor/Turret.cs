using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private Coroutine corFlashId;
    MaterialPropertyBlock propBlock;
    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");

    // UI
    public Slider hpSlider;

    // Component
    private SpriteRenderer sprRdr;
    private Animator anim;

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        propBlock = new MaterialPropertyBlock();
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

        maxHp = 10;
        hp = maxHp;

        hpSlider.gameObject.SetActive(false); // HP슬라이더 비표시
        hpSlider.value = (float)hp / maxHp;
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

    /// <summary>
    /// 적으로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        const float Y_OFFSET = 0.7f; // Pivot이 아래 있으므로, Y축을 약간 올려서 데미지 텍스트 표시

        // 이미 죽은상태라면 텍스트만 더 띄우고 종료
        if(hp <= 0)
        {
            hp = 0;
            GM._.dmgTxtMng.GetPool(dmg, transform.position + Vector3.up * Y_OFFSET, false);
            hpSlider.gameObject.SetActive(false);
            return;
        }

        // 데미지 텍스트 표시
        GM._.dmgTxtMng.GetPool(dmg, transform.position + Vector3.up * Y_OFFSET, false);

        hp -= dmg;

        Flash();
        hpSlider.value = (float)hp / maxHp;

        // HP슬라이더 표시
        if(!hpSlider.gameObject.activeSelf)
            hpSlider.gameObject.SetActive(true);
    }

    /// <summary>
    /// 피격시 이미지 흰색번쩍 효과
    /// </summary>
    public void Flash()
    {
        if(corFlashId != null)
            StopCoroutine(corFlashId);
        corFlashId = StartCoroutine(CorFlash());
    }

    /// <summary>
    /// (코루틴 대기) 피격시 이미지 흰색번쩍 효과
    /// </summary>
    IEnumerator CorFlash()
    {
        const int ORIGIN_COLOR = 0;
        const int WHITE_COLOR = 1;

        // 흰색으로 만들기
        sprRdr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(hitFlashMat_IsHit, WHITE_COLOR);
        sprRdr.SetPropertyBlock(propBlock);

        yield return new WaitForSeconds(0.05f);

        // 원래대로 돌리기
        sprRdr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(hitFlashMat_IsHit, ORIGIN_COLOR);
        sprRdr.SetPropertyBlock(propBlock);
    }
#endregion
}
