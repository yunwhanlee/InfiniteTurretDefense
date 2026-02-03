using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Util : MonoBehaviour
{
    // 싱글톤
    static public Util _;

    // 코루틴 대기시간 변수 선언
    public WaitForSecondsRealtime WFS_RT_2SEC = new WaitForSecondsRealtime(2);

    // 토스트 메세지 팝업
    enum MSG_TYPE {ERROR, SUCCESS, INTERACTION}
    [SerializeField] public GameObject toastMsgPopup;
    [SerializeField] public GameObject errMsgToast;
    [SerializeField] public GameObject successMsgToast;
    [SerializeField] public TextMeshProUGUI toastMsgTxt;

    [SerializeField] public GameObject interactionMsgToast;
    [SerializeField] public TextMeshProUGUI interactionMsgTxt;

    // 언더바 메세지 팝업
    [SerializeField] public GameObject underBarMsgPopup;
    [SerializeField] public TextMeshProUGUI underBarMsgTxt;

    // 확인 팝업
    public event Action OnClickConfirmEvent;
    [SerializeField] public GameObject confirmPopup;
    [SerializeField] public TextMeshProUGUI confirmTitleTxt;
    [SerializeField] public TextMeshProUGUI confirmMessageTxt;
    [SerializeField] public TextMeshProUGUI confirmBtnTxt;

    // private
    event Action OnBackInteractionMsgEvent; // Interaction 뒤로가기 버튼 콜백 이벤트
    Coroutine corShowToastMsgID;
    Coroutine corShowUnderbarMsgID;

    void Start() => _ = this;

#region EVENT
    /// <summary>
    /// 확인 팝업 버튼 클릭시, 콜백 이벤트 실행
    /// </summary>
    public void OnClickConfirmPopUpBtn()
    {
        confirmPopup.SetActive(false);
        OnClickConfirmEvent.Invoke();
    }
#endregion

#region FUNC
    /// <summary> (코루틴) 해당 메세지 2초간 표시 </summary>
    private IEnumerator CorShowMsg(GameObject msgPopup)
    {
        msgPopup.SetActive(true);
        yield return WFS_RT_2SEC;
        msgPopup.SetActive(false);
    }

    /// <summary> 토스트 메시지 처리 </summary>
    private void ShowToastMsg(MSG_TYPE type, string msg)
    {
        // 메세지 팝업 타입배경 표시
        errMsgToast.SetActive(type == MSG_TYPE.ERROR);
        successMsgToast.SetActive(type == MSG_TYPE.SUCCESS);

        toastMsgTxt.text = $"{msg}";

        if(type == MSG_TYPE.SUCCESS || type == MSG_TYPE.ERROR)
        {
            // 코루틴 실행
            if(corShowToastMsgID != null) StopCoroutine(corShowToastMsgID);
            corShowToastMsgID = StartCoroutine(CorShowMsg(toastMsgPopup));
        }
        else
        {
            toastMsgPopup.SetActive(true);
        }
    }

    /// <summary> UnderBar 메시지 처리 </summary>
    private void ShowUnderBarMsg(string msg)
    {
        underBarMsgTxt.text = msg;

        // 코루틴 실행
        if(corShowUnderbarMsgID != null) StopCoroutine(corShowUnderbarMsgID);
        corShowUnderbarMsgID = StartCoroutine(CorShowMsg(underBarMsgPopup));
    }

    /// <summary> 실패 토스트 메시지 표시 </summary>
    public void ShowErrorMessage(string msg)
    {
        ShowToastMsg(MSG_TYPE.ERROR, msg);
    }

    /// <summary> 성공 토스트 메시지 표시 </summary>
    public void ShowSuccessMessage(string msg)
    {
        ShowToastMsg(MSG_TYPE.SUCCESS, msg);
    }

    /// <summary> 특정 상호작용까지 토스트 메시지를 표시 </summary>
    /// <param name="msg">메시지</param>
    /// <param name="callback">뒤로가기 버튼 누를시 실행하는 콜백함수</param>
    public void ShowInteractionMessage(string msg, Action callback)
    {
        // 선택할때까지 메세지 팝업창 표시
        interactionMsgToast.SetActive(true);
        interactionMsgTxt.text = msg;

        // 만약 닫기버튼 누를시 실행할 콜백함수
        OnBackInteractionMsgEvent = CloseInteractionMsgPopup;
        OnBackInteractionMsgEvent += callback;
    }

    /// <summary> 상호작용 메시지 숨기기 </summary>
    public void CloseInteractionMsgPopup()
    {
        interactionMsgToast.SetActive(false);
    }

    /// <summary> 상호작용 뒤로가기 버튼 클릭 </summary>
    public void OnClickInteractionBackBtn()
    {
        if(OnBackInteractionMsgEvent == null) return;
        OnBackInteractionMsgEvent.Invoke(); // 콜백함수 실행
    }

    /// <summary> UnderBar 메시지 2초간 표시 </summary>
    public void ShowUnderBarMessage(string msg)
    {
        ShowUnderBarMsg(msg);
    }

    /// <summary> 확인 팝업 표시 </summary>
    public void ShowConfirmPopup(string title, string msg, string okTxt, Action callback)
    {
        confirmPopup.SetActive(true);
        // UI
        confirmTitleTxt.text = title;
        confirmMessageTxt.text = msg;
        confirmBtnTxt.text = okTxt;
        // 이벤트 구독
        OnClickConfirmEvent = callback;
    }
#endregion
}
