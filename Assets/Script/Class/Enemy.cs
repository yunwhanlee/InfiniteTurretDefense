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



    [Header("속성")]
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

    [Header("타겟팅(어그로) 설정")]
    public Transform targetSpotTf;
    float detectRadius = 2.0f; // 타겟팅 탐색 반경
    Transform towerTf; // 타워 Transform
    Transform curTarget; // 💡 요놈 하나로 이동/공격 모두 처리!
    Vector3 direction; // 적 이동 방향

    // UI
    public Slider hpSlider;

    // 컴포넌트
    SpriteRenderer sprRdr;
    Animator anim;
    SpriteLibrary sprLib;
    public Rigidbody2D rigid;

    MaterialPropertyBlock propBlock;
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
                direction = (curTarget.position - transform.position).normalized;
                sprRdr.flipX = direction.x < 0? true : false;;
            }
        }
        // 공격
        else if(state == STATE.ATTACK)
        {
            time += Time.deltaTime;

            if(time > span)
            {
                Attack();
                time = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 💡 Config 레이어 비트마스크 연산으로 충돌 확인
        if ((Config.Layer.TOWER & (1 << col.gameObject.layer)) > 0)
        {
            curTarget = col.transform; // 부딪힌 놈을 타겟으로 확정!
            state = STATE.ATTACK;
            CancelInvoke(nameof(FindTarget)); // 공격할 땐 두리번거리지 않음
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
        towerTf = GM._.tower.transform;
        curTarget = towerTf;
        direction = (curTarget.position - transform.position).normalized;

        // 방향에 따라 이미지 반전
        bool isFlip = (direction.x < 0)? true : false;
        sprRdr.flipX = isFlip;

        CancelInvoke(nameof(FindTarget));
        InvokeRepeating(nameof(FindTarget), 0f, 0.25f);
    }

    private void FindTarget()
    {
        if (!IsAlive || state == STATE.DEAD) return;

        // 💡 Config 레이어 직접 사용! (Config.LAYER.Tower 등으로 쓰시면 됩니다)
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, detectRadius, Config.Layer.TOWER);

        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        // 가장 가까운 타겟 탐색
        foreach (Collider2D col in cols)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTarget = col.transform;
            }
        }

        // 가장 가까운 놈으로 타겟 갱신 (없다면 타워로)
        curTarget = (nearestTarget != null) ? nearestTarget : towerTf;
        direction = (curTarget.position - transform.position).normalized;
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
    public void Attack()
    {
        // Debug.Log("Attack():: tower=", tower);
        if(corAttackId != null) StopCoroutine(corAttackId);
        corAttackId = StartCoroutine(CorAttack());
    }

    IEnumerator CorAttack()
    {
        anim.SetTrigger(ANIM_TRG_IS_ATTACK);

        // 타겟에 따른 컴포넌트 호출
        if(curTarget.GetComponent<Tower>() is Tower tower)
        {
            tower.OnHit(Dmg);
        }
        else if(curTarget.GetComponent<Turret>() is Turret turret)
        {
            turret.OnHit(Dmg);
        }

        yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// 플레이어로부터 공격받음
    /// </summary>
    public void OnHit(int dmg, bool isCritical)
    {
        const float Y_OFFSET = 0.7f; // Pivot이 아래 있으므로, Y축을 약간 올려서 데미지 텍스트 표시

        // 이미 죽은상태라면 텍스트만 더 띄우고 종료
        if(state == STATE.DEAD)
        {
            GM._.dmgTxtMng.GetPool(dmg, transform.position + Vector3.up * Y_OFFSET, isCritical);
            return;
        }

        // 데미지 텍스트 표시
        GM._.dmgTxtMng.GetPool(dmg, transform.position + Vector3.up * Y_OFFSET, isCritical);

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

#if UNITY_EDITOR
    /// <summary>
    /// 에디터 씬 뷰에서 적을 선택했을 때 어그로 반경을 그려줍니다.
    /// </summary>
    private void OnDrawGizmos()
    {
        // 1. 기즈모 색상을 반투명한 빨간색으로 설정
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        
        // 2. 내 위치를 기준으로 detectRadius 반경만큼 선으로 된 원을 그림
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
#endif
}
