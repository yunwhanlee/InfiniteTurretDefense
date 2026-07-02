using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 엔지니어 LV3. 터렛
/// </summary>
public class Turret : MonoBehaviour
{
    public enum STATE { IDLE, ATTACK, DEAD };
    [SerializeField] private STATE state;

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
    Vector3 direction;
    Coroutine corFlashId;
    MaterialPropertyBlock propBlock;
    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");
    const string ANIM_TRG_IS_MOVE = "IsMove";
    const string ANIM_TRG_IS_ATTACK = "IsAttack";
    const string ANIM_TRG_IS_DEAD = "IsDead";

    // UI
    public Slider hpSlider;
    // Component
    SpriteRenderer sprRdr;
    Animator anim;

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        propBlock = new MaterialPropertyBlock();
        Init();
    }

    void Update()
    {
        if(state == STATE.DEAD) return;

        time += Time.deltaTime;

        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
        {
            if (state != STATE.IDLE)
            {
                state = STATE.IDLE;
                anim.SetTrigger(ANIM_TRG_IS_MOVE);
            }
            return;
        }

        // 공격
        if(time > Util.GetAttackPerSecond(AttackSpeed))
        {
            state = STATE.ATTACK;
            Attack(target);
            time = 0;
            attackSpeed = 1;
        }
    }

#region FUNC
    public void Init()
    {
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        state = STATE.IDLE;
        time = 0;

        attackSpeed = 1;
        damage = 2;

        maxHp = 3;
        hp = maxHp;

        hpSlider.gameObject.SetActive(false); // HP슬라이더 비표시
        hpSlider.value = (float)hp / maxHp;
    }

    public void Attack(Enemy enemy)
    {
        // Debug.Log($"Attack():: {enemy.name}, HP: {enemy.hp}");
        sprRdr.flipX = direction.x < 0;
        anim.SetTrigger(ANIM_TRG_IS_ATTACK);

        // 오브젝트 중심과 총구 사이의 절대적인 X거리(Offset)를 구함
        float shootOffsetX = Mathf.Abs(shootTf.position.x - transform.position.x);
        float posX = transform.position.x + (sprRdr.flipX ? -shootOffsetX : shootOffsetX);
        Vector3 shootPos = new Vector3(posX, shootTf.position.y, 0);

        direction = (enemy.targetSpotTf.position - shootPos).normalized;

        // 투사체 발사
        GM._.mpm.SpawnPool(shootPos, direction, damage, 0, missileSpr, false);
    }

    /// <summary>
    /// 적으로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        if(state == STATE.DEAD) return;

        const float Y_OFFSET = 0.7f; // Pivot이 아래 있으므로, Y축을 약간 올려서 데미지 텍스트 표시


        // 데미지 텍스트 표시
        GM._.dmgTxtMng.GetPool(dmg, transform.position + Vector3.up * Y_OFFSET, false);

        hp -= dmg;

        Flash();
        hpSlider.value = (float)hp / maxHp;

        // 죽음
        if(hp <= 0)
        {
            StartCoroutine(CorDead());
            return;
        }

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

    IEnumerator CorDead()
    {
        state = STATE.DEAD;
        hp = 0;
        anim.SetTrigger(ANIM_TRG_IS_DEAD);
        hpSlider.gameObject.SetActive(false);

        yield return Config.WFS_1;
        yield return Config.WFS_0_5;
    }
#endregion
}
