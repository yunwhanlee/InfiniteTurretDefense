using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    const int DEF_HP = 10;
    const int DEF_DMG = 5;

    // 오브젝트 풀링
    public Transform enemyGroupTf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}

    [SerializeField] float totalTime;
    [SerializeField] float time;
    [SerializeField] float span;
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
                UI._.EnemyCntTxt.text = $"몬스터 수 : {value}";
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

    void Start()
    {
        pool = new ObjectPool<Enemy>(
            Create, OnGet, OnRelease, OnDelete, maxSize: 20
        );

        time = span;

        KillCnt = 0;
        EnemyCnt = 0;
        EnemyHp = DEF_HP;
        EnemyDmg = DEF_DMG;
    }

    void Update()
    {
        totalTime += Time.deltaTime;
        time += Time.deltaTime;

        if(time > span)
        {
            time = 0;

            const int OFFSET_SEC = 10;

            if(totalTime >= OFFSET_SEC)
            {
                float calTime = totalTime - OFFSET_SEC;

                // 적 체력 및 공격력 업데이트
                EnemyHp = DEF_HP + (int)(calTime / 2.5f);
                EnemyDmg = DEF_DMG + (int)(calTime / OFFSET_SEC);
            }

            // 적 생성
            Enemy enemy = SpawnEnemy(enemyHp, enemyDmg);

            // 카운트 증가
            EnemyCnt++;

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