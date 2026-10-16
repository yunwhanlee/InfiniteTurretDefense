using System.Net.WebSockets;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public enum STAGE { FIELD, FOREST, GRAVE, OCEAN, LAVA }
    public STAGE stage;
    public int stageLv; // 스테이지 레벨 분 단위

    [Header("스테이지1. 몬스터 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_MonsterSprLibAstArr;
    [Header("스테이지1. 보스 스프라이트 에셋")]
    public SpriteLibraryAsset[] stage1_BossSprLibAstArr;

    // UI
    public TextMeshProUGUI stageTxt;

}