using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum ENEMY_TYPE
{
    NORMAL, ELITE, BOSS
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public enum STATE { MOVE, ATTACK, DEAD }
    
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

    [SerializeField] float time = 0;
    [SerializeField] float span = 1.0f;

    // 컴포넌트
    SpriteRenderer sprRdr;
    Animator anim;

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
        Debug.Log($"OnTriggerEnter2D():: collision= {col.name}");

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

        this.maxHp = maxHp;
        this.dmg = dmg;

        state = STATE.MOVE;
        anim.SetTrigger(ANIM_TRG_IS_MOVE);
        hp = maxHp;
        hpSlider.value = (float)hp / maxHp;

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
        corAttackId = StartCoroutine(CorAttack(tower));
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

#endregion
}
