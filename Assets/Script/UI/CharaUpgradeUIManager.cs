using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 캐릭터 업그레이드 UI 매니저
/// </summary>
public class CharaUpgradeUIManager : MonoBehaviour
{
    public GameObject panelObj;
    public GameObject placedNoticeIcon; // 배치중 아이콘

    public Image charaImg; // 캐릭터 이미지

    public TextMeshProUGUI gradeTxt;
    public TextMeshProUGUI dmgTxt;
    public TextMeshProUGUI atkSpdTxt;
    public TextMeshProUGUI rangeTxt;
    public TextMeshProUGUI critPerTxt;
    public TextMeshProUGUI critDmgPerTxt;

    void Start()
    {
        panelObj.SetActive(false);
        placedNoticeIcon.SetActive(false);
    }

#region EVENT
    public void OnClickCloseBtn()
    {
        panelObj.SetActive(false);
        GM._.crm.InActiveCharaRangeCircle();
    }
    /// <summary>
    /// 현재 배치된 캐릭터 선택 좌우 이동
    /// </summary>
    /// <param name="isRight">True : 오른쪽 방향, False : 왼쪽 방향</param>
    public void OnClickArrowBtn(bool isRight)
    {
        
    }
#endregion
#region FUNC
    /// <summary>
    /// 인게임에서 캐릭터 클릭 또는 메뉴:캐릭터 버튼 클릭
    /// </summary>
    public void ShowPanel()
    {
        panelObj.SetActive(true);
        UpdateUI(GM._.crm.curSelectedChara);
    }

    /// <summary>
    /// 캐릭터카드 콜랙션에서 카드 클릭
    /// </summary>
    /// <param name="card"></param>
    public void ShowPanel(CharaCard card)
    {
        panelObj.SetActive(true);
        UpdateUI(card);
    }

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
    }

    /// <summary> 콜렉션에서 캐릭터카드 선택시 업그레이드 UI 창 표시 </summary>
    public void UpdateUI(CharaCard card)
    {
        CharaDataAsset data = card.GetCharaDataAsset();

        // 배치중 아이콘 표시
        placedNoticeIcon.SetActive(false);

        // 캐릭터 이미지 교체
        charaImg.sprite = card.GetIconSprite();

        // UI텍스트 최신화
        gradeTxt.text = $"{card.GetGrade()}";
        dmgTxt.text = $"{data.baseDmg}";
        atkSpdTxt.text = $"{data.baseAttackSpeed}";
        rangeTxt.text = $"{data.baseRange}";
        critPerTxt.text = $"{data.baseCritPer}";
        critDmgPerTxt.text = $"{data.baseCritDmgPer}";
    }
#endregion
}
