using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 타워 캐릭터배치 버튼 클래스
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

/// <summary>
/// 타워 업그레이드 버튼 클래스
/// </summary>
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

/// <summary>
/// 타워 업그레이드 UI 매니저
/// </summary>
public class TowerUpgradeUIManager : MonoBehaviour
{
    public enum SEAT_IDX { CENTER, LEFT, BOTTOM, RIGHT, TOP }
    public enum UPG_IDX { HP, ARMOR, HEAL }

    public GameObject panelObj; // 패널
    public TowerSeatBtn[] charaPlaceBtnArr; // 캐릭터 배치 버튼
    public TowerUpgradeBtn[] upgradeBtnArr; // 타워 업그레이드 버튼

    //TODO DB화 하기
    /// <summary> 캐릭터 배치 잠김여부 배열 </summary>
    public bool[] DB_isPlaceLockedArr;

    // 캐릭터 배치
    readonly int[] placePriceArr = { 0, 5000, 20000, 50000, 100000 }; // 좌석별 가격

    // 업그레이드
    public static int UPGRADE_HP_UNIT = 100;
    private int upgradeHpLv = 1; 
    private int upgradeArmorLv = 1;
    private int upgradeHealLv = 1;

    Tower tower;
    CharaManager crm;

    void Awake()
    {
        //TODO (DB로드) 캐릭터 배치 잠금해제
        DB_isPlaceLockedArr = new bool[] { false, true, true, true, true };
    }

    void Start()
    {
        tower = GM._.tower;
        crm = GM._.crm;

        panelObj.SetActive(false);

        // 캐릭터 배치버튼 초기화 (CENTER는 기본 오픈)
        for (int i = (int)SEAT_IDX.LEFT; i < charaPlaceBtnArr.Length; i++)
        {
            // 잠김여부
            charaPlaceBtnArr[i].isLocked = DB_isPlaceLockedArr[i];
            bool isLocked = charaPlaceBtnArr[i].isLocked;
            charaPlaceBtnArr[i].lockedFrameObj.SetActive(isLocked); // 잠김 프레임

            // charaSeatBtnArr[i].charaImg.sprite = null; //TODO 이미지 설정
            charaPlaceBtnArr[i].priceTxt.text = $"{placePriceArr[i]}"; // 가격
        }

        // 업그레이드 버튼 초기화
        upgradeBtnArr[(int)UPG_IDX.HP].UpdateHpCardUI(upgradeHpLv);
        upgradeBtnArr[(int)UPG_IDX.ARMOR].UpdateArmorCardUI(upgradeArmorLv);
        upgradeBtnArr[(int)UPG_IDX.HEAL].UpdateHealCardUI(upgradeHealLv);
    }

#region EVENT
    public void OnClickPlaceBtn(int idx)
    {
        if(charaPlaceBtnArr[idx].isLocked)
        {
            int price = placePriceArr[idx];

            // 좌석 잠금해제
            if(GM._.Coin >= price)
            {
                Util._.ShowConfirmPopup("잠금 해제", "정말로 구매하시겠습니까?", "네",
                    () => {
                        GM._.Coin -= price;

                        // 데이터 업데이트
                        DB_isPlaceLockedArr[idx] = false;
                        charaPlaceBtnArr[idx].isLocked = false;
                        // UI 업데이트
                        charaPlaceBtnArr[idx].lockedFrameObj.SetActive(false);

                        Util._.SuccessMessage("구매 성공!");
                    }
                );

            }
            else
            {
                Util._.ErrorMessage("코인이 부족합니다.");
            }
        }
        else
        {
            Debug.Log("캐릭터 카드 콜렉션 UI창 표시");
            UI._.charaCltUI.ShowPanel();
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
