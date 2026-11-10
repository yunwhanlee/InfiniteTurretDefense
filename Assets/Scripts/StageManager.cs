using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using static Config;

public class StageManager : MonoBehaviour
{
    const int STAGE_LOOP_MAX = 10; // 스테이지 일반, 엘리트, 보스 패턴 순환 MAX

    [Header("UI Component")]
    public TextMeshProUGUI stageTxt;
    public Slider progressSlider;

    [Header("스테이지1. 몬스터 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_MonsterSprLibAstArr;

    [Header("스테이지1. 보스 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_BossSprLibAstArr;

    [Header("Value")]
    public MAP_TYPE mapType;
    public int stage; // 스테이지 = 분
    public int prevStageLoopIdx; // 이전 스테이지 순환 인덱스
    public int curStageLoopIdx; // 0~9까지의 스테이지 순환 인덱스
    private Dictionary<int, int> hpCache = new Dictionary<int, int>(); // 거듭 제곱 체력을 저장할 캐시 딕셔너리
    private int startStage;
    private float time;

#region 적 HP 증가량
    [Header("적 스테이지 밸런스 변수")]
    public int curEnemyHp; // 현재 적 HP
    public int nextEnemyHp; // 다음 스테이지 적 HP
    [field:SerializeField] public int EnemyStageHp {get; private set;}
    const int DEF_HP = 3; // 기본 적 HP
    public const float ELITE_INC_HP_RATE = 5; // 엘리트 체력 증가 배율
    public const float BOSS_INC_HP_RATE = 20; // 보스 체력 증가 배율
    const float MIN_HP_RATE = 1.04f; // 스테이지당 HP 증가율 최소값
    public const float INC_HP_RATE = 1.5f; // 스테이지당 HP 증가량
    public const float HP_DECAY = 0.005f; // 스테이지당 HP 배율 감소폭
#endregion

#region 적 처지시 획득 골드량
    const int DEFCOIN = 1; // 기본 코인
    const float ELITE_INC_COIN_RATE = 2.0f; // 엘리트 몬스터 코인 획득 배율
    const float BOSS_INC_COIN_RATE = 4.0f; // 보스 코인 획득 배율
    const float COIN_MULT = 2.0f; // 코인 배율
    const float STAGE_RATE = 0.01f; // 스테이지당 추가 코인 증가량
    [field:SerializeField] public int EnemyStageCoin {get; private set;}
#endregion
#region 적 공격력 증가량
    const int DEF_DMG = 1; // 기본 적 공격력
    public const float ELITE_INC_DMG_RATE = 1.75f;
    const float BOSS_INC_DMG_RATE = 3.5f;
    public const int INC_DMG_VAL = 2; // 공격력 증가량 (정수)
    [field:SerializeField] public int EnemyStageDmg {get; private set;}
#endregion

    void Start()
    {
        stage = DB._.stage;
        startStage = stage;
        prevStageLoopIdx = stage % STAGE_LOOP_MAX;
        curStageLoopIdx = prevStageLoopIdx;
        mapType = (MAP_TYPE)(stage / (int)MAP_TYPE.MAX);

        time = 0;
        Time.timeScale = 7;

        stageTxt.text = $"{mapType} : 스테이지 {stage}";

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
                // 현재 분과 초 먼저 계산
                int minute = Mathf.FloorToInt(time / 60f); // 순수하게 경과한 '분'
                int sec = (int)time % 60;

                // 스테이지는 베이스 스테이지(startStage) + 경과한 분(passedMinute)으로 동기화
                stage = startStage + minute;
                prevStageLoopIdx = stage % STAGE_LOOP_MAX;

                // 다음 스테이지로 갱신될 때
                if(prevStageLoopIdx != curStageLoopIdx)
                {
                    curStageLoopIdx = prevStageLoopIdx;

                    switch (curStageLoopIdx)
                    {
                        case 2: case 4: case 6: case 8:
                            Debug.Log($"{curStageLoopIdx} => 적 소환 : 엘리트 몬스터");
                            // TODO
                            break;
                        case 0: // 10, 20, 30 스테이지는 나머지가 무조건 0
                            Debug.Log($"{curStageLoopIdx} => 적 소환 : 보스");
                            // TODO
                            break;
                        default:
                            break;
                    }
                }

                // 맵 타입 캐스팅 시 범위 초과 방지 (순환 구조)
                mapType = (MAP_TYPE)(stage % (int)MAP_TYPE.MAX);
                stageTxt.text = $"{mapType} : 스테이지 {stage}";

                // 진행도 계산 (10초마다 0.2씩 증가)
                float progressPer = sec / 10 * 0.2f;

                // 적 HP
                float curStageHp = GetStageHp(stage);
                float nextStageHp = GetStageHp(stage + 1);
                EnemyStageHp = Mathf.CeilToInt((float)(curStageHp + (nextStageHp - curStageHp) * progressPer));
                Debug.Log($"적 HP= {curStageHp} + ({nextStageHp} - {curStageHp}) * {progressPer} = {EnemyStageHp}");

                // 적 공격력
                int curStageDmg = DEF_DMG + (stage - 1) * INC_DMG_VAL; // 현재 스테이지 공격력
                int nextStageDmg = DEF_DMG + stage * INC_DMG_VAL; // 다음 스테이지 공격력
                EnemyStageDmg = Mathf.CeilToInt((float)(curStageDmg + (nextStageDmg - curStageDmg) * progressPer));
                Debug.Log($"적 공격력= {curStageDmg} + ({nextStageDmg} - {curStageDmg}) * {progressPer} = {EnemyStageDmg}");

                // 일반 적 코인 획득량
                EnemyStageCoin = GetCalcCoin(EnemyStageHp, ENEMY_TYPE.NORMAL);

                // UI
                UI._.timeTxt.text = $"{minute + startStage} : {sec}";
                UI._.EnemyHpTxt.text = $"체력 : {EnemyStageHp}";
                UI._.EnemyDmgTxt.text = $"데미지 : {EnemyStageDmg}";

                yield return WFS_10; // 10초마다 갱신
                time += 10;
            }
            else
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 기획표와 일치하는 스테이지별 HP 누적 계산 함수
    /// </summary>
    private int GetStageHp(int targetStage)
    {
        // 1. 초기 1스테이지 기본값 세팅
        if (hpCache.Count == 0) 
            hpCache[1] = DEF_HP;

        // 2. 이미 계산해서 저장해둔 스테이지면 for문 없이 딕셔너리에서 즉시 반환 (연산 비용 0)
        if (hpCache.ContainsKey(targetStage)) 
            return hpCache[targetStage];

        // 3. 캐시에 없는 스테이지라면, '가장 마지막으로 저장된 스테이지'부터 이어서 계산
        int lastCachedStage = hpCache.Count;
        int hp = hpCache[lastCachedStage];

        for (int i = lastCachedStage; i < targetStage; i++)
        {
            float rate = CalcIncHpRate(i);
            hp = Mathf.RoundToInt(hp * rate);

            // 지나가는 모든 스테이지의 정수 HP 수치를 전부 딕셔너리에 저장
            hpCache[i + 1] = hp;
        }

        return hpCache[targetStage];
    }

    /// <summary>
    /// HP 증가 배율 계산
    /// </summary>
    private float CalcIncHpRate(int stage) => Mathf.Max(MIN_HP_RATE, INC_HP_RATE - (stage - 1) * HP_DECAY);

    /// <summary>
    /// 적 처치시 획득 골드량
    /// </summary>
    public int GetCalcCoin(int hp, ENEMY_TYPE enemyType)
    {
        int eliteHp = Mathf.RoundToInt(hp * ELITE_INC_HP_RATE);
        int bossHp = Mathf.RoundToInt(hp * BOSS_INC_HP_RATE);

        // 타입에 따른 HP 증가률
        float resultHp = enemyType == ENEMY_TYPE.NORMAL? hp : enemyType == ENEMY_TYPE.ELITE? eliteHp : bossHp;
        // 타입에 따른 추가 곱연산 변수
        float extraMulti = enemyType == ENEMY_TYPE.NORMAL? 1 : enemyType == ENEMY_TYPE.ELITE? ELITE_INC_COIN_RATE : BOSS_INC_COIN_RATE;
        // 코인 획득량 계산
        int coin = DEFCOIN + Mathf.RoundToInt(Mathf.Sqrt(resultHp) * COIN_MULT * (1 + (stage-1) * STAGE_RATE));

        // 최종 결과 계산
        return Mathf.RoundToInt(coin * extraMulti);
    }
#endregion
}