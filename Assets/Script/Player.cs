using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum STATE
    {
        IDLE, DEAD
    }

    public TargetFinder targetFinder;
    public Missile missile;
    public bool IsAlive => hp > 0;

    [SerializeField] STATE state;    public STATE State {get => state; set => state = value;}
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] float attackSpeed = 2.0f;
    int missileCnt = 1;
    float criticalPer = 0;
    float criticalDmgPer = 1.5f;
    float fixSpan = 5;
    float splashPer = 0;
    float splashing = 0;

    [SerializeField] float time = 0;

    void Start(){
        state = STATE.IDLE;
        time = attackSpeed;
        hp = maxHp;
    }
    
    void Update(){
        if(state == STATE.DEAD)
            return;

        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        time += Time.deltaTime;

        // 미사일 발사 처리
        if(time > attackSpeed){
            Shot(target);
            time = 0;
        }
    }

#region FUNC
    public void Shot(Enemy enemy)
    {
        Debug.Log($"Shot():: {enemy.name}, HP: {enemy.hp}");
        Missile ins = Instantiate(missile, transform.position, quaternion.identity);
        ins.Direction = (enemy.targetSpotTf.position - transform.position).normalized;
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
