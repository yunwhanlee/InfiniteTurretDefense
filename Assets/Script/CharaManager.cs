using System;
using System.Collections.Generic;
using UnityEngine;
using static Config;

/// <summary>
/// 현재 배치된 캐릭터 및 캐릭터 카드 관리 매니저
/// </summary>
public class CharaManager : MonoBehaviour
{
    [Header("실제 및 배치 생성된 캐릭터 리스트")]
    public List<Chara> curCharaList;

    [Header("캐릭터 배치장소 오브젝트 배열")]
    public GameObject[] placeAreaArr;

    [Header("현재 선택되어있는 캐릭터")]
    public Chara curSelectedChara;

    void Start()
    {
        //모든 캐릭터 카드배열
        CharaCard[] charaCardArr = UI._.charaCltUI.charaCardArr;

        for(int i = 0; i < charaCardArr.Length; i++) {
            // 캐릭터 프리팹 생성 및 배치
            if(charaCardArr[i].place != CHR_PLACE.NONE)
            {
                Chara chara = PlaceChara(charaCardArr[i]);
                // 현재 캐릭터 리스트에 추가
                curCharaList.Add(chara);
            }
        }

        // CENTER로 현재 선택한 캐릭터 초기화
        curSelectedChara = curCharaList[0];
    }

#region FUNC
    /// <summary>
    ///* 캐릭터 자리 배치
    /// </summary>
    /// <param name="chara">배치할 캐릭터 카드에서 정보를 뽑음</param>
    public Chara PlaceChara(CharaCard card)
    {
        if(card.place == CHR_PLACE.NONE)
            return null;

        // 캐릭터 생성 및 배치
        GameObject obj = Instantiate(card.GetCurGradeCharaPref(), placeAreaArr[(int)card.grade].transform);
        Chara chara = obj.GetComponent<Chara>();

        return chara; // 리턴한 캐릭터를 전역변수인 캐릭터 리스트에 추가
    }

    /// <summary>
    ///* 공격범위 원 전부 비표시 초기화
    /// </summary>
    public void InActiveCharaRangeCircle()
    {
        Debug.Log("모든 캐릭터 공격범위 원 비활성화");
        curCharaList.ForEach(chara => chara.rangeCircle.SetActive(false));
    }

    /// <summary>
    ///* 캐릭터 선택
    /// </summary>
    public void SelectChara(Chara target)
    {
        // 현택 선택된 캐릭터로 변경 업데이트
        curSelectedChara = target;
        // 공격범위 원 전부 비표시 초기화
        InActiveCharaRangeCircle();
        // 선택한 캐릭터 공격범위 원 표시
        target.rangeCircle.SetActive(true);
    }
#endregion
}
