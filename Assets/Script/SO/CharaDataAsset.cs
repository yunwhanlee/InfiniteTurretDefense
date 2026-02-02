using UnityEngine;
using static Config;

// 에셋 메뉴에서 쉽게 생성 가능하게 함
[CreateAssetMenu(fileName = "CharaDataAsset", menuName = "Scriptable Ojbect/CharaDataAsset")]
public class CharaDataAsset : ScriptableObject
{
    [Header("기본 정보")]
    public CHR_CARD_IDX cardIdx;
    public CHR_GRADE grade;
    public string charaName;
    public Sprite icon;
    public GameObject charaPrefab; // 인게임 생성될 프리팹

    [Header("스탯 정보")]
    public int baseDmg;
    public float baseAttackSpeed;
    public float baseRange;
    public float baseCritPer;
    public float baseCritDmgPer;
}