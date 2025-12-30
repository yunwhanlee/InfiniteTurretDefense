using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum STATE
    {
        IDLE, DEAD
    }

    public bool IsAlive => hp > 0;

    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}
    [SerializeField] int hp;
    [SerializeField] int maxHp;

    void Start(){
        state = STATE.IDLE;
        hp = maxHp;
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
            hp -= dmg;
        }
        else
        {
            state = STATE.DEAD;
            hp = 0;

            //TODO ReStart Game
        }
    }
#endregion
}
