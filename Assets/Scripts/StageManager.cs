using System.Collections;
using System.Net.WebSockets;
using AssetKits.ParticleImage.Enumerations;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public enum STAGE_TYPE {
        FIELD,          // 황무지
        FOREST,         // 초원
        TOMB,           // 무덤
        OCEAN,          // 해양
        LAVAZONE,       // 용암지대
        MAGIC_DUNGEON,  // 마법던전
        RUINS,          // 유적지
        GOLDEN_TEMPLE,  // 황금신전
    };

    [Header("적 HP 증가량")]
    const int DEF_HP = 3;            // 기본 적 HP
    const float MIN_HP_RATE = 1.04f; // 스테이지당 HP 증가율 최소값
    public float incHpRate = 1.5f;   // 스테이지당 HP 증가량
    public float hpDecay = 0.005f;   // 스테이지당 HP 배율 감소폭
    public int curEnemyHp;           // 현재 적 HP
    public int nextEnemyHp;          // 다음 스테이지 적 HP

    [Header("적 공격력 증가량")]
    const int DEF_ATK = 2;           // 기본 적 공격력
    public int incAtkVal = 3;       // 공격력 증가량 (정수)

    [Header("스테이지1. 몬스터 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_MonsterSprLibAstArr;
    [Header("스테이지1. 보스 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_BossSprLibAstArr;

    // VALUE
    private float time;
    public STAGE_TYPE type;
    public int stage; // 스테이지 = 분
    public int EnemyStageHp;

    // UI
    public TextMeshProUGUI stageTxt;
    public Slider progressSlider;
    public int EnemyStageAtk;

    void Start()
    {
        time = 0;
        stage = 0; //TODO 저장된 스테이지 불러오기
        Time.timeScale = 2;

        // 스테이지 타미어 시작
        StartCoroutine(CoTimer());
    }

#region FUNC
    /// <summary>
    /// 타이머 코루틴
    /// </summary>
    IEnumerator CoTimer()
    {
        while (true)
        {
            if(GM._.tower.State != Tower.STATE.DEAD)
            {
                // 1. 현재 분과 초 먼저 계산
                int minute = Mathf.FloorToInt(time / 60f); // 1分以内では値０
                int sec = (int)time % 60;

                // 2. 스테이지는 분 + 1 로 무조건 동기화
                stage = minute + 1; // time이 0~59초면 stage 1, 60~119초면 stage 2

                // 3. 진행도 계산 (10초마다 0.2씩 증가)
                float progressPer = sec / 10 * 0.2f;
                Debug.Log($"stage({stage}):: sec= {sec}, per= {progressPer}");

                // 5. 적 HP
                float curStageHp = DEF_HP * Mathf.Pow(CalcIncHpRate(stage), stage - 1);
                float nextStageHp = DEF_HP * Mathf.Pow(CalcIncHpRate(stage), stage);
                EnemyStageHp = Mathf.CeilToInt((float)(curStageHp + (nextStageHp - curStageHp) * progressPer));
                Debug.Log($"적 HP= {curStageHp} + ({nextStageHp} - {curStageHp}) * {progressPer} = {EnemyStageHp}");

                // 6. 적 공격력
                int curStageAtk = DEF_ATK + (stage - 1) * incAtkVal; // 현재 스테이지 공격력
                int nextStageAtk = DEF_ATK + stage * incAtkVal; // 다음 스테이지 공격력
                EnemyStageAtk = Mathf.CeilToInt((float)(curStageAtk + (nextStageAtk - curStageAtk) * progressPer));
                Debug.Log($"적 공격력= {curStageAtk} + ({nextStageAtk} - {curStageAtk}) * {progressPer} = {EnemyStageAtk}");

                yield return Config.WFS_10; // 10초마다 갱신
                time += 10;
            }
            else
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// HP 증가 배율 계산
    /// </summary>
    private float CalcIncHpRate(int stage)
    {
        return Mathf.Max(MIN_HP_RATE, incHpRate - (stage - 1) * hpDecay);
    }
#endregion
}