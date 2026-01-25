using UnityEngine;

public class CharaCollectionUIManager : MonoBehaviour
{
    public GameObject panelObj;


    void Start()
    {
        panelObj.SetActive(false);
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
