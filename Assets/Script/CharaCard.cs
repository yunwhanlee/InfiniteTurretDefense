using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Config;

public class CharaCard : MonoBehaviour
{
    // *UI
    public GameObject lockFrame; // 잠김 표시 프레임 => 해금은 카드 획득으로만 가능! 직접 돈으로 사는거 안됨
    public GameObject placedFrame; // 배치중 표시 프레임
    public TextMeshProUGUI nameTxt; // 등급에 따른 캐릭터 이름 텍스트
    public TextMeshProUGUI placedTxt; // 배치된 장소 위치 텍스트
    public TextMeshProUGUI cardCntTxt; // 카드 보유 수량 텍스트
    public Slider cardCntGaugeSlider; // 카드 카운트 게이지 슬라이더
    public Image iconImg; // 캐릭터 아이콘 이미지


    //* Value
    // public int cardCnt; // 소유중인 카드 수량
    // public CHR_GRADE grade; // 등급
    // public CHR_PLACE place; // 배치 위치
    // public Sprite[] gradeCharaSprArr; // 등급에 따른 캐릭터 아이콘 스프라이트 배열
    public string[] gradeCharaNameArr; // 등급에 따른 캐릭터 고유 이름 배열
    // public bool IsLocked => cardCnt <= 0; // 잠김 여부
    //* Object
    // public GameObject[] charaPrefArr; // 캐릭터 등급별 프리팹

    // Data
    private UserCharaData userData; // 유저 캐릭터 데이터
    private CharaDataAsset charaDataAsset; // 캐릭터 데이터 에셋

    #region FUNC
    /// <summary>
    /// 다음 등급업에 필요한 카드 수량 반환
    /// </summary>
    private int GetNextGradeCardCnt(CHR_GRADE grade)
    {
        const int OFFSET = 1;
         return ((int)grade + OFFSET) * 10;
    }

    /// <summary>
    /// 데이터 세팅
    /// </summary>
    /// <param name="cardIdx">캐릭터 카드 인덱스</param>
    public void SetUp(CHR_CARD_IDX cardIdx)
    {
        userData = DB._.GetUserCharaDataAsset(cardIdx);

        switch (cardIdx)
        {
            case CHR_CARD_IDX.ARCHER:
                charaDataAsset = GM._.crm.archerDataAssetArr[(int)userData.grade];
                break;
            case CHR_CARD_IDX.WARRIOR:
                charaDataAsset = GM._.crm.warriorDataAssetArr[(int)userData.grade];
                break;
            // 여기에 추가
        }
    }

    public void UpdateUI()
    {
        // 프레임 (비)표시
        lockFrame.SetActive(userData.IsLocked);
        placedFrame.SetActive(userData.place != CHR_PLACE.NONE);

        nameTxt.text = charaDataAsset.charaName;
        placedTxt.text = userData.place.ToString();
        cardCntTxt.text = $"{userData.cardCnt} / {GetNextGradeCardCnt(userData.grade)}";
        cardCntGaugeSlider.value = (float)0 / 0;
        iconImg.sprite = charaDataAsset.icon;
    }

    public CHR_PLACE GetPlace() => userData.place;
    public CHR_PLACE SetPlace(CHR_PLACE place) => userData.place = place;
    public GameObject GetCharaPref() => charaDataAsset.charaPrefab;
    public Sprite GetIconSprite() => charaDataAsset.icon;
    public bool IsLocked() => userData.IsLocked;
#endregion
}
