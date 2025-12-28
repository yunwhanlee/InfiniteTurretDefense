using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum MONSTER_TYPE
{
    NORMAL, ELITE, BOSS
}

public class Enemy : MonoBehaviour
{
    public enum STATE
    {
        MOVE, ATTACK, DEAD
    }
    
    public int maxHp;
    public float hp;
    public float moveSpeed;
    public int dmg;
    public bool IsAlive => hp > 0;

    [SerializeField] MONSTER_TYPE type;    public MONSTER_TYPE Type {get => type;}
    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}

    // 컴포넌트
    SpriteRenderer sprRdr;
    Animator anim;

    MaterialPropertyBlock propBlock;
    Vector3 playerPos;
    Vector3 direction;
    Coroutine corFlashId;

    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");

    const string ANIM_TRG_IS_MOVE = "IsMove";
    const string ANIM_TRG_IS_ATTACK = "IsAttack";
    const string ANIM_TRG_IS_DEAD = "IsDead";

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        state = STATE.MOVE;
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        propBlock = new MaterialPropertyBlock();
        playerPos = Vector3.zero;
        hp = maxHp;

        // 방향
        direction = (playerPos - transform.position).normalized;
        // 방향에 따라 이미지 반전
        bool isFlip = (direction.x < 0)? true : false;
        sprRdr.flipX = isFlip;
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
            Player player = col.GetComponent<Player>();
            Attack(player);
        }
    }

#region FUNC
    /// <summary>
    /// 플레이어를 공격
    /// </summary>
    public void Attack(Player player)
    {
        Debug.Log("Attack()::");
        state = STATE.ATTACK;

        StartCoroutine(CorAttack(player));
    }

    IEnumerator CorAttack(Player player)
    {
        anim.SetTrigger(ANIM_TRG_IS_ATTACK);
        player.OnHit(dmg);
        yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// 플레이어로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        if(IsAlive)
        {
            hp -= dmg;
            Flash();
        }
        else
        {
            state = STATE.DEAD;
            hp = 0;
            Flash();

            Destroy(gameObject, 0.1f);
        }
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
