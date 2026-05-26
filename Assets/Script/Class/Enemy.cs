using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public enum ENEMY_TYPE
{
    NORMAL, ELITE, BOSS
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public enum STATE { MOVE, ATTACK, DEAD, KNOCKBACK }

    // 이벤트 액션
    public Action<Enemy> OnDeadEvent = (Enemy) => {};

    public int maxHp;
    public int hp;

    private float originMoveSpeed;
    public float moveSpeed;

    public int ExtraDmg;
    [SerializeField] int dmg;   public int Dmg
    {
        get => Mathf.Max(1, dmg + ExtraDmg);
        set => dmg = value;
    }

    public bool IsAlive => hp > 0;
    public Transform targetSpotTf;

    [SerializeField] ENEMY_TYPE type;    public ENEMY_TYPE Type {get => type;}
    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}

    // 공격속도
    [SerializeField] float time = 0;
    [SerializeField] float span = 1.0f;

    [SerializeField] float knockbackPower; // 넉백 파워
    [SerializeField] Vector3 knockbackDir; // 넉백 방향

    [Header("상태이상 타이머")]
    [SerializeField] float stunTime = 0; // 스턴
    [SerializeField] float slowTime = 0; // 슬로우
    [SerializeField] float dotTime = 0; // 지속딜

    [Header("상태이상 상세 수치")]
    const float SLOW_RATIO = 0.5f;   // 둔화율 (예: 0.5면 반토막)
    public int dotDamage = 0;      // 초당 들어갈 도트 데미지
    private float dotTickTimer = 0f; // 1초마다 데미지를 주기 위한 내부 틱 타이머

    // 컴포넌트
    SpriteRenderer sprRdr;
    Animator anim;
    SpriteLibrary sprLib;
    public Rigidbody2D rigid;

    // UI
    public Slider hpSlider;

    MaterialPropertyBlock propBlock;
    Vector3 playerPos;
    Vector3 direction;
    Coroutine corFlashId;
    Coroutine corAttackId;

    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");

    const string ANIM_TRG_IS_MOVE = "IsMove";
    const string ANIM_TRG_IS_ATTACK = "IsAttack";
    const string ANIM_TRG_IS_DEAD = "IsDead";

    void Awake()
    {
        // 게임 시작이후 한번만 실행될 것
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sprLib = GetComponent<SpriteLibrary>();
        rigid = GetComponent<Rigidbody2D>();
        propBlock = new MaterialPropertyBlock();
        playerPos = Vector3.zero;
    }

    void Update()
    {
        if(state == STATE.DEAD)
            return;
#region 상태이상
        // 스턴 (상태이상)
        if(stunTime > 0)
        {
            stunTime -= Time.deltaTime;
            // 원상복귀
            if(stunTime <= 0)
            {
                stunTime = 0;
                sprRdr.color = Color.white;
                anim.speed = 1; // 애니메이션 재개
            }
            return;
        }

        // 슬로우
        if(slowTime > 0)
        {
            slowTime -= Time.deltaTime;
            // 원상복귀
            if(slowTime <= 0)
            {
                slowTime = 0;
                sprRdr.color = Color.white;
                moveSpeed = originMoveSpeed;
            }
        }
#endregion
        // 이동
        if(state == STATE.MOVE)
        {
            transform.position += moveSpeed * Time.deltaTime * direction;
        }
        // 넉백
        else if(state == STATE.KNOCKBACK)
        {
            if(knockbackPower > 0)
            {
                transform.position += knockbackPower * Time.deltaTime * knockbackDir;
                knockbackPower -= Time.deltaTime * 4;
            }
            else
            {
                state = STATE.MOVE;
                anim.SetTrigger(ANIM_TRG_IS_MOVE);
                knockbackPower = 0;
                knockbackDir = Vector3.zero;
                direction = (playerPos - transform.position).normalized;
                sprRdr.flipX = direction.x < 0? true : false;;
            }
        }
        // 공격
        else if(state == STATE.ATTACK)
        {
            time += Time.deltaTime;

            if(time > span)
            {
                Tower tower = GM._.tower;
                Attack(tower);

                time = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Debug.Log($"OnTriggerEnter2D():: collision= {col.name}");

        //TODO Player를 Config로 상수만들기
        if(col.gameObject.CompareTag("Player"))
        {
            state = STATE.ATTACK;
        }
    }

#region FUNC
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init(int maxHp, int dmg)
    {
        hpSlider.gameObject.SetActive(false); // HP슬라이더 비표시
        sprRdr.color = Color.white;
        time = 0;
        stunTime = 0;
        slowTime = 0;
        dotTime = 0;
        knockbackPower = 0;
        knockbackDir = Vector3.zero;

        this.maxHp = maxHp;
        Dmg = dmg;
        ExtraDmg = 0;

        state = STATE.MOVE;
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        anim.speed = 1;
        originMoveSpeed = moveSpeed;
        hp = maxHp;
        hpSlider.value = (float)hp / maxHp;

        // 방향
        direction = (playerPos - transform.position).normalized;
        // 방향에 따라 이미지 반전
        bool isFlip = (direction.x < 0)? true : false;
        sprRdr.flipX = isFlip;
    }

    /// <summary>
    /// 스턴 상태이상 적용
    /// </summary>
    /// <param name="duration">지속시간</param>
    public void Stun(float duration)
    {   
        if(state == STATE.DEAD)
            return;

        stunTime = duration;
        sprRdr.color = Color.gray;
        anim.speed = 0; // 애니메이션 멈춤
    }

    /// <summary>
    /// 슬로우
    /// </summary>
    /// <param name="duration">지속시간</param>
    public void Slow(float duration)
    {
        if(state == STATE.DEAD)
            return;

        slowTime = duration;
        sprRdr.color = Color.blue;
        moveSpeed = originMoveSpeed * SLOW_RATIO;
    }

    public void KnockBack(float power, Vector3 dir)
    {
        if(state == STATE.DEAD)
            return;

        state = STATE.KNOCKBACK;
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        knockbackPower = power;
        knockbackDir = dir;
    }

    /// <summary>
    /// 플레이어를 공격
    /// </summary>
    public void Attack(Tower tower)
    {
        // Debug.Log("Attack():: tower=", tower);
        corAttackId = StartCoroutine(CorAttack(tower));
    }

    IEnumerator CorAttack(Tower tower)
    {
        anim.SetTrigger(ANIM_TRG_IS_ATTACK);
        tower.OnHit(Dmg);
        yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// 플레이어로부터 공격받음
    /// </summary>
    public void OnHit(int dmg, bool isCritical)
    {
        // 이미 죽은상태라면 텍스트만 더 띄우고 종료
        if(state == STATE.DEAD)
        {
            GM._.dmgTxtMng.GetPool(dmg, transform.position, isCritical);
            return;
        }

        // 데미지 텍스트 표시
        GM._.dmgTxtMng.GetPool(dmg, transform.position, isCritical);

        hp -= dmg;

        Flash();
        hpSlider.value = (float)hp / maxHp;

        // HP슬라이더 표시
        if(!hpSlider.gameObject.activeSelf)
            hpSlider.gameObject.SetActive(true);

        // 죽음
        if(!IsAlive)
        {
            StartCoroutine(CorDead());
        }
    }

    IEnumerator CorDead()
    {
        state = STATE.DEAD;
        hp = 0;
        anim.SetTrigger(ANIM_TRG_IS_DEAD);
        hpSlider.gameObject.SetActive(false);

        GM._.emm.KillCnt++;
        GM._.emm.EnemyCnt--;

        if(corAttackId != null)
            StopCoroutine(corAttackId);

        yield return new WaitForSeconds(1.5f);
        OnDeadEvent?.Invoke(this);
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

    public void SetSprLibAst(SpriteLibraryAsset[] sprLibAstArr)
    {
        int len = sprLibAstArr.Length;
        int randIdx = Random.Range(0, len);
        sprLib.spriteLibraryAsset = sprLibAstArr[randIdx];
    }

#endregion
}
