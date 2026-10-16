using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Config;

/// <summary>
///* 캐릭터 업그레이드UI의 스킬카드
/// </summary>
public class SkillCard : MonoBehaviour
{
    // UI
    public GameObject lockFrame;
    public TextMeshProUGUI lockedTxt;
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI descTxt;
    public TextMeshProUGUI priceTxt;
    public TextMeshProUGUI lvTxt;
    public Image iconImg;

    // Data


#region FUNC
    /// <summary>
    /// 스킬레벨에 따른 강화비용 계산
    /// </summary>
    /// <param name="priceUnit">가격 계수 단위</param>
    /// <param name="skillLv">스킬 레벨</param>
    private int CalcPrice(int priceUnit, int skillLv)
    {
        return priceUnit + skillLv * (skillLv - 1) * priceUnit / 2;
    }

    /// <summary>
    /// 스킬 내용 수치 채우기
    /// </summary>
    /// 
    private void SetDescribe(string msg, float val1, float val2 = -9999)
    {
        if(val2 == -9999)
            string.Format(msg, val1);
        else
            string.Format(msg, val1, val2);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx">스킬카드 프리팹 인덱스</param>
    /// <param name="chara">현재 선택한 캐릭터</param>
    public void UpdateUI(int idx, CharaSkillAsset charaSkill, int skillLv, CHR_GRADE curGrade)
    {
        SkillAsset skillAsset = charaSkill.skillAssetArr[idx];
        // int skillLv = chara.SkillLvArr[idx];

        // 잠김상태 여부
        bool isOverGrade = idx > (int)curGrade;
        lockFrame.SetActive(isOverGrade);
        // UI 업데이트
        lockedTxt.text = $"{(CHR_GRADE)idx}등급 잠금해제";
        titleTxt.text = skillAsset.Name;
        priceTxt.text = $"{CalcPrice(skillAsset.PriceUnit, skillLv)}";
        lvTxt.text = $"LV.{skillLv}/{skillAsset.MaxLv}";
        iconImg.sprite = skillAsset.Img;
    }

    /// <summary>
    /// (캐릭터카드) 스킬1 상세내용 업그레이드 최신화 (고정 : 업그레이드시 공격력 증가)
    /// </summary>
    /// <param name="skillLv">스킬레벨</param>
    /// <param name="msg">스킬내용</param>
    /// <param name="dmg">등급별 초기데미지</param>
    /// <param name="unit">등급별 업그레이드 단위 증가량</param>
    public void UpdateDescUI_Normal(int skillLv, string msg, int dmg, float unit)
    {
        descTxt.text = string.Format(msg, dmg + skillLv * Mathf.Round(dmg * unit));
    }
    /// <summary>
    /// (인게임 캐릭터 클릭) 스킬 1 상세내용 업그레이드 최신화
    /// </summary>
    public void UpdateDescUI_Normal(string msg, int dmg)
    {
        descTxt.text = string.Format(msg, dmg);
    }

    /// <summary>
    /// 스킬 상세내용 업데이트 (데이터 기반 만능 함수)
    /// </summary>
    public void UpdateDescUI(int skillLv, CHR_GRADE curGrade, SkillAsset skillAsset)
    {
        try
        {
            // 스킬 값의 개수만큼 배열 생성 (string.Format에 넣을 용도)
            object[] args = new object[skillAsset.ValueList.Count];

            for (int i = 0; i < skillAsset.ValueList.Count; i++)
            {
                SkillValue val = skillAsset.ValueList[i];
                float finalValue = 0;

                if (val.type == SkillValue.Type.SkillLv)
                {
                    // 1. 스킬 레벨 비례 수치
                    finalValue = val.def + (skillLv * val.unit);
                }
                else if (val.type == SkillValue.Type.GradeLv)
                {
                    // 2. 캐릭터 등급 비례 수치 (현재 등급 - 스킬 해금 등급)
                    int gradeDiff = Mathf.Max(0, (int)curGrade - (int)skillAsset.Grade);
                    finalValue = val.def + (gradeDiff * val.unit);
                }

                args[i] = finalValue;
            }

            // {0}, {1}, {2} ... 등 데이터 개수에 상관없이 알아서 텍스트 매핑
            descTxt.text = string.Format(skillAsset.Desc, args);
        }
        catch(System.Exception err)
        {
            Debug.LogError(err);
        }
    }
#endregion
}