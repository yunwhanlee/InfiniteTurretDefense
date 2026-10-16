using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;
using static Config;

public class Tower : MonoBehaviour
{
    const float BLINK_TIME = 0.05f; // 피격시 플래시 지속시간
    const int DEF_HP = 500;             // 기본 체력
    const int HEAL_SPAN_SEC = 5;        // 자동 회복 간격(초)

    public enum STATE {IDLE, DEAD}
    [SerializeField] STATE state;    public STATE State { get => state; set => state = value; }

    //* 이벤트
    public event Action<int> OnArmorChanged; // 방어력
    public event Action<int, int> OnHpChanged; // 현재 체력, 최대 체력
    public event Action<int, int> OnSheildChanged; // 현재 쉴드, 최대 쉴드

    public SpriteRenderer towerUpSideSprRdr; // 성벽 위쪽 스프라이트 랜더러

    [SerializeField] float blinkTime = 0f; // 현재 색이 흰색인지 체크하는 변수
    [SerializeField] bool isBlink = false;

    // 회복력
    [SerializeField] float healTime = 0f;
    [SerializeField] int healVal;  public int HealVal
    {
        get => healVal;
        set => healVal = value;
    }
    // 방어력
    [SerializeField] int armor; public int Armor
    {
        get => armor;
        set{
            armor = value;
            OnArmorChanged?.Invoke(armor); // 이벤트 호출
        }
    }
    // 체력
    [SerializeField] int maxHp; public int MaxHp { get => maxHp;}
    [SerializeField] int hp;    public int Hp
    {
        get => hp;
        set{
            // 체력 변경시
            hp = Mathf.Clamp(value, 0, maxHp);
            OnHpChanged?.Invoke(hp, maxHp); // 이벤트 호출
        }
    }
    // 쉴드
    [SerializeField] int maxShield; public int MaxSheild {get => maxShield;}
    [SerializeField] int shield; public int Sheild
    {
        get => shield;
        set
        {
            shield = value;

            // 쉴드가 다 깎였을때 초기화
            if(shield <= 0)
            {
                shield = 0;
                maxShield = 0;
            }
            // 쉴드 최대량 업데이트
            else if(maxShield < shield)
            {
                maxShield = value;
            }

            OnSheildChanged?.Invoke(shield, maxShield); // 이벤트 호출
        } 
    }

    Coroutine corBlinkID;
    WaitForSeconds waitSec;
    SpriteRenderer sprRdr;
    MaterialPropertyBlock propBlock;
    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit"); // 피격시 흰색 플래시 효과용 SHADER 프로퍼티 ID

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
        waitSec = new WaitForSeconds(BLINK_TIME);

        state = STATE.IDLE;
        healVal = 1; // 회복력
        shield = 0; // 쉴드
        Armor = 0; // 방어력
        Hp = maxHp = DEF_HP; // 체력

        // (이벤트 등록) 방어력 변경시 UI 업데이트
        OnArmorChanged += (_armor) => UI._.SetTowerArmorTxt(_armor);
        // (이벤트 등록) 체력 변경시 UI 업데이트
        OnHpChanged += (_hp, _maxHp) => UI._.SetTowerHpSlider(_hp, _maxHp);
        // (이벤트 등록) 쉴드 변경시 UI 업데이트
        OnSheildChanged += (_sheild, _maxSheild) => UI._.SetSheildHpSlider(_sheild, _maxSheild);
    }
    
    void Update(){
        if(state == STATE.DEAD)
            return;
        
        // 자동 회복
        healTime += Time.deltaTime;
        if( healTime >= HEAL_SPAN_SEC )
        {
            healTime = 0f;
            Heal(HealVal);
        }
    }

#region FUNC
    IEnumerator CoBlink()
    {
        // 블링크 처리
        Blink(true);
        yield return waitSec;
        Blink(false);

        // 코루틴 비우기
        StopCoroutine(corBlinkID);
        corBlinkID = null;
    }

    private void Blink(bool isEnable)
    {
        int val = isEnable ? 1 : 0;

        sprRdr.GetPropertyBlock(propBlock);
        towerUpSideSprRdr.GetPropertyBlock(propBlock);

        propBlock.SetFloat(hitFlashMat_IsHit, val);
        sprRdr.SetPropertyBlock(propBlock);
        towerUpSideSprRdr.SetPropertyBlock(propBlock);
    }

    /// <summary>
    /// 적으로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        if(state == STATE.DEAD) return;

        dmg = armor >= dmg ? 1 : dmg - armor;

        // 쉴드
        if(shield > 0)
        {
            if(shield >= dmg)
            {
                Sheild -= dmg;
            }
            // 데미지가 쉴드를 초과할시
            else
            {
                int overDmg = dmg - shield;
                Sheild = 0;
                Hp -= overDmg; // 초과부분 체력 감소
            }
        }
        // 체력
        else
        {
            Hp -= dmg;
        }

        if(hp > 0)
        {
            // 블링크
            if(corBlinkID == null)
            {
                corBlinkID = StartCoroutine(CoBlink());
            }
        }
        else
        {
            state = STATE.DEAD;
            Hp = 0;

            //TODO ReStart Game
        }
    }
    /// <summary>
    /// 최대체력 증가
    /// </summary>
    public void AddMaxHp(int val)
    {
        maxHp += val;
        Hp += val; // 최대 체력 증가시 현재 체력도 같이 증가
    }

    /// <summary>
    /// 최대방어력 증가
    /// </summary>
    public void AddArmor(int val)
    {
        Armor += val;
    }

    /// <summary>
    /// 타워 회복
    /// </summary>
    public void Heal(int val)
    {
        Hp += val;
    }
#endregion
}
