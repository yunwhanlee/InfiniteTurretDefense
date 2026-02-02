using UnityEngine;
using static Config;

/// <summary>유저 캐릭터 저장 데이터</summary>
[System.Serializable]
public class UserTowerUpgData
{
    public bool[] isPlaceLockedArr; // 캐릭터 배치 잠김여부 배열
    public int upgradeHpLv = 1; 
    public int upgradeArmorLv = 1;
    public int upgradeHealLv = 1;

    public UserTowerUpgData(bool[] isPlaceLockedArr, int upgradeHpLv, int upgradeArmorLv, int upgradeHealLv)
    {
        this.isPlaceLockedArr = isPlaceLockedArr;
        this.upgradeHpLv = upgradeHpLv;
        this.upgradeArmorLv = upgradeArmorLv;
        this.upgradeHealLv = upgradeHealLv;
    }
}

/// <summary>유저 캐릭터 저장 데이터</summary>
[System.Serializable]
public class UserCharaData
{
    public CHR_CARD_IDX cardIdx; // 캐릭터 종류
    public CHR_GRADE grade;      // 등급
    public int cardCnt;          // 카드보유 수량
    public CHR_PLACE place;      // 배치위치

    public UserCharaData(CHR_CARD_IDX cardIdx, CHR_GRADE grade, int cardCnt, CHR_PLACE place)
    {
        this.cardIdx = cardIdx;
        this.grade = grade;
        this.cardCnt = cardCnt;
        this.place = place;
    }

    public bool IsLocked => cardCnt <= 0; // 잠김여부

    public void Reset()
    {
        
    }
}

/// <summary>
///* 저장 데이터 베이스
/// </summary>
public class DB : MonoBehaviour
{
    public static DB _; // 싱글톤

    public UserTowerUpgData userTowerUpgData; // 유저 타워 데이터
    public UserCharaData[] userCharaDataArr; // 유저 캐릭터 데이터 배열

    void Awake()
    {
        _ = this;

        //TODO 나중에 PlayerPref으로 클래스 저장하기
        //TODO 나중에 Load함수에서 호출하기
        //TODO 나중에 클래스에서 Init과 Reset함수를 따로 만들기

        // 타워 업그레이드 데이터 로드
        userTowerUpgData = new UserTowerUpgData(
            new bool[] {false, true, true, true, true},
            upgradeHpLv : 1,
            upgradeArmorLv : 1,
            upgradeHealLv : 1
        );

        // 캐릭터 데이터 로드
        userCharaDataArr = new UserCharaData[] {
            new (CHR_CARD_IDX.ARCHER, CHR_GRADE.NORMAL, 1, CHR_PLACE.CENTER),
            new (CHR_CARD_IDX.WARRIOR, CHR_GRADE.NORMAL, 1, CHR_PLACE.NONE)
        };
    }

#region FUNC
    /// <summary> 유저 캐릭터 데이터 로드 </summary>
    public UserCharaData GetUserCharaDataAsset(CHR_CARD_IDX cardIdx)
    {
        return userCharaDataArr[(int)cardIdx];
    }

#endregion
}
