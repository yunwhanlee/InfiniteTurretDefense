using Unity.Mathematics;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public enum STATE {IDLE, DEAD}


    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}
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


    void Start(){
        state = STATE.IDLE;
        Armor = 0;
        maxHp = 1000;
        Hp = maxHp;
    }
    
    void Update(){
        if(state == STATE.DEAD)
            return;
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
        }
        else
        {
            state = STATE.DEAD;
            Hp = 0;

            //TODO ReStart Game
        }
    }
#endregion
}
