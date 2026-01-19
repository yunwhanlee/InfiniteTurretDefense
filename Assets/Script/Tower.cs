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
                UI._.towerHpTxt.text = $"{hp} / {maxHp}";
                UI._.towerHpSlider.value = (float)hp / maxHp;
            }
        }
    public bool IsAlive => hp > 0;

    TowerUIManager towerUI;
    SpriteRenderer sprRdr;
    MaterialPropertyBlock propBlock;
    Coroutine corFlashId;
    static readonly int hitFlashMat_IsHit = Shader.PropertyToID("_IsHit");
    const float FLASH_DEF_TIME = 0.05f;

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

    public int GetMaxHp()
    {
        const int DEF_HP = 500;
        return DEF_HP + (towerUI.upgradeHpLv * towerUI.upgHpVal);
    }
    public void SetMaxHp() => maxHp = GetMaxHp();

    public int GetArmor()
    {
        return towerUI.UpgradeArmorLv * towerUI.upgArmorVal;
    }
    public void SetArmor() => armor = GetArmor();

    private void SetFlashColor(bool isEnable)
    {
        isFlashing = isEnable;

        int val = isEnable ? 1 : 0;

        sprRdr.GetPropertyBlock(propBlock);
        propBlock.SetFloat(hitFlashMat_IsHit, val);
        sprRdr.SetPropertyBlock(propBlock);
    }

    /// <summary>
    /// 피격시 이미지 흰색번쩍 효과
    /// </summary>
    // public void Flash()
    // {
    //     if(corFlashId != null)
    //         StopCoroutine(corFlashId);
    //     corFlashId = StartCoroutine(CorFlash());
    // }

    /// <summary>
    /// (코루틴 대기) 피격시 이미지 흰색번쩍 효과
    /// </summary>
    // IEnumerator CorFlash()
    // {
    //     const int ORIGIN_COLOR = 0;
    //     const int WHITE_COLOR = 1;

    //     // 흰색으로 만들기
    //     sprRdr.GetPropertyBlock(propBlock);
    //     propBlock.SetFloat(hitFlashMat_IsHit, WHITE_COLOR);
    //     sprRdr.SetPropertyBlock(propBlock);

    //     yield return new WaitForSeconds(0.1f);

    //     // 원래대로 돌리기
    //     sprRdr.GetPropertyBlock(propBlock);
    //     propBlock.SetFloat(hitFlashMat_IsHit, ORIGIN_COLOR);
    //     sprRdr.SetPropertyBlock(propBlock);
    // }
#endregion
}
