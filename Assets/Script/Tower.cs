using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;

public class Tower : MonoBehaviour
{
    public enum STATE {IDLE, DEAD}

    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}

    [SerializeField] float flashTime = 0f; // 현재 색이 흰색인지 체크하는 변수
    [SerializeField] bool isFlashing = false;
    [SerializeField] int maxHp;
    [SerializeField] int armor; public int Armor
        {
            get => armor;
            set{
                armor = value;
                UI._.towerArmorTxt.text = armor.ToString();
            }
        }
    [SerializeField] int hp;    public int Hp
        {
            get => hp;
            set{
                hp = value;
                UI._.SetTowerHpSlider(hp, maxHp);
            }
        }
    public bool IsAlive => hp > 0;

    TowerUIManager towerUI;
    SpriteRenderer sprRdr;
    MaterialPropertyBlock propBlock;

    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");
    const float FLASH_DEF_TIME = 0.05f;
    const int DEF_HP = 500;

    void Start(){
        towerUI = UI._.towerUI;
        sprRdr = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();

        state = STATE.IDLE;
        Armor = 0;
        maxHp = GetMaxHp();
        Hp = maxHp;
    }
    
    void Update(){
        if(state == STATE.DEAD)
            return;
        
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

    public int GetMaxHp() => DEF_HP + towerUI.GetUpgradeHpVal();
    public void SetMaxHp()
    {
        hp += towerUI.UPGRADE_HP_VAL;
        maxHp = GetMaxHp();
    }

    public int GetArmor() => towerUI.GetUpgradeArmorVal();
    public void SetArmor() => armor = GetArmor();

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
