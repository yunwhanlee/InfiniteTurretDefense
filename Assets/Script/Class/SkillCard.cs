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
    /// 스킬1 상세내용 업그레이드 최신화 (고정 : 업그레이드시 공격력 증가)
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
    /// 스킬 상세내용 업그레이드 최신화
    /// </summary>
    /// <param name="skillLv">스킬레벨</param>
    /// <param name="msg">스킬내용</param>
    /// <param name="skillValList">스킬 <초기값, 단위증가량> 리스트</param>
    public void UpdateDescUI(int skillLv, string msg, List<SkillValue> skillValList)
    {
        try
        {
            if(skillValList.Count == 1)
            {
                descTxt.text = string.Format(
                    msg,
                    skillValList[0].def + skillLv * skillValList[0].unit
                );
            }
            else if(skillValList.Count == 2)
            {
                descTxt.text = string.Format(
                    msg,
                    skillValList[0].def + skillLv * skillValList[0].unit,
                    skillValList[1].def + skillLv * skillValList[1].unit
                );
            }
        }
        catch(Exception err)
        {
            Debug.LogError(err);
        }
    }
#endregion
}
