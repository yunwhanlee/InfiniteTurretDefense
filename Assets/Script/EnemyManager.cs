using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] int DEF_HP = 900; // 5;
    [SerializeField] int DEF_DMG = 2;
    [SerializeField] float MONSTER_SPAN = 1; // 일반몬스터 소환주기
    const int ELITE_SPAN = 20; // 엘리트몬스터 소환주기
    const int BOSS_SPAN = ELITE_SPAN * 10; // 엘리트몬스터 소환주기

    [Header("적 체력 및 공격력 증가 시간단위")]
    [SerializeField] float upHpSpan = 2.5f;
    [SerializeField] int upDmgSpan = 10;

    // 오브젝트 풀링
    public Transform enemyGroupTf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}

    [SerializeField] float totalTime;
    [SerializeField] float time;
    [SerializeField] float eliteTime;
    [SerializeField] float spawnRadius; // 몬스터 생성 원 크기

    [SerializeField] Enemy enemyPref; // 몬스터 프리팹

    [SerializeField] int killCnt;   public int KillCnt
        {
            get => killCnt;
            set {
                killCnt = value;
                UI._.killCntTxt.text = $"{value}킬";
            }
        }
    [SerializeField] int enemyCnt;   public int EnemyCnt
        {
            get => enemyCnt;
            set {
                enemyCnt = value;
                UI._.EnemyCntTxt.text = $" : {value}";
            }
        }
    [SerializeField] int enemyHp;    public int EnemyHp
        {
            get => enemyHp;
            set {
                enemyHp = value;
                UI._.EnemyHpTxt.text = $"체력 : {value}";
            }
        }
    [SerializeField] int enemyDmg;  public int EnemyDmg
        {
            get => enemyDmg;
            set {
                enemyDmg = value;
                UI._.EnemyDmgTxt.text = $"데미지 : {value}";
            }
        }

    StageManager stm;

    void Start()
    {
        const int OFFSET_SEC = 15;

        stm = GM._.stm;
        pool = new ObjectPool<Enemy>(Create, OnGet, OnRelease, OnDelete, maxSize: 20);

        totalTime = 0 - OFFSET_SEC;
        time = MONSTER_SPAN;

        KillCnt = 0;
        EnemyCnt = 0;
        EnemyHp = DEF_HP;
        EnemyDmg = DEF_DMG;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        totalTime += deltaTime;
        time += deltaTime;
        eliteTime += deltaTime;

        // 몬스터 소환
        if(time > MONSTER_SPAN)
        {
            time = 0;

            if(totalTime >= 0)
            {
                // 적 체력 및 공격력 업데이트
                EnemyHp = DEF_HP + (int)(totalTime / upHpSpan);
                EnemyDmg = DEF_DMG + (int)(totalTime / upDmgSpan);
            }

            // 적 생성
            Enemy enemy = SpawnEnemy(enemyHp, enemyDmg);
            enemy.moveSpeed = 1;
            enemy.transform.localScale = new Vector3(1,1,1);
            enemy.SetSprLibAst(stm.stage1_MonsterSprLibAstArr);
            // 카운트 증가
            EnemyCnt++;
            // (이벤트 구독) 죽었을시 오브젝트풀링 회수 
            enemy.OnDeadEvent = (enemy) => {
                pool.Release(enemy);
            };
        }
        // 엘리트 몬스터 소환
        else if(eliteTime > ELITE_SPAN)
        {
            Debug.Log("엘리트 몬스터 소환");
            eliteTime = 0;
            time = 0; // 몬스터 소환도 한 주기 안되도록

            // 적 체력 및 공격력 업데이트
            EnemyHp = DEF_HP + (int)(totalTime / upHpSpan);
            EnemyDmg = DEF_DMG + (int)(totalTime / upDmgSpan);

            const int ELITE_UNIT = 8;

            // 적 체력 및 공격력 업데이트
            Enemy enemy = SpawnEnemy(enemyHp * ELITE_UNIT, enemyDmg * ELITE_UNIT / 4);
            enemy.moveSpeed = 0.5f;
            enemy.transform.localScale = new Vector3(3,3,3);
            enemy.SetSprLibAst(stm.stage1_BossSprLibAstArr);

            // (이벤트 구독) 죽었을시 오브젝트풀링 회수 
            enemy.OnDeadEvent = (enemy) => {
                pool.Release(enemy);
            };
        }
    }

#region OBJECT POOL
    Enemy Create() => Instantiate(enemyPref, enemyGroupTf);

    void OnGet(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    void OnDelete(Enemy enemy) => Destroy(enemy);

    public Enemy SpawnEnemy(int maxHp, int dmg)
    {
        Enemy enemy = pool.Get();
        enemy.transform.position = GetRandomCirclePosition(spawnRadius);
        enemy.Init(maxHp, dmg);

        return enemy;
    }
    #endregion
    #region FUNC
    Vector3 GetRandomCirclePosition(float radius)
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0
        );

        return Vector3.zero + dir * radius;
    }
#endregion
#region GIZMOS
    void OnDrawGizmosSelected() {
        const int SEGMENTS = 64;
        Gizmos.color = Color.green;
        Vector2 prev = new Vector2(spawnRadius, 0f);

        for (int i = 1; i <= SEGMENTS; i++)
        {
            float angle = (360f / SEGMENTS) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 next = new Vector2(
                Mathf.Cos(rad) * spawnRadius,
                Mathf.Sin(rad) * spawnRadius
            );

            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endregion
}