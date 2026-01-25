using UnityEngine;

/// <summary>
/// 현재 배치된 캐릭터 및 캐릭터 카드 관리 매니저
/// </summary>
public class CharaManager : MonoBehaviour
{
    public enum PLACE { CENTER, LEFT, RIGHT, TOP, BOTTOM }

    public Chara[] charaArr;
    public GameObject[] placeObjArr; // 캐릭터 배치 오브젝트 배열

    public Chara curSelectedChara; // 현재 선택한 캐릭터

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
        // 캐릭터 배치 초기화
        //TODO 선택한 캐릭터가 배치되도록 로직 추가 필요
        Instantiate(charaArr[0], placeObjArr[(int)PLACE.CENTER].transform);

        // CENTER로 현재 선택한 캐릭터 초기화
        curSelectedChara = charaArr[0];
    }
}
