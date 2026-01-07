using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 홈화면에 표시되어있는 UI관리 (패널관련 UI는 각자 매니저 스크립트에서 관리)
/// </summary>
public class UI : MonoBehaviour
{
    public static UI _;

    // TOP
    public TextMeshProUGUI killCntTxt;
    public TextMeshProUGUI EnemyCntTxt;
    public TextMeshProUGUI EnemyHpTxt;
    public TextMeshProUGUI EnemyDmgTxt;

    // BOTTOM
    public TextMeshProUGUI towerArmorTxt;
    public TextMeshProUGUI towerHpTxt;
    public Slider towerHpSlider;

    void Awake()
    {
        _ = this;
    }

#region EVENT
#endregion
#region FUNC
#endregion
}
