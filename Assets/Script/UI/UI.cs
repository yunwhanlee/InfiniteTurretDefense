using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 홈화면 UI
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

    //* BOTTOM
    public Slider towerHpSlider;
    public TextMeshProUGUI towerHpTxt;
    public TextMeshProUGUI towerArmorTxt;

    //* 외부 컴포넌트
    public TowerUpgradeUIManager towerUpgUI; // 타워 업그레이드 UI 매니저

    void Awake()
    {
        _ = this;
        towerUpgUI = GameObject.Find("TowerUpgradeUIManager").GetComponent<TowerUpgradeUIManager>();
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

    public void OnClickTowerBtn()
    {
        Debug.Log("OnClickTowerBtn()::");
        towerUpgUI.ShowPanel();
    }
    public void OnClickCharaBtn()
    {
        //TODO 캐릭터 패널 띄우기
    }
#endregion
#region FUNC
    /// <summary>
    /// 타워 체력 슬라이더 UI 설정
    /// </summary>
    public void SetTowerHpSlider(int hp, int maxHp)
    {
        towerHpTxt.text = $"{hp} / {maxHp}";
        towerHpSlider.value = (float)hp / maxHp;
    }

    public void SetTowerArmorTxt(int armor)
    {
        towerArmorTxt.text = armor.ToString();
    }
#endregion
}
