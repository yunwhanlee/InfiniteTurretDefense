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

    //* TOP
    public TextMeshProUGUI coinTxt;
    public TextMeshProUGUI diamondTxt;

    [Space(10)]
    public TextMeshProUGUI killCntTxt;  //TODO 지금은 안씀
    public TextMeshProUGUI EnemyCntTxt; // 몬스터 수
    public TextMeshProUGUI EnemyHpTxt;  // 몬스터 체력
    public TextMeshProUGUI EnemyDmgTxt; // 몬스터 공격력

    //* BOTTOM
    public Slider towerHpSlider;
    public Slider towerSheildSlider;
    public TextMeshProUGUI towerHpTxt;
    public TextMeshProUGUI towerArmorTxt;
    public TextMeshProUGUI towerSheildTxt;

    //* 외부 컴포넌트
    [HideInInspector] public TowerUpgradeUIManager towerUpgUI; // 타워 업그레이드 UI 매니저
    [HideInInspector] public CharaUpgradeUIManager charaUpgUI; // 캐릭터 업그레이드 UI 매니저
    [HideInInspector] public CharaCollectionUIManager charaCltUI; // 캐릭터 카드콜레션 UI 매니저

    void Awake()
    {
        _ = this;
        towerUpgUI = GameObject.Find("TowerUpgradeUIManager").GetComponent<TowerUpgradeUIManager>();
        charaUpgUI = GameObject.Find("CharaUpgradeUIManager").GetComponent<CharaUpgradeUIManager>();
        charaCltUI = GameObject.Find("CharaCollectionUIManager").GetComponent<CharaCollectionUIManager>();
    }

    void Start()
    {
        menuPanel.SetActive(false);
        towerSheildSlider.gameObject.SetActive(false);
    }

#region EVENT
    public void OnClickMenuBtn()
    {
        menuPanel.SetActive(true);
    }

    public void OnClickTowerBtn()
    {
        towerUpgUI.ShowPanel();
    }
    public void OnClickCharaBtn()
    {
        GM._.crm.SelectChara(GM._.crm.curSelectedChara); // 캐릭터 선택
        charaUpgUI.ShowPanel();
    }
    public void OnClickCollectionBtn()
    {
        charaCltUI.ShowPanel();
    }
#endregion
#region FUNC
    /// <summary>
    /// 타워 체력 슬라이더 UI 업데이트
    /// </summary>
    public void SetTowerHpSlider(int hp, int maxHp)
    {
        towerHpTxt.text = $"{hp} / {maxHp}";
        towerHpSlider.value = (float)hp / maxHp;
    }

    /// <summary>
    /// 타워 쉴드 슬라이더 UI 업데이트
    /// </summary>
    public void SetSheildHpSlider(int shield, int maxShield)
    {
        if(shield > 0)
        {
            towerSheildSlider.gameObject.SetActive(true);
        }
        else
        {
            towerSheildSlider.gameObject.SetActive(false);
            return;
        }

        towerSheildSlider.maxValue = maxShield;
        towerSheildTxt.text = $"{shield} / {maxShield}";
        towerSheildSlider.value = (float)shield / maxShield;
    }

    /// <summary>
    /// 방어력 업데이트
    /// </summary>
    public void SetTowerArmorTxt(int armor)
    {
        towerArmorTxt.text = armor.ToString();
    }
#endregion
}
