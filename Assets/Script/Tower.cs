using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;

public class Tower : MonoBehaviour
{
    public enum STATE {IDLE, DEAD}

    [SerializeField] STATE state;    public STATE State { get => state; set => state = value; }

    //* 이벤트
    public event Action<int, int> OnHpChanged; // 현재 체력, 최대 체력
    public event Action<int> OnArmorChanged; // 방어력

    [SerializeField] float flashTime = 0f; // 현재 색이 흰색인지 체크하는 변수
    [SerializeField] bool isFlashing = false;

    [SerializeField] float healTime = 0f;
    [SerializeField] int healVal = 0;  public int HealVal { get => healVal; set { healVal = value; } }

    [SerializeField] int maxHp;
    [SerializeField] int armor; public int Armor
        {
            get => armor;
            set{
                armor = value;
                OnArmorChanged?.Invoke(armor); // 이벤트 호출
            }
        }
    [SerializeField] int hp;    public int Hp
        {
            get => hp;
            set{
                // 체력 변경시
                hp = Mathf.Clamp(value, 0, maxHp);
                OnHpChanged?.Invoke(hp, maxHp); // 이벤트 호출
            }
        }

    public bool IsAlive => hp > 0;

    SpriteRenderer sprRdr;
    MaterialPropertyBlock propBlock;

    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit"); // 피격시 흰색 플래시 효과용 SHADER 프로퍼티 ID

    const float FLASH_DEF_TIME = 0.05f; // 피격시 플래시 지속시간
    const int DEF_HP = 500;             // 기본 체력
    const int HEAL_SPAN_SEC = 1;        // 자동 회복 간격(초)

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();

        state = STATE.IDLE;
        Armor = 0;
        Hp = maxHp = DEF_HP;

        // (이벤트 등록) 체력 변경시 UI 업데이트
        OnHpChanged += (_hp, _maxHp) => UI._.SetTowerHpSlider(_hp, _maxHp);
        // (이벤트 등록) 방어력 변경시 UI 업데이트
        OnArmorChanged += (_armor) => UI._.SetTowerArmorTxt(_armor);
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


        // 피격 받았을시 플래시 효과
        if(flashTime > 0f)
        {
            flashTime -= Time.deltaTime;

            if(flashTime <= 0f && isFlashing)
            {
                // 원래대로 돌리기
                SetFlashColor(false);
            }
        }
    }

#region FUNC
    /// <summary>
    /// 적으로부터 공격받음
    /// </summary>
    public void OnHit(int dmg)
    {
        if(IsAlive)
        {
            dmg = armor >= dmg ? 1 : dmg - armor;
            Hp -= dmg;
            
            flashTime = FLASH_DEF_TIME;

            if(!isFlashing)
            {
                // 흰색으로 만들기
                SetFlashColor(true);
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

    private void SetFlashColor(bool isEnable)
    {
        isFlashing = isEnable;

        int val = isEnable ? 1 : 0;

        sprRdr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(hitFlashMat_IsHit, val);
        sprRdr.SetPropertyBlock(propBlock);
    }
#endregion
}
