using System.Collections.Generic;
using UnityEngine;
using static Config;

/// <summary>
/// 스킬 초기값 및 업그레이드 단위 관리 구조체
/// </summary>
[System.Serializable]
public struct SkillValue
{
    public enum Type {SkillLv, GradeLv}
    public Type type; // 타입: 스킬레벨 또는 등급레벨
    public float def; // 초기 수치
    public float unit; // 업그레이드 단위 증가량
}

/// <summary>
/// 스킬 데이터 에셋
/// </summary>
[System.Serializable]
public class SkillAsset
{
    public string Name;
    public CHR_GRADE Grade;
    public Sprite Img;
    public string Desc; // 스킬 내용
    public List<SkillValue> ValueList;
    public int MaxLv;
    public int PriceUnit; // 강화비용 계수 단위 (예: 1000, 2000 등)
}

/// <summary>
/// 캐릭터 등급별 스킬 에셋
/// </summary>
[CreateAssetMenu(fileName = "CharaDataAsset", menuName = "Scriptable Ojbect/CharaSkillAsset")]
public class CharaSkillAsset : ScriptableObject
{
    public SkillAsset[] skillAssetArr;
}
