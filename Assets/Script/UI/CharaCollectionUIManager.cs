using System;
using UnityEngine;
using static Config;

//TODO DB에서 캐릭터 Data 만들기
[Serializable]
public class CharaCardData
{
    public CHR_GRADE grade;
    public CHR_PLACE place;
    public int cnt;

    public CharaCardData(CHR_GRADE grade, CHR_PLACE place, int cnt)
    {
        this.grade = grade;
        this.place = place;
        this.cnt = cnt;
    }
}

public class CharaCollectionUIManager : MonoBehaviour
{
    public GameObject panelObj;

    [Header("캐릭터 카드 배열 ※캐릭터 추가시 여기에 카드도 추가")]
    public CharaCard[] charaCardArr;

    void Awake()
    {
        //TODO DB로 캐릭터카드 클래스 데이터 로드
        charaCardArr[(int)CHR_CARD_IDX.ARCHER].SetUp(CHR_CARD_IDX.ARCHER);
        charaCardArr[(int)CHR_CARD_IDX.WARRIOR].SetUp(CHR_CARD_IDX.WARRIOR);
    }

    void Start()
    {
        panelObj.SetActive(false);

        // 캐릭터 콜렉션 카드 UI 업데이트
        UpdateUI();
    }

    #region EVENT
    public void OnClickCharaCardFrameBtn(int cardIdx)
    {
        if(charaCardArr[cardIdx].IsLocked())
            return;

        // 캐릭터 배치변경 모드
        if (UI._.towerUpgUI.isChangeCharaMode)
        {
            var targetPlace = UI._.towerUpgUI.changePlaceIdx; // 배치하려는 위치
            var selectedCard = charaCardArr[cardIdx]; // 선택한 캐릭터카드

            if(selectedCard.GetPlace() == targetPlace) {
                Util._.ShowUnderBarMessage("같은 캐릭터입니다. 다른걸 선택해주세요.");
                return;
            }

            UI._.towerUpgUI.isChangeCharaMode = false;

            // 배치하려는 위치에 이미 캐릭터가 존재한다면
            var targetCard = FindCharaCard(targetPlace);
            if(targetCard != null) {
                // 다른캐릭으로 대체되기때문에 제거
                GM._.crm.RemoveChara(targetCard);
                targetCard.SetPlace(CHR_PLACE.NONE);
            }

            // 선택한 캐릭터카드가 이미 존재한다면
            if(selectedCard.GetPlace() != CHR_PLACE.NONE) {
                // 다른곳으로 배치될거기때문에 제거
                GM._.crm.RemoveChara(selectedCard);
            }

            // 캐릭터카드 배치데이터 변경
            charaCardArr[cardIdx].SetPlace(targetPlace);

            // 캐릭터 생성 배치
            GM._.crm.PlaceChara(charaCardArr[cardIdx]);

            // 타워 업그레이드 UI 업데이트
            UI._.towerUpgUI.UpdatePlaceUI();

            // 불필요한 패널 닫기
            panelObj.SetActive(false);
            Util._.toastMsgPopup.SetActive(false);

            // 메시지 표시
            Util._.ShowUnderBarMessage("캐릭터 위치변경 완료");
        }
        else
        {
            // 배치중인지 표시추가하여 캐릭터 업그레이드 창 표시
            UI._.charaUpgUI.ShowPanel(charaCardArr[cardIdx]);
        }
    }
    #endregion
    #region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
        UpdateUI();
    }

    /// <summary>
    /// 캐릭터 콜렉션 카드 UI 업데이트
    /// </summary>
    public void UpdateUI()
    {
        charaCardArr[(int)CHR_CARD_IDX.ARCHER].UpdateUI();
        charaCardArr[(int)CHR_CARD_IDX.WARRIOR].UpdateUI();
        //* 캐릭터카드 추가시 위에도 추가
    }

    /// <summary>
    /// 배치된 정보로 캐릭터카드 찾기
    /// </summary>
    /// <param name="place">찾고싶은 배치 인덱스</param>
    /// <returns>배치정보로 찾은 캐릭터카드</returns>
    public CharaCard FindCharaCard(CHR_PLACE place)
    {
        var findCard = Array.Find(charaCardArr, card => place == card.GetPlace());

        if(findCard != null)
        {
            Debug.Log($"FindCharaCard({place}):: findCard = {findCard.name}");
            return findCard;
        }
        else
        {
            Debug.Log($"FindCharaCard({place}):: 못 찾음 (Null)");
            return null;
        }
    }
#endregion
}