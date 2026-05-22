using UnityEngine;

public class HolyKnight : MonoBehaviour
{
    // 빛의 가호
    const int HOLY_GUARD_COOLTIME = 26;
    [SerializeField] float holyGuardTime = 0;

    // 빛 폭발
    const int HOLY_BURST_COOLTIME = 26;
    [SerializeField] float holyBurstTime = 0;

    // 빛의 치유
    const int HOLY_HEAL_COOLTIME = 23;
    [SerializeField] float holyHealTime = 0;

    // 빛의 장막
    const int HOLY_AURA_COOLTIME = 47;
    [SerializeField] float holyAuraTime = 0;

    // 빛의 기둥
    const int HOLY_BEAM_COOLTIME = 39;
    [SerializeField] float holyBeamTime = 0;

    // 신의 심판
    const int HOLY_SMITE_COOLTIME = 69;
    [SerializeField] float holySmiteTime = 0;

    
    // void Update()
    // {
    //     base.Update();
    // }
}