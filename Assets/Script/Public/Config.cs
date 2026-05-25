using UnityEngine;

public static class Config
{
    // 코루틴 대기시간 변수 선언
    public readonly static WaitForSeconds WFS_0_1 = new(0.1f);
    public readonly static WaitForSeconds WFS_0_2 = new(0.2f);
    public readonly static WaitForSeconds WFS_0_5 = new(0.5f);
    public readonly static WaitForSeconds WFS_1 = new(1);
    public readonly static WaitForSeconds WFS_3 = new(3);
    public readonly static WaitForSeconds WFS_5 = new(5);
    public readonly static WaitForSecondsRealtime WFS_RT_2 = new(2);

    public class Layer
    {
        public static LayerMask ENEMY = LayerMask.GetMask("Enemy");
    }

    /// <summary> 태그네임 </summary>
    public class TAG
    {
        public static readonly string ENEMY = "Enemy";
    }

#region ENUM
    /// <summary> 캐릭터카드 인덱스 (UI Content에 배치 순서) </summary>
    public enum CHR_CATE {
        ARCHER, WARRIOR, MAGICIAN, HOLYKNIGHT
    }
    /// <summary> 캐릭터 등급 </summary>
    public enum CHR_GRADE {
        NORMAL, RARE, EPIC, UNIQUE, LEGEND, MYTHIC, PRIME, COUNT
    }
    /// <summary> 캐릭터 배치 위치 </summary>
    public enum CHR_PLACE {
        NONE = -1, CENTER, LEFT, RIGHT, TOP, BOTTOM
    }
}
#endregion