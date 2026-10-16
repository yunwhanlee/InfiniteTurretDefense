using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Config;

/// <summary>
///* 캐릭터 카드 UI
/// </summary>
public class CharaCard : MonoBehaviour
{
    // UI
    public GameObject lockFrame; // 잠김 표시 프레임 => 해금은 카드 획득으로만 가능! 직접 돈으로 사는거 안됨
    public GameObject placedFrame; // 배치중 표시 프레임
    public TextMeshProUGUI nameTxt; // 등급에 따른 캐릭터 이름 텍스트
    public TextMeshProUGUI placedTxt; // 배치된 장소 위치 텍스트
    public TextMeshProUGUI cardCntTxt; // 카드 보유 수량 텍스트
    public Slider cardCntGaugeSlider; // 카드 카운트 게이지 슬라이더
    public Image iconImg; // 캐릭터 아이콘 이미지

    // Data
    [SerializeField] private UserCharaData userData; // 유저 캐릭터 데이터
    [SerializeField] private CharaDataAsset charaDataAsset; // 캐릭터 데이터 에셋

    #region FUNC
    /// <summary>
    /// 캐릭터에셋 변경
    /// </summary>
    /// <param name="cate">캐릭터 카테고리</param>
    private void SetCharaDataAsset(CHR_CATE cate)
    {
        Debug.Log($"SetCharaDataAsset():: cate= {cate}");
        switch (cate)
        {
            case CHR_CATE.ARCHER:
                charaDataAsset = GM._.crm.archerDataAssetArr[(int)userData.grade];
                break;
            case CHR_CATE.WARRIOR:
                charaDataAsset = GM._.crm.warriorDataAssetArr[(int)userData.grade];
                break;
            case CHR_CATE.MAGICIAN:
                charaDataAsset = GM._.crm.magicianDataAssetArr[(int)userData.grade];
                break;
            case CHR_CATE.HOLYKNIGHT:
                charaDataAsset = GM._.crm.holyKnightDataAssetArr[(int)userData.grade];
                break;
            case CHR_CATE.NINZA:
                charaDataAsset = GM._.crm.ninzaDataAssetArr[(int)userData.grade];
                break;
            case CHR_CATE.ENGINEER:
                charaDataAsset = GM._.crm.engineerDataAssetArr[(int)userData.grade];
                break;
            // 여기에 추가
        }
    }
    
    /// <summary>
    /// 다음 등급업에 필요한 카드 수량 반환
    /// </summary>
    public int GetNextGradeCardCnt()
    {
        const int DEF = 10;
        return DEF << (int)userData.grade; // grade가 0이면 10, 1이면 20, 2면 40, 3이면 80...
    }

    /// <summary>
    /// 데이터 세팅 (초기화)
    /// </summary>
    /// <param name="cate">캐릭터 카테고리</param>
    public void SetUp(CHR_CATE cate)
    {
        userData = DB._.GetUserCharaDataAsset(cate);

        // 카테고리별 현재 캐릭터 등급별 에셋 설정
        SetCharaDataAsset(cate);
    }

    public void UpdateUI()
    {
        // 프레임 (비)표시
        lockFrame.SetActive(userData.IsLocked);
        placedFrame.SetActive(userData.place != CHR_PLACE.NONE);

        nameTxt.text = charaDataAsset.charaName;
        placedTxt.text = userData.place.ToString();
        cardCntTxt.text = $"{userData.cardCnt} / {GetNextGradeCardCnt()}";
        cardCntGaugeSlider.value = (float)0 / 0;
        iconImg.sprite = charaDataAsset.icon;
    }

    /// <summary>
    /// 등급 업
    /// </summary>
    public bool GradeUp(CHR_CATE cate)
    {
        if(GetCardCnt() >= GetNextGradeCardCnt())
        {
            userData.grade++;

            // 카테고리별 현재 캐릭터 등급별 에셋 설정
            SetCharaDataAsset(cate);

            // 카드수량 감소
            userData.cardCnt -= GetNextGradeCardCnt();
            // 캐릭터 오브젝트 반영
            GM._.crm.RemoveChara(this);
            GM._.crm.PlaceChara(this);
            GM._.crm.SelectChara(GM._.crm.curSelectedChara);

            return true;
        }
        else
            return false;
    }

    public CharaDataAsset GetCharaDataAsset() => charaDataAsset;
    public UserCharaData GetUserData() => userData;
    public CHR_CATE GetCate() => userData.cate;
    public CHR_GRADE GetGrade() => userData.grade;
    public CHR_PLACE GetPlace() => userData.place;
    public CHR_PLACE SetPlace(CHR_PLACE place) => userData.place = place;
    public GameObject GetCharaPref() => charaDataAsset.charaPrefab;
    public Sprite GetIconSprite() => charaDataAsset.icon;
    public int GetCardCnt() => userData.cardCnt;
    public int GetSkillLv(int idx) => userData.skillLvArr[idx];
    public bool IsLocked() => userData.IsLocked;
#endregion
}