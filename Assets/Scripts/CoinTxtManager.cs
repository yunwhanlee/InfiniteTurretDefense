using TMPro;
using UnityEngine;

/// <summary>
/// 코인 획득 표시 텍스트 (기존 DmgTxtAnim 활용)
/// </summary>
public class CoinTxtManager : MonoBehaviour
{
    const float SHOW_DURATION = 1.5f;

    public TextMeshPro coinTxt;
    public Animator anim;

    int totalCoin = 0;
    float hideTimer = 0f;
    bool isShowing = false;

    void Start()
    {
        anim = coinTxt.GetComponent<Animator>();
        coinTxt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isShowing)
            return;

        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0f)
        {
            isShowing = false;
            totalCoin = 0;
            coinTxt.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 획득한 코인량 텍스트 표시 애니메이션
    /// </summary>
    public void ActiveAnim(int coinVal)
    {
        if (coinVal <= 0)
            return;

        totalCoin += coinVal;
        coinTxt.text = $"<sprite name=coin>{totalCoin}";

        // 처음 켜질 때만 애니메이션 1회 재생 (연속 도착 시 뚝뚝 끊김 방지)
        if (!isShowing)
        {
            isShowing = true;
            hideTimer = SHOW_DURATION;
            coinTxt.gameObject.SetActive(true);
            anim.Play("TxtUpAnim", -1, 0f);
        }
    }
}