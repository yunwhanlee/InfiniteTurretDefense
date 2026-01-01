using UnityEngine;

public class Chara : MonoBehaviour
{
    public TargetFinder targetFinder;
    public Missile missile;

    [SerializeField] float time = 0;
    [SerializeField] float attackSpeed = 2.0f;

    int missileCnt = 1;
    float criticalPer = 0;
    float criticalDmgPer = 1.5f;
    float fixSpan = 5;
    float splashPer = 0;
    float splashing = 0;

    SpriteRenderer sprRdr;
    Animator anim;

    void Start()
    {
        sprRdr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        time = attackSpeed;
    }

    void Update()
    {
        Enemy target = targetFinder.CurrentTarget;
        if(target == null)
            return;

        time += Time.deltaTime;

        // 공격
        if(time > attackSpeed){
            Attack(target);
            time = 0;
        }
    }

#region FUNC
    public void Attack(Enemy enemy)
    {
        Debug.Log($"Attack():: {enemy.name}, HP: {enemy.hp}");

        Vector3 direction = (enemy.targetSpotTf.position - transform.position).normalized;

        sprRdr.flipX = direction.x < 0;

        anim.SetTrigger("IsAttack");

        // 투사체 발사
        GameManager._.msm.SpawnMissile(transform.position, direction);
    }
#endregion
}
