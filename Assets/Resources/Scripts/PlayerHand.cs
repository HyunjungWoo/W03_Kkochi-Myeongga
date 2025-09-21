using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Camera mainCamera;
    private Skewer grabbableSkewer; // 잡을 수 있는 범위에 들어온 꼬치
    private Skewer heldSkewer;      // 현재 잡고 있는 꼬치

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. 손을 마우스 커서 위치로 계속 이동
        transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 잡을 수 있는 꼬치가 있고, 현재 아무것도 안 잡고 있다면
            if (grabbableSkewer != null && heldSkewer == null)
            {
                heldSkewer = grabbableSkewer;
                heldSkewer.Grab(this.transform); // 꼬치에게 "잡혔다!"고 알려줌
            }
        }

        // 3. 마우스 왼쪽 버튼을 뗐을 때
        if (Input.GetMouseButtonUp(0))
        {
            // 잡고 있는 꼬치가 있다면
            if (heldSkewer != null)
            {
                heldSkewer.Release(); // 꼬치에게 "놓아졌다!"고 알려줌
                heldSkewer = null;
            }
        }
    }

    // 손의 트리거에 다른 Collider가 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 어떤 오브젝트든 닿기만 하면 로그를 출력해서 확인
        //Debug.Log("트리거 감지! 닿은 오브젝트: " + other.name + " | 태그: " + other.tag);

        // 들어온 것이 "Stick" 태그를 가졌다면
        if (other.CompareTag("Stick"))
        {
            // 잡을 수 있는 대상으로 지정
            grabbableSkewer = other.GetComponent<Skewer>();
        }
    }

    // 손의 트리거에서 다른 Collider가 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 것이 "Stick" 태그를 가졌다면
        if (other.CompareTag("Stick"))
        {
            // 잡을 수 있는 대상에서 해제
            grabbableSkewer = null;
        }
    }

    public void ForceRelease()
    {
        // 잡고 있던 꼬치에 대한 참조를 강제로 비워줌
        heldSkewer = null;
    }
}