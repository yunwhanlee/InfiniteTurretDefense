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
        const int ARCHER = 0;
        charaCardArr[ARCHER].grade = CHR_GRADE.NORMAL;
        charaCardArr[ARCHER].place = CHR_PLACE.CENTER;
    }

    void Start()
    {
        // 캐릭터 콜렉션 카드 UI 업데이트
        charaCardArr[0].UpdateUI();
    }

    #region EVENT
    #endregion
    #region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }
#endregion
}
