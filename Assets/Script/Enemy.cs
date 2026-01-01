using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum ENEMY_TYPE
{
    NORMAL, ELITE, BOSS
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public enum STATE
    {
        MOVE, ATTACK, DEAD
    }
    
    // 이벤트 액션
    public Action<Enemy> OnDeadEvent = (Enemy) => {};

    public int maxHp;
    public int hp;
    public float moveSpeed;
    public int dmg;
    public bool IsAlive => hp > 0;
    public Transform targetSpotTf;

    [SerializeField] ENEMY_TYPE type;    public ENEMY_TYPE Type {get => type;}
    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}

    // 컴포넌트
    SpriteRenderer sprRdr;
    Animator anim;

    MaterialPropertyBlock propBlock;
    Vector3 playerPos;
    Vector3 direction;
    Coroutine corFlashId;
    Coroutine CorAttackId;

    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");

    const string ANIM_TRG_IS_MOVE = "IsMove";
    const string ANIM_TRG_IS_ATTACK = "IsAttack";
    const string ANIM_TRG_IS_DEAD = "IsDead";

    void Awake()
    {
        // 게임 시작이후 한번만 실행될 것
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        propBlock = new MaterialPropertyBlock();
        playerPos = Vector3.zero;
    }

    void Update()
    {
        if(state == STATE.MOVE)
        {
            // 이동
            transform.position += moveSpeed * Time.deltaTime * direction;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"OnTriggerEnter2D():: collision= {col.name}");

        //TODO Player를 Config로 상수만들기
        if(col.gameObject.CompareTag("Player"))
        {
            Tower tower = GM._.tower;
            Attack(tower);
        }
    }

#region FUNC
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init(int maxHp, int dmg)
    {
        this.maxHp = maxHp;
        this.dmg = dmg;

        state = STATE.MOVE;
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        hp = maxHp;

        // 방향
        direction = (playerPos - transform.position).normalized;
        // 방향에 따라 이미지 반전
        bool isFlip = (direction.x < 0)? true : false;
        sprRdr.flipX = isFlip;
    }
    
    /// <summary>
    /// 플레이어를 공격
    /// </summary>
    public void Attack(Tower tower)
    {
        Debug.Log("Attack():: tower=", tower);
        state = STATE.ATTACK;

        CorAttackId = StartCoroutine(CorAttack(tower));
    }

    IEnumerator CorAttack(Tower tower)
    {
        anim.SetTrigger(ANIM_TRG_IS_ATTACK);
        tower.OnHit(dmg);
        yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// 플레이어로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        hp -= dmg;
        Flash();

        if(!IsAlive)
        {
            StartCoroutine(CorDead());
        }
    }

    IEnumerator CorDead()
    {
        GM._.emm.KillCnt++;
        GM._.emm.EnemyCnt--;

        state = STATE.DEAD;
        hp = 0;
        anim.SetTrigger(ANIM_TRG_IS_DEAD);

        if(CorAttackId != null)
            StopCoroutine(CorAttackId);

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

        yield return new WaitForSeconds(0.1f);

        // 원래대로 돌리기
        sprRdr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(hitFlashMat_IsHit, ORIGIN_COLOR);
        sprRdr.SetPropertyBlock(propBlock);
    }

#endregion
}
