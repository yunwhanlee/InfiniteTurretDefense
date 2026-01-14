using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 홈화면에 표시되어있는 UI관리 (패널관련 UI는 각자 매니저 스크립트에서 관리)
/// </summary>
public class UI : MonoBehaviour
{
    public static UI _;

    [Header("메뉴 패널")]
    public GameObject menuPanel;

    [Space(10)]
    public TextMeshProUGUI killCntTxt;  //TODO 지금은 안씀
    public TextMeshProUGUI EnemyCntTxt; // 몬스터 수
    public TextMeshProUGUI EnemyHpTxt;  // 몬스터 체력
    public TextMeshProUGUI EnemyDmgTxt; // 몬스터 공격력

    // BOTTOM
    public TextMeshProUGUI towerArmorTxt;
    public TextMeshProUGUI towerHpTxt;
    public Slider towerHpSlider;

    public TowerUIManager towerUI;

    void Awake()
    {
        _ = this;
        towerUI = GameObject.Find("TowerUIManager").GetComponent<TowerUIManager>();
    }

    void Start()
    {
        menuPanel.SetActive(false);
    }

#region EVENT
    public void OnClickMenuBtn()
    {
        menuPanel.SetActive(true);
    }

    public void OnClickMenuTowerBtn()
    {
        towerUI.ShowPanel();
    }
    public void OnClickMenuCharaBtn()
    {
        //TODO 캐릭터 패널 띄우기
    }
#endregion
#region FUNC
#endregion
}
