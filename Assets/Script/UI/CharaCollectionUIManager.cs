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
        charaCardArr[(int)CHR_CARD_IDX.ARCHER].UpdateData(CHR_GRADE.NORMAL, CHR_PLACE.CENTER, 1);
    }

    void Start()
    {
        // 캐릭터 콜렉션 카드 UI 업데이트
        charaCardArr[(int)CHR_CARD_IDX.ARCHER].UpdateUI();
    }

    #region EVENT
    public void OnClickCharaCardFrameBtn(int cardIdx)
    {
        if(charaCardArr[cardIdx].IsLocked)
            return;

        // 캐릭터 배치변경 모드
        if (UI._.towerUpgUI.isChangeCharaMode)
        {
            Debug.Log($"OnClickCharaCardFrameBtn({cardIdx}):: 캐릭터 배치변경 완료");
            UI._.towerUpgUI.isChangeCharaMode = false;
            charaCardArr[cardIdx].place = UI._.towerUpgUI.changePlaceIdx;

            GM._.crm.PlaceChara(charaCardArr[cardIdx]);

            panelObj.SetActive(false);
            Util._.msgPopup.SetActive(false);
        }
        else
        {
            
        }
    }
    #endregion
    #region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }

    /// <summary>
    /// 배치된 정보로 캐릭터카드 찾기
    /// </summary>
    /// <param name="place">찾고싶은 배치 인덱스</param>
    /// <returns>배치정보로 찾은 캐릭터카드</returns>
    public CharaCard FindCharaCard(CHR_PLACE place)
    {
        var findCard = Array.Find(charaCardArr, card => place == card.place);

        if(findCard != null)
            Debug.Log($"FindCharaCard({place}):: findCard = {findCard.name}");
        else
            Debug.Log($"FindCharaCard({place}):: 못 찾음 (Null)");

        return findCard;
    }
#endregion
}