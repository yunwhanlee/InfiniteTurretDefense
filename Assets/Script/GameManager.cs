using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static GameManager _;

    // 컴포넌트
    public Player Player;

    void Awake()
    {
        _ = this;
        Player = GameObject.Find("Tower").GetComponent<Player>();
    }

    //! 어차피 하나의 씬에서 플레이 될거라서 필요X
    void Singleton()
    {
        // 1. 인스턴스가 비어있다면(처음 생성된 거라면)
        if (_ == null)
        {
            _ = this;
            DontDestroyOnLoad(gameObject); // 핵심: 씬이 넘어가도 파괴되지 않게 설정
        }
        else // 2. 이미 누군가(진짜)가 존재한다면
        {
            Destroy(gameObject); // 나는 가짜니까 내 게임 오브젝트를 통째로 파괴
        }
    }
}
