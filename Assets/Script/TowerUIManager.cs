using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 타워 캐릭터배치 버튼
/// </summary>

[Serializable]
public struct TowerSeatBtn
{
    public bool isLocked;
    public Button seatBtn;
    public Image charaImg;
    public GameObject lockedFrameObj;
    public TextMeshProUGUI priceTxt;
}

[Serializable]
public struct TowerUpgradeBtn
{
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI valTxt;
    public TextMeshProUGUI priceTxt;
}

public class TowerUIManager : MonoBehaviour
{
    public enum SEAT_IDX { CENTER, LEFT, BOTTOM, RIGHT, TOP }

    public GameObject panelObj; // 패널
    public TowerSeatBtn[] charaSeatBtnArr; // 캐릭터 잠김화면
    public TowerUpgradeBtn[] upgradeBtnArr; // 타워 업그레이드 버튼

    readonly int[] seatPriceArr = { 0, 5000, 20000, 50000, 100000 }; // 좌석별 가격

    public int upgradeHpLv = 1; 
    const int MAX_UPGRADE_HP_LV = 9999;
    public readonly int UPGRADE_HP_VAL = 100;

    public int upgradeArmorLv = 1;
    const int MAX_UPGRADE_ARMOR_LV = 999;
    const int UPGRADE_ARMOR_VAL = 1;
    Tower tower;

    void Start()
    {
        tower = GM._.tower;

        panelObj.SetActive(false);

        // 캐릭터 배치버튼 초기화 (CENTER는 기본 오픈)
        for (int i = (int)SEAT_IDX.LEFT; i < charaSeatBtnArr.Length; i++)
        {
            charaSeatBtnArr[i].isLocked = true; 
            // charaSeatBtnArr[i].charaImg.sprite = null; //TODO 이미지 설정
            charaSeatBtnArr[i].lockedFrameObj.SetActive(true); //TODO DB에서 잠금여부 가져오기
            charaSeatBtnArr[i].priceTxt.text = $"{seatPriceArr[i]}";
        }
    }

#region EVENT
    public void OnClickSeatBtn(int idx)
    {
        if( charaSeatBtnArr[idx].isLocked )
        {
            //TODO 좌석 잠금해제 로직
        }
        else
        {
            //TODO 캐릭터 배치 로직
        }
    }

    public void OnClickUpgradeHpBtn()
    {
        Debug.Log("Upgrade HP");
        upgradeHpLv++;
        tower.SetMaxHp();
        UI._.SetTowerHpSlider(tower.Hp, tower.GetMaxHp());
    }

    public void OnClickUpgradeArmorBtn()
    {
        Debug.Log("Upgrade Armor");
        upgradeArmorLv++;
        tower.SetArmor();
        UI._.SetTowerArmorTxt(tower.Armor);
    }
#endregion
#region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }
    /// <summary>
    /// 업그레이드된 최대 체력 값 반환
    /// </summary>
    public int GetUpgradeHpVal() => upgradeHpLv * UPGRADE_HP_VAL;
    /// <summary>
    /// 업그레이드된 방어력 값 반환
    /// </summary>
    public int GetUpgradeArmorVal() => upgradeArmorLv * UPGRADE_ARMOR_VAL;
#endregion
}
