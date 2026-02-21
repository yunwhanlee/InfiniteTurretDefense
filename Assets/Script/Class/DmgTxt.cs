using UnityEngine;
using TMPro;
using static Config;

public class DmgTxt : MonoBehaviour
{
    const int DELAY_SEC = 1;

    public TextMeshPro txt;
    float time = 0;

    public Animator anim;

    void Update()
    {
        time += Time.deltaTime;

        if(time >= DELAY_SEC)
        {
            GM._.dmgTxtMng.ReleasePool(this);
        }
    }

    public void PlayAnim(int dmg, bool isCritical)
    {
        time = 0;
        txt.text = $"{dmg}";
        txt.color = isCritical? Color.yellow : Color.white;
        anim.Play("DmgTxtAnim", -1, 0f);
    }
}
