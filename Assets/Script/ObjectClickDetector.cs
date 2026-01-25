using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// 화면클릭 시 오브젝트 레이케스트 탐지
/// </summary>
public class ObjectClickDetector : MonoBehaviour
{
    public bool isClicked; // 현재 클릭되어있는지 여부
    public LayerMask targetLayer; // 일부 레이어만 체크할 수 있도록

    void Update()
    {
        // 마우스가 UI 위에 있다면 로직 무시
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // 클릭/터치 감지
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            // 화면(스크린) 좌표 가져오기
            Vector2 pointerPos = Pointer.current.position.ReadValue();

            // 화면 좌표를 게임 세상(월드) 좌표로 변환
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(pointerPos);

            // 해당 지점에 있는 2D 콜라이더 검출 (Raycast 사용)
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, targetLayer);

            // 맞은 게 있는지 확인
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Character"))
                {
                    Debug.Log("캐릭터 선택됨: " + hit.collider.name);

                    // 현재 선택된 캐릭터로 업데이트
                    Chara target = hit.collider.GetComponent<Chara>();
                    // 캐릭터 선택
                    GM._.crm.SelectChara(target);
                    // 캐릭터 업그레이드 UI 패널 표시
                    UI._.charaUpgUI.ShowPanel();
                }
            }
        }
    }
}
