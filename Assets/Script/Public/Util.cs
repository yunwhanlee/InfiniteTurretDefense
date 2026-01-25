using System.Collections;
using TMPro;
using UnityEngine;

public class Util : MonoBehaviour
{
    // 싱글톤
    static public Util _;

    // 코루틴 대기시간 변수 선언
    public WaitForSecondsRealtime WFS_RT_2SEC = new WaitForSecondsRealtime(2);

    // 메세지 팝업
    enum MSG_TYPE {ERROR, SUCCESS, INTERACTION}
    public GameObject msgPopup;
    public GameObject errMsgToast;
    public GameObject successMsgToast;
    public GameObject interactionMsgToast;
    public TextMeshProUGUI msgTxt;

    // private
    Coroutine corShowMsgID;

    void Start() => _ = this;

#region FUNC
    /// <summary>
    /// 메세지 팝업 타입배경 표시
    /// </summary>
    private void ActiveMsgPopupBg(MSG_TYPE type)
    {
        errMsgToast.SetActive(type == MSG_TYPE.ERROR);
        successMsgToast.SetActive(type == MSG_TYPE.SUCCESS);
        interactionMsgToast.SetActive(type == MSG_TYPE.INTERACTION);
    }

    private void ShowMsg(MSG_TYPE type, string msg)
    {
        ActiveMsgPopupBg(type);

        msgTxt.text = $"{msg}";

        // 코루틴 실행
        if(corShowMsgID != null) StopCoroutine(corShowMsgID);
        corShowMsgID = StartCoroutine(CorShowMsg());
    }

    private IEnumerator CorShowMsg()
    {
        msgPopup.SetActive(true);
        yield return WFS_RT_2SEC;
        msgPopup.SetActive(false);
    }

    public void ErrorMessage(string msg)
    {
        ShowMsg(MSG_TYPE.ERROR, msg);
    }

    public void SuccessMessage(string msg)
    {
        ShowMsg(MSG_TYPE.SUCCESS, msg);
    }
    public void InteractionMessage(string msg)
    {
        //TODO 
        // EX) 캐릭터를 선택해주세요
        // 선택할때까지 메세지 팝업창 표시
        // 선택하고나서 비표시 (Event Action)
    }
#endregion
}
