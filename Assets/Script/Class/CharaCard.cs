using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharaCard : MonoBehaviour
{
    // UI
    public GameObject lockFrame; // 잠김 표시 프레임 => 해금은 카드 획득으로만 가능! 직접 돈으로 사는거 안됨
    public GameObject placedFrame; // 배치중 표시 프레임
    public TextMeshProUGUI nameTxt; // 등급에 따른 캐릭터 이름
    public TextMeshProUGUI placedTxt; // 배치된 장소 위치 텍스트
    public Slider cardCntGaugeSlider; // 카드 카운트 게이지 슬라이더
    public Image iconImg; // 캐릭터 아이콘 이미지

    // Value
    public Sprite[] gradeCharaSprArr; // 등급에 따른 캐릭터 아이콘 스프라이트 배열
    public string[] gradeCharaNameArr; // 등급에 따른 캐릭터 고유 이름 배열
    public bool isLocked; // 잠김 여부
    public int cardCnt; // 소유중인 카드 수량

#region FUNC
    public void UpdateUI()
    {
        // TODO 잠김 여부 확인
        
    }
#endregion
}
