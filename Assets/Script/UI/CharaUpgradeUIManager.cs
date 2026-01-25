using TMPro;
using UnityEngine;

/// <summary>
/// 캐릭터 업그레이드 UI 매니저
/// </summary>
public class CharaUpgradeUIManager : MonoBehaviour
{
    public GameObject panelObj;

    public TextMeshProUGUI gradeTxt;
    public TextMeshProUGUI dmgTxt;
    public TextMeshProUGUI atkSpdTxt;
    public TextMeshProUGUI rangeTxt;
    public TextMeshProUGUI critPerTxt;
    public TextMeshProUGUI critDmgPerTxt;

    void Start()
    {
        panelObj.SetActive(false);
    }

#region EVENT
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
        UpdateUI(GM._.crm.charaArr[0]);
    }

    public void UpdateUI(Chara chara)
    {
        gradeTxt.text = $"{chara.Grade}";
        dmgTxt.text = $"{chara.Dmg}";
        atkSpdTxt.text = $"{chara.AttackSpeed}";
        rangeTxt.text = $"{chara.Range}";
        critPerTxt.text = $"{chara.CritPer}";
        critDmgPerTxt.text = $"{chara.CritDmgPer}";
    }
#endregion
}
