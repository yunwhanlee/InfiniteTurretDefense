using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static Config;

/// <summary>
/// 타워 캐릭터배치 버튼 클래스
/// </summary>
[Serializable]
public struct TowerPlaceBtn
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
    public enum UPG_IDX { HP, ARMOR, HEAL }

    public GameObject panelObj; // 패널
    public TowerPlaceBtn[] charaPlaceBtnArr; // 캐릭터 배치 버튼
    public bool[] isPlaceLockedArr; // 캐릭터 배치 잠김여부 배열

    public TowerUpgradeBtn[] upgradeBtnArr; // 타워 업그레이드 버튼

    public bool isChangeCharaMode; // 캐릭터 변경 모드 트리거
    public CHR_PLACE changePlaceIdx; // 변경할 캐릭터 배치 위치

    // 캐릭터 배치
    readonly int[] placePriceArr = { 0, 5000, 20000, 50000, 100000 }; // 좌석별 가격

    // 업그레이드
    public static int UPGRADE_HP_UNIT = 100;
    private int upgradeHpLv = 1; 
    private int upgradeArmorLv = 1;
    private int upgradeHealLv = 1;

    Tower tower;

    void Awake()
    {
        //TODO (DB로드) 캐릭터 배치 잠금해제
        isPlaceLockedArr = new bool[] { false, true, true, true, true };
    }

    void Start()
    {
        tower = GM._.tower;
        panelObj.SetActive(false);

        // 캐릭터 배치버튼 초기화 (CENTER는 기본 오픈)
        UpdatePlaceUI();

        // 업그레이드 버튼 초기화
        UpdateUpgradeUI();
    }

#region EVENT
    /// <summary>
    /// 캐릭터 배치 버튼 (현재 캐릭터가 배치되어있는 위치 표시)
    /// </summary>
    /// <param name="idx">enum형 CENTER, LEFT, BOTTOM, RIGHT, TOP</param>
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
                        isPlaceLockedArr[idx] = false;
                        charaPlaceBtnArr[idx].isLocked = false;
                        // UI 업데이트
                        charaPlaceBtnArr[idx].lockedFrameObj.SetActive(false);

                        Util._.ShowSuccessMessage("구매 성공!");
                    }
                );
            }
            else
            {
                Util._.ShowErrorMessage("코인이 부족합니다.");
            }
        }
        else
        {
            Debug.Log("캐릭터 카드 콜렉션 UI창 표시");

            Util._.ShowInteractionMessage("변경할 캐릭터 카드를 선택해주세요.",
                () => {
                    Util._.toastMsgPopup.SetActive(false);
                    UI._.charaCltUI.panelObj.SetActive(false);
                }
            );
            UI._.charaCltUI.ShowPanel();

            // 캐릭터 변경 모드 ON
            isChangeCharaMode = true;
            changePlaceIdx = (CHR_PLACE)idx;
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
    private int GetUnlockedCardCnt()
    {
        int cnt = 0;
        foreach(var card in UI._.charaCltUI.charaCardArr)
        {
            if (!card.IsLocked) cnt++;
            if (cnt >= 2) break; // 2개 찾으면 바로 중단
        }

        return cnt;
    }

    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }

    public void UpdatePlaceUI()
    {
        for (int i = 0; i < charaPlaceBtnArr.Length; i++)
        {
            // 1. 잠금여부 및 가격 설정
            charaPlaceBtnArr[i].isLocked = isPlaceLockedArr[i];
            charaPlaceBtnArr[i].lockedFrameObj.SetActive(charaPlaceBtnArr[i].isLocked);
            charaPlaceBtnArr[i].priceTxt.text = $"{placePriceArr[i]}";

            // 2. 해당 자리에 배치된 카드 찾기
            CHR_PLACE currentPlace = (CHR_PLACE)i;
            var foundCard = UI._.charaCltUI.FindCharaCard(currentPlace);

            // 3. 카드가 있을 때만 이미지 변경, 없으면 이미지 끄기
            if (foundCard)
            {
                charaPlaceBtnArr[i].charaImg.enabled = true; // 이미지 켜기
                charaPlaceBtnArr[i].charaImg.sprite = foundCard.GetCurGradeSprite();
            }
            else
            {
                // 배치된 캐릭터가 없으면 이미지를 끄거나 기본 이미지로 설정
                charaPlaceBtnArr[i].charaImg.enabled = false; 
            }
        }
    }

    public void UpdateUpgradeUI()
    {
        upgradeBtnArr[(int)UPG_IDX.HP].UpdateHpCardUI(upgradeHpLv);
        upgradeBtnArr[(int)UPG_IDX.ARMOR].UpdateArmorCardUI(upgradeArmorLv);
        upgradeBtnArr[(int)UPG_IDX.HEAL].UpdateHealCardUI(upgradeHealLv);
    }
#endregion
}
