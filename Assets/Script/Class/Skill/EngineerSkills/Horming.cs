using UnityEngine;
using static SkillPoolManager;

public class Horming : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 250f; // 회전 속도 (낮을수록 크게 원을 그리며 꺾임)

    [Header("Wave Settings")]
    public float waveFrequency = 5f;  // 싸인파 속도 (얼마나 빨리 좌우로 요동칠지)
    public float waveMagnitude = 1.5f; // 싸인파 폭 (얼마나 크게 좌우로 요동칠지)

    [Header("References")]
    public TargetFinder targetFinder; // TargetFinder 스크립트 연결

    int dmg;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        
        // 컴포넌트가 할당되지 않았다면 자기 자신에서 찾기
        if (targetFinder == null)
        {
            targetFinder = GetComponent<TargetFinder>();
        }

        // 팁: 생성 직후에 미사일이 약간 엉뚱한 방향을 보게 랜덤 회전을 주면, 
        // 처음에 크게 바깥으로 돌았다가 타겟을 향해 꺾여 들어가는 멋진 연출이 나옵니다.
        float randomAngle = Random.Range(-60f, 60f);
        transform.Rotate(0, 0, randomAngle);
    }

    void Update()
    {
        // TargetFinder를 통해 타겟이 존재하는지 확인[cite: 2]
        if (targetFinder != null && targetFinder.CurrentTarget != null)
        {
            // 1. 타겟을 향한 기본 방향 벡터
            Vector2 targetPos = targetFinder.CurrentTarget.transform.position;
            Vector2 dirToTarget = (targetPos - (Vector2)transform.position).normalized;

            // 2. 진행 방향의 수직(직각) 벡터 구하기 (2D 기준)
            Vector2 perpDir = new Vector2(-dirToTarget.y, dirToTarget.x);

            // 3. 시간에 따른 싸인파 계산
            float waveOffset = Mathf.Sin((Time.time - spawnTime) * waveFrequency) * waveMagnitude;

            // 4. 최종 목표 방향 = 기본 방향 + (수직 벡터 * 싸인파 오프셋)
            Vector2 finalDir = (dirToTarget + perpDir * waveOffset).normalized;

            // 5. 목표 방향을 바라보는 2D 각도 계산
            float angle = Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);

            // 6. 부드러운 방향 전환 (멋진 곡선을 그리는 핵심)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        // 타겟 유무와 상관없이 항상 자신의 앞쪽 방향으로 계속 전진
        // (주의: 미사일 2D 스프라이트의 앞부분이 오른쪽(Right)이 아니라 위쪽(Up)이라면 transform.up으로 변경하세요)
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
                // 적과 충돌
        if (col.gameObject.CompareTag(Config.TAG.ENEMY))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy.State == Enemy.STATE.DEAD)
                return;

            //TODO 이펙트

            enemy.OnHit(dmg, false);
            GM._.spm.ReleasePoolDics(SK_IDX.SK_Horming, gameObject);
        }

    }

#region FUNC
    public void Init(Vector3 pos, Vector3 dir, int dmg, float angleOffset)
    {
        transform.position = pos;
        this.dmg = dmg;

        // 발사 방향(각도)을 먼저 계산합니다.
        Vector3 rotatedDir = Quaternion.Euler(0, 0, angleOffset) * dir;
        // 계산된 방향을 클래스 멤버 변수에 저장합니다. (실제 이동 방향)
        float angle = Mathf.Atan2(rotatedDir.y, rotatedDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
#endregion
}
