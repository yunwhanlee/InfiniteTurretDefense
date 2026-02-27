using UnityEngine;

public static class Config
{
    /// <summary> 태그네임 </summary>
    public class TAG
    {
        public static readonly string Enemy = "Enemy";
    }


    /// <summary> 캐릭터 등급 </summary>
    public enum CHR_GRADE {
        NORMAL, RARE, EPIC, UNIQUE, LEGEND, MYTHIC, PRIME, COUNT
    }
    /// <summary> 캐릭터 배치 위치 </summary>
    public enum CHR_PLACE {
        NONE = -1, CENTER, LEFT, RIGHT, TOP, BOTTOM
    }
    /// <summary> 캐릭터카드 인덱스 (UI Content에 배치 순서) </summary>
    public enum CHR_CATE {
        ARCHER, WARRIOR,
    }
}
