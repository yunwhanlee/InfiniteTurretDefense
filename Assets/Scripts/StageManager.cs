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
    const float STAGE_UP_PER = 1.22f;

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
                double curStageHp = DEF_HP * Mathf.Pow(STAGE_UP_PER, stage - 1);
                double nextStageHp = DEF_HP * Mathf.Pow(STAGE_UP_PER, stage);

                int sec = (int)time % 60; // 1분 단위 현재 몇초인지
                float progressPer = 0; // 스테이지 진행도 %

                // 현재 스테이지 진행도 %
                switch (sec)
                {
                    case 0: ++stage; progressPer = 0;
                        break;
                    case 10: progressPer = 0.2f;
                        break;
                    case 20: progressPer = 0.4f;
                        break;
                    case 30: progressPer = 0.6f;
                        break;
                    case 40: progressPer = 0.8f;
                        break;
                    case 50: progressPer = 1.0f;
                        break;
                    case 60: stage += 1;
                        break;
                }

                // float progress = time % 60f / 60f;
                // Debug.Log("progress= " + progress);

                Debug.Log($"stage({stage}):: sec= {sec}, per= {progressPer}");

                // 실제 적 Hp
                enemyHp = Mathf.CeilToInt((float)(curStageHp + (nextStageHp - curStageHp) * progressPer));
                Debug.Log($"적 HP= {curStageHp} + ({nextStageHp} - {curStageHp}) * {progressPer}= {enemyHp}");

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

// 10 0 
// 20 20
// 30 40
// 40 60
// 50 80 
// 60 100