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
    /// <summary>
    /// 현재 배치된 캐릭터 선택 좌우 이동
    /// </summary>
    /// <param name="isRight">True : 오른쪽 방향, False : 왼쪽 방향</param>
    public void OnClickArrowBtn(bool isRight)
    {
        
    }
#endregion
#region FUNC
    public void ShowPanel()
    {
        panelObj.SetActive(true);
    }
#endregion
}
