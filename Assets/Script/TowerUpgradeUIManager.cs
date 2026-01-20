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
public class TowerUpgradeBtn
{
    const int MAX_UPGRADE_HP_LV = 5; // 9999;
    const int MAX_UPGRADE_ARMOR_LV = 999;
    const int MAX_UPGRADE_HEAL_LV = 999;

    public GameObject btnObj;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI valTxt;
    public TextMeshProUGUI priceTxt;
    
    // 레벨 문자열 반환
    private string GetLv(int lv, int maxLv)
    {
        return lv < maxLv ? $"Lv.{lv}" : "Lv.MAX";
    }

    private void ActiveBtn(bool isNotMaxLv)
    {
        btnObj.SetActive(isNotMaxLv);
    }

    // 체력 업그레이드 UI 업데이트
    public void UpdateHpCardUI(int lv)
    {
        levelTxt.text = GetLv(lv, MAX_UPGRADE_HP_LV);
        valTxt.text = $"+{lv * TowerUpgradeUIManager.UPGRADE_HP_UNIT}";
        priceTxt.text = $"💰{lv * 30}";

        ActiveBtn(lv < MAX_UPGRADE_HP_LV);
    }

    // 방어력 업그레이드 UI 업데이트
    public void UpdateArmorCardUI(int lv)
    {
        levelTxt.text = GetLv(lv, MAX_UPGRADE_ARMOR_LV);
        valTxt.text = $"+{lv}";
        priceTxt.text = $"💰{lv * 150}";

        ActiveBtn(lv < MAX_UPGRADE_ARMOR_LV);
    }

    // 회복력 업그레이드 UI 업데이트
    public void UpdateHealCardUI(int lv)
    {
        levelTxt.text = GetLv(lv, MAX_UPGRADE_HEAL_LV);
        valTxt.text = $"+{lv}";
        priceTxt.text = $"💰{lv * 50}";

        ActiveBtn(lv < MAX_UPGRADE_HEAL_LV);
    }
}

public class TowerUpgradeUIManager : MonoBehaviour
{
    public enum SEAT_IDX { CENTER, LEFT, BOTTOM, RIGHT, TOP }
    public enum UPG_IDX { HP, ARMOR, HEAL }

    public GameObject panelObj; // 패널
    public TowerSeatBtn[] charaSeatBtnArr; // 캐릭터 잠김화면 버튼
    public TowerUpgradeBtn[] upgradeBtnArr; // 타워 업그레이드 버튼

    readonly int[] seatPriceArr = { 0, 5000, 20000, 50000, 100000 }; // 좌석별 가격

    public static int UPGRADE_HP_UNIT = 100;
    private int upgradeHpLv = 1; 
    private int upgradeArmorLv = 1;
    private int upgradeHealLv = 1;

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

        // 업그레이드 버튼 초기화
        upgradeBtnArr[(int)UPG_IDX.HP].UpdateHpCardUI(upgradeHpLv);
        upgradeBtnArr[(int)UPG_IDX.ARMOR].UpdateArmorCardUI(upgradeArmorLv);
        upgradeBtnArr[(int)UPG_IDX.HEAL].UpdateHealCardUI(upgradeHealLv);
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

        // 타워 최대 체력 증가
        tower.AddMaxHp(UPGRADE_HP_UNIT);

        // UI 패널 업데이트
        upgradeBtnArr[(int)UPG_IDX.HP].UpdateHpCardUI(upgradeHpLv);
    }

    public void OnClickUpgradeArmorBtn()
    {
        Debug.Log("Upgrade Armor");
        upgradeArmorLv++;
        // 타워 방어력 증가
        tower.AddArmor(1);
        // UI 패널 업데이트
        upgradeBtnArr[(int)UPG_IDX.ARMOR].UpdateArmorCardUI(upgradeArmorLv);
    }

    public void OnClickUpgradeHealBtn()
    {
        Debug.Log("Upgrade Heal");
        upgradeHealLv++;
        // 타워 회복력 업데이트
        tower.HealVal = upgradeHealLv;
        // UI 패널 업데이트
        upgradeBtnArr[(int)UPG_IDX.HEAL].UpdateHealCardUI(upgradeHealLv);
    }
#endregion
#region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }
#endregion
}
