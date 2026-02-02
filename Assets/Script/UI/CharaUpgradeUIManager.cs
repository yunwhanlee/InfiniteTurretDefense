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
    public void ShowPanel()
    {
        panelObj.SetActive(true);
        UpdateUI(GM._.crm.curSelectedChara);
    }

    public void UpdateUI(Chara chara)
    {
        // 현재 캐릭터가 배치중인지
        bool isPlaced = GM._.crm.curCharaList.Exists(_chara => _chara.place == chara.place);

        // 배치중 아이콘 표시
        placedNoticeIcon.SetActive(isPlaced);

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
#endregion
}
