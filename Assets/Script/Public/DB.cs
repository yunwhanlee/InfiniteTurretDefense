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
    public CHR_CATE cate; // 캐릭터 종류
    public CHR_GRADE grade;      // 등급
    public int cardCnt;          // 카드보유 수량
    public CHR_PLACE place;      // 배치위치
    public int[] skillLvArr;     // 스킬 레벨 배열

    public UserCharaData(CHR_CATE cate, CHR_GRADE grade, int cardCnt, CHR_PLACE place, int[] skillLvArr)
    {
        this.cate = cate;
        this.grade = grade;
        this.cardCnt = cardCnt;
        this.place = place;
        this.skillLvArr = skillLvArr;
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

        const int SKILL_CNT = (int)CHR_GRADE.COUNT; // 스킬 개수 (등급 수와 동일)

        // 캐릭터 데이터 로드
        userCharaDataArr = new UserCharaData[] {
            new (CHR_CATE.ARCHER, CHR_GRADE.NORMAL, 999, CHR_PLACE.CENTER, new int[SKILL_CNT] {1,0,0,0,0,0,0}),
            new (CHR_CATE.WARRIOR, CHR_GRADE.NORMAL, 999, CHR_PLACE.NONE, new int[SKILL_CNT] {1,0,0,0,0,0,0}),
            new (CHR_CATE.MAGICIAN, CHR_GRADE.NORMAL, 999, CHR_PLACE.NONE, new int[SKILL_CNT] {1,0,0,0,0,0,0}),
            new (CHR_CATE.HOLYKNIGHT, CHR_GRADE.NORMAL, 999, CHR_PLACE.NONE, new int[SKILL_CNT] {1,0,0,0,0,0,0}),
            new (CHR_CATE.NINZA, CHR_GRADE.NORMAL, 999, CHR_PLACE.NONE, new int[SKILL_CNT] {1,0,0,0,0,0,0}),
            // 여기에 추가
        };
    }

#region FUNC
    /// <summary> 유저 캐릭터 데이터 로드 </summary>
    public UserCharaData GetUserCharaDataAsset(CHR_CATE cate)
    {
        return userCharaDataArr[(int)cate];
    }

#endregion
}
