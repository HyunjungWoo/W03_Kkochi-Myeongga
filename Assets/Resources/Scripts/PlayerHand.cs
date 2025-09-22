using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Camera mainCamera;
    private Skewer grabbableSkewer; // 잡을 수 있는 범위에 들어온 꼬치
    private Skewer heldSkewer;      // 현재 잡고 있는 꼬치
    private Customer currentCustomer; // 손이 현재 접촉하고 있는 고

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. 손을 마우스 커서 위치로 계속 이동
        transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. 마우스 왼쪽 버튼을 '누르는 순간'
        if (Input.GetMouseButtonDown(0))
        {
            // 잡을 수 있는 꼬치가 있고, 현재 아무것도 안 잡고 있다면
            if (grabbableSkewer != null && heldSkewer == null)
            {
                heldSkewer = grabbableSkewer;
                heldSkewer.Grab(this.transform); // 꼬치에게 잡혔다고 알려줌
            }
        }

        // 3. 마우스 왼쪽 버튼을 '떼는 순간'
        if (Input.GetMouseButtonUp(0))
        {
            // 만약 잡고 있는 꼬치가 있다면
            if (heldSkewer != null)
            {
                if(currentCustomer != null)
                {
                    if (GameManager.Instance.isLeaving) return; // 손님이 나가는 중이면 아무것도 안 함
                    float money = currentCustomer.CalculatePayment(heldSkewer);
                    Debug.Log($"손님에게서 {money}원을 받았습니다. 현재 잔액: {GameManager.Instance.money}원");
                    GameManager.Instance.AddMoney((int)money);
                    GameManager.Instance.CompleteOrder();

                }

                heldSkewer.Release(); // 꼬치에게 놓였다고 알려줌
                heldSkewer = null;    // 손은 이제 아무것도 안 잡고 있음

            }
        }
    }

    // 손의 트리거에 다른 Collider가 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 것이 "Stick" 태그를 가졌다면
        if (other.CompareTag("Stick"))
        {
            // 잡을 수 있는 대상으로 지정
            grabbableSkewer = other.GetComponent<Skewer>();
        }
        else if(other.CompareTag("Customer"))
        {
            currentCustomer = other.GetComponent<Customer>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 것이 "Stick" 태그를 가졌다면
        if (other.CompareTag("Stick") && grabbableSkewer == other.GetComponent<Skewer>()) 
        { 
            grabbableSkewer = null; 
        }
        else if(other.CompareTag("Customer") && currentCustomer == other.GetComponent<Customer>())
        {
            currentCustomer = null;
        }
    }

    public void ForceRelease()
    {
        // 잡고 있던 꼬치에 대한 참조를 강제로 비워줌
        heldSkewer = null;
    }
}