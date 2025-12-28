using UnityEngine;

public class Player : MonoBehaviour
{
    public enum STATE
    {
        IDLE, DEAD
    }

    int hp;
    int maxHp;
    int dmg;
    int missileCnt = 1;
    float attackSpeed = 3;
    float criticalPer = 0;
    float criticalDmg = 1.5f;
    float fixSpan = 5;
    float splashPer = 0;
    float splashing = 0;
    
    float time = 0;
    bool isTarget = false;
    public bool IsAlive => hp > 0;
    public STATE state;
    
    void Start(){
        state = STATE.IDLE;
        time = attackSpeed;
        hp = maxHp;
    }
    
    void Update(){
        time += Time.deltaTime;

        // 미사일 발사 처리
        if(isTarget && time > attackSpeed){
            Shot();
            time = 0;
        }
    }

#region FUNC
    public void Shot()
    {
        
    }

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
