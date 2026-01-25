using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 배치된 캐릭터 및 캐릭터 카드 관리 매니저
/// </summary>
public class CharaManager : MonoBehaviour
{
    public enum PLACE { CENTER, LEFT, RIGHT, TOP, BOTTOM }

    [Header("(테스트) 배치시킬 캐릭터 프리팹 리스트")]
    public Chara[] applyCharaPrefArr; //TODO 나중에 캐릭터가 선택된 정보를 저장하여 프리팹을 불러오는 기능 추가필요

    [Header("배치 생성된 캐릭터 리스트")]
    public List<Chara> charaInsList;

    [Header("캐릭터 배치장소 오브젝트 배열")]
    public GameObject[] placeObjArr;

    [Header("현재 선택되어있는 캐릭터")]
    public Chara curSelectedChara;

    //TODO DB화 하기
    /// <summary> 캐릭터 배치 잠김여부 배열 </summary>
    public bool[] DB_isPlaceLockedArr;

    void Awake()
    {
        //TODO (DB로드) 캐릭터 배치 잠금해제
        DB_isPlaceLockedArr = new bool[] { false, true, true, true, true };
    }

    void Start()
    {
        //TODO 선택한 캐릭터가 배치되도록 로직 추가 필요
        for(int i = 0; i < applyCharaPrefArr.Length; i++)
        {
            // 프리팹 캐릭터 생성
            Chara ins = Instantiate(applyCharaPrefArr[i], placeObjArr[(int)PLACE.CENTER].transform);
            // 현재 배치된 캐릭터 리스트에 추가
            charaInsList.Add(ins);
        }

        // CENTER로 현재 선택한 캐릭터 초기화
        curSelectedChara = charaInsList[0];
    }

#region FUNC
    /// <summary>
    /// 공격범위 원 전부 비표시 초기화
    /// </summary>
    public void InActiveCharaRangeCircle()
    {
        Debug.Log("모든 캐릭터 공격범위 원 비활성화");
        charaInsList.ForEach(chara =>
        {
            chara.rangeCircle.SetActive(false);
        });
    }

    /// <summary>
    /// 캐릭터 선택
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
