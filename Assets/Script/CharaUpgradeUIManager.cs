using UnityEngine;

/// <summary>
/// 캐릭터 업그레이드 UI 매니저
/// </summary>
public class CharaUpgradeUIManager : MonoBehaviour
{
    public GameObject panelObj;

    void Start()
    {
        
    }

    void Update()
    {
        
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
