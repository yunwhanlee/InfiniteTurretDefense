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

    const int DEF_HP = 8;
    const int DEF_ATK = 2;
    const float STAGE_INCREASE_HP_UP_PER = 1.22f; // 스테이지 

    [Header("스테이지1. 몬스터 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_MonsterSprLibAstArr;
    [Header("스테이지1. 보스 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_BossSprLibAstArr;

    // VALUE
    private float time;
    public STAGE_TYPE type;
    public int stage; // 스테이지 레벨 분 단위
    public int enemyHp;

    // UI
    public TextMeshProUGUI stageTxt;
    public Slider progressSlider;

    void Start()
    {
        time = 0;
        stage = 0;
        Time.timeScale = 3;
        StartCoroutine(CoTimer());
    }

    IEnumerator CoTimer()
    {
        while (true)
        {
            // if(GM._.tower.State != Tower.STATE.DEAD)
            // {
                // 1. 현재 분(minute)과 초(sec)를 먼저 계산
                int minute = Mathf.FloorToInt(time / 60f); 
                int sec = (int)time % 60;

                // 2. 스테이지는 분(minute) + 1 로 무조건 동기화
                stage = minute + 1; // time이 0~59초면 stage 1, 60~119초면 stage 2

                // 3. 진행도(progressPer) 계산 (10초마다 0.2씩 증가)
                float progressPer = sec / 10 * 0.2f;

                // 4. 결정된 스테이지를 바탕으로 HP 기준점 계산! (여기가 핵심)
                double curStageHp = DEF_HP * Mathf.Pow(STAGE_INCREASE_HP_UP_PER, stage - 1);
                double nextStageHp = DEF_HP * Mathf.Pow(STAGE_INCREASE_HP_UP_PER, stage);

                // 5. 최종 적 HP 적용
                enemyHp = Mathf.CeilToInt((float)(curStageHp + (nextStageHp - curStageHp) * progressPer));
                
                Debug.Log($"stage({stage}):: sec= {sec}, per= {progressPer}");
                Debug.Log($"적 HP= {curStageHp} + ({nextStageHp} - {curStageHp}) * {progressPer} = {enemyHp}");

                yield return Config.WFS_10;
                time += 10;
            // }
            // else
            // {
            //     yield return null;
            // }
        }
    }
}