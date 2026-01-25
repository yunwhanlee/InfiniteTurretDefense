using UnityEngine;

/// <summary>
/// 현재 배치된 캐릭터 및 캐릭터 카드 관리 매니저
/// </summary>
public class CharaManager : MonoBehaviour
{
    public enum PLACE { CENTER, LEFT, RIGHT, TOP, BOTTOM }

    public Chara charaArr;

    public GameObject[] placeObjArr; // 캐릭터 배치 오브젝트 배열

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
        
    }

    void Update()
    {
        
    }
}
