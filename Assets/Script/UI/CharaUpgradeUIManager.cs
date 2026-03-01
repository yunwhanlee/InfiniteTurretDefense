using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Config;
using System.Collections.Generic;

/// <summary>
///* 캐릭터 업그레이드 UI 매니저
/// </summary>
public class CharaUpgradeUIManager : MonoBehaviour
{
    public GameObject panelObj;
    public GameObject placedNoticeIcon; // 배치중 아이콘
    public GameObject arrowBtnGroup;    // 화살표 버튼 그룹

    public Image charaImg; // 캐릭터 이미지

    public TextMeshProUGUI gradeTxt;
    public TextMeshProUGUI dmgTxt;
    public TextMeshProUGUI atkSpdTxt;
    public TextMeshProUGUI rangeTxt;
    public TextMeshProUGUI critPerTxt;
    public TextMeshProUGUI critDmgPerTxt;

    public TextMeshProUGUI gradeUpTxt;

    public SkillCard[] skillCardArr;

    CharaManager crm;

    void Start()
    {
        crm = GM._.crm;

        panelObj.SetActive(false);
        placedNoticeIcon.SetActive(false);
    }

#region EVENT
    public void OnClickCloseBtn()
    {
        panelObj.SetActive(false);
        crm.InActiveCharaRangeCircle();
    }

    /// <summary>
    /// 등급업 버튼 클릭
    /// </summary>
    public void OnClickGradeUpBtn()
    {
        CHR_CATE cate = GM._.crm.curSelectedChara.Cate;
        var curCard = UI._.charaCltUI.charaCardArr[(int)cate];

        bool isSuccess = curCard.GradeUp(cate);

        if (isSuccess)
        {
            Util._.ShowUnderBarMessage("캐릭터 등급업 완료!");
            UpdateUI(curCard);
        }
        else
            Util._.ShowErrorMessage("캐릭터카드가 부족합니다.");
    }

    /// <summary>
    /// 현재 배치된 캐릭터 선택 좌우 이동
    /// </summary>
    /// <param name="sign">1 : 오른쪽 방향, -1 : 왼쪽 방향</param>
    public void OnClickArrowBtn(int sign)
    {
        int curIdx = crm.curCharaList.IndexOf(crm.curSelectedChara);
        int idx = curIdx + sign;

        // 현재 선택된 캐릭터 변경
        crm.curSelectedChara = crm.curCharaList[idx % crm.curCharaList.Count];

        // 캐릭터 선택 및 UI 업데이트
        crm.SelectChara(crm.curSelectedChara);
        UpdateUI(crm.curSelectedChara);
    }

    /// <summary>
    /// 스킬레벨 업그레이드 버튼 클릭
    /// </summary>
    /// <param name="lv">스킬</param>
    public void OnClickUpgradeSkillBtn(int idx)
    {
        Chara chara = crm.curSelectedChara;

        chara.LevelUpSkill((CHR_GRADE)idx);
        UpdateUI(chara);

        Util._.ShowUnderBarMessage("스킬 업그레이드 성공!");
    }
#endregion
#region FUNC
    /// <summary>
    /// 인게임에서 캐릭터 클릭 또는 메뉴>캐릭터 버튼 클릭 시, 패널 표시 (배치된 캐릭터만 표시)
    /// </summary>
    public void ShowPanel()
    {
        panelObj.SetActive(true);
        arrowBtnGroup.SetActive(true); // 화살표 이동 활성화
        UpdateUI(crm.curSelectedChara);
    }

    /// <summary>
    /// 캐릭터카드 콜랙션에서 카드 클릭 시, 패널 표시 (모든 캐릭터 표시)
    /// </summary>
    /// <param name="card"></param>
    public void ShowPanel(CharaCard card)
    {
        panelObj.SetActive(true);
        arrowBtnGroup.SetActive(false); // 화살표 이동 비활성화
        UpdateUI(card);
    }

    /// <summary> 인게임에서 캐릭터 선택시 업데이트 </summary>
    public void UpdateUI(Chara chara)
    {
        // 배치중 아이콘 표시
        placedNoticeIcon.SetActive(true); //isPlaced);

        // 캐릭터 이미지 교체
        charaImg.sprite = chara.defaultSpr;

        // UI텍스트 최신화
        gradeTxt.text = $"{chara.Grade}";
        dmgTxt.text = $"{chara.Dmg}";
        atkSpdTxt.text = $"{chara.AttackSpeed}";
        rangeTxt.text = $"{chara.Range}";
        critPerTxt.text = $"{chara.CritPer}";
        critDmgPerTxt.text = $"{chara.CritDmgPer}";
        
        // 등급업 텍스트 최신화
        CharaCard curCard = UI._.charaCltUI.charaCardArr[(int)chara.Cate];
        gradeUpTxt.text = $"등급업 {curCard.GetUserData().cardCnt} / {curCard.GetNextGradeCardCnt()}";

        // Skill 최신화
        for(int i = 0; i < skillCardArr.Length; i++)
        {
            SkillCard skillcard = skillCardArr[i];
            skillcard.UpdateUI(i, chara.CharaSkill, chara.SkillLvArr[i], chara.Grade);

            int lv = chara.SkillLvArr[i];
            var skillAst = chara.CharaSkill.skillAssetArr[i];

            switch(i)
            {
                case (int)CHR_GRADE.NORMAL:
                    skillcard.UpdateDescUI_Normal(lv, skillAst.Desc, chara.Dmg, chara.DmgUpgUnit);
                    break;
                case (int)CHR_GRADE.RARE:
                case (int)CHR_GRADE.EPIC:
                case (int)CHR_GRADE.UNIQUE:
                case (int)CHR_GRADE.LEGEND:
                case (int)CHR_GRADE.MYTHIC:
                case (int)CHR_GRADE.PRIME:
                    skillcard.UpdateDescUI(lv, skillAst.Desc, skillAst.ValueList);
                    break;
            }
        }
    }

    /// <summary> 콜렉션에서 캐릭터카드 선택시 업데이트 </summary>
    public void UpdateUI(CharaCard card)
    {
        CharaDataAsset data = card.GetCharaDataAsset();

        // 배치중 아이콘 표시
        bool isPlaced = card.GetPlace() != CHR_PLACE.NONE;
        placedNoticeIcon.SetActive(isPlaced);

        // 캐릭터 이미지 교체
        charaImg.sprite = card.GetIconSprite();

        // UI텍스트 최신화
        gradeTxt.text = $"{card.GetGrade()}";
        dmgTxt.text = $"{data.baseDmg}";
        atkSpdTxt.text = $"{data.baseAttackSpeed}";
        rangeTxt.text = $"{data.baseRange}";
        critPerTxt.text = $"{Chara.DEF_CRIT}%";
        critDmgPerTxt.text = $"{Chara.DEF_CRITDMG * 100}%";

        // 등급업 텍스트 최신화
        gradeUpTxt.text = $"등급업 {card.GetUserData().cardCnt} / {card.GetNextGradeCardCnt()}";

        // Skill 최신화
        for(int i = 0; i < skillCardArr.Length; i++)
        {
            SkillCard skillcard = skillCardArr[i];
            skillcard.UpdateUI(i, data.charaSkillAsset, card.GetSkillLv(i), card.GetGrade());
        }

        // Skill 최신화
        for(int i = 0; i < skillCardArr.Length; i++)
        {
            SkillCard skillcard = skillCardArr[i];
            skillcard.UpdateUI(i, data.charaSkillAsset, card.GetSkillLv(i), card.GetGrade());

            int lv = card.GetSkillLv(i);
            var skillAst = data.charaSkillAsset.skillAssetArr[i];

            switch(i)
            {
                case (int)CHR_GRADE.NORMAL:
                    skillcard.UpdateDescUI_Normal(lv, skillAst.Desc, card.GetCharaDataAsset().baseDmg, card.GetCharaDataAsset().dmgUpgUnit);
                    break;
                case (int)CHR_GRADE.RARE:
                case (int)CHR_GRADE.EPIC:
                case (int)CHR_GRADE.UNIQUE:
                case (int)CHR_GRADE.LEGEND:
                case (int)CHR_GRADE.MYTHIC:
                case (int)CHR_GRADE.PRIME:
                    skillcard.UpdateDescUI(lv, skillAst.Desc, skillAst.ValueList);
                    break;
            }
        }
    }
#endregion
}
