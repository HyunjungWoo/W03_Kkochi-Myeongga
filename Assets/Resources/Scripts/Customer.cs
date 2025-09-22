using NUnit.Framework;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public CustomerData customerData; // 여기에 고객 데이터 ScriptableObject를 연결합니다.
    public TextMeshProUGUI dialogueTextUI; // 1번에서 만든 UI Text를 여기에 연결
    private SpriteRenderer portraitSR;

    private Skewer skewerInRange;
    public Button leaveButton; // 나갈 버튼 UI
    void Start()
    {
        SetupCustomer();

        // 버튼이 있다면 초기 상태를 비활성화로 설정
        if (leaveButton != null)
        {
            leaveButton.gameObject.SetActive(false);
        }
    }

    // 나가기 버튼을 표시하는 함수
    public void ShowLeaveButton()
    {
        if (leaveButton != null)
        {
            leaveButton.gameObject.SetActive(true);
        }
    }

    // 나가기 버튼을 숨기는 함수
    public void HideLeaveButton()
    {
        if (leaveButton != null)
        {
            leaveButton.gameObject.SetActive(false);
        }
    }

    public void SetupCustomer()
    {
        if (customerData == null) return;
        portraitSR = GetComponent<SpriteRenderer>();

        // 고객이 Normal이라면 랜덤으로 선택
        if (customerData.customerType == CustomerType.Normal)
        {
            portraitSR.sprite = customerData.GetRandomPortrait();
        }
        else
          portraitSR.sprite = customerData.portraitList[0];
        
        Debug.Log($"손님 '{this.name}' 등장! 주문: {customerData.favoriteOrder}");

        DialogueManager.Instance.Speak(customerData.GetRandomOrderHint());
    }

    public float CalculatePayment(Skewer skewer)
    {
        bool isCorrect = skewer.MatchesRecipe(customerData.favoriteOrder);
        float finalPrice = 0;

        if(customerData.customerType == CustomerType.Student) 
        {
            DialogueManager.Instance.Speak("개이득~ 야미");
            GameManager.Instance.AddAffection(5); // 호감도 감소
            skewer.ResetSkewer();
            return -50;
        }
        else if(customerData.customerType == CustomerType.Beggar) // 거지
        {
            DialogueManager.Instance.Speak(customerData.GetRandomThankYou());
            GameManager.Instance.AddAffection(5); // 호감도 감소
            skewer.ResetSkewer();
            return 0;
        }

        if (isCorrect)
        {
            finalPrice = skewer.price;
            // 주문이 맞을 때, 무작위 감사 인사 출력
            DialogueManager.Instance.Speak(customerData.GetRandomThankYou());
            GameManager.Instance.AddAffection(5); // 호감도 감소
            skewer.ResetSkewer();
        }
        else
        {
            finalPrice = skewer.price * 0.3f;
            // 주문이 틀릴 때, 무작위 불평 출력
            DialogueManager.Instance.Speak(customerData.GetRandomComplaint());
            GameManager.Instance.AddAffection(-5); // 호감도 감소
            skewer.ResetSkewer();
        }

        return finalPrice;
    }

    // 스프라이트를 외부에서 변경할 수 있도록 public 함수 추가
    public void SetPortrait(Sprite newPortrait)
    {
        if (portraitSR != null)
        {
            portraitSR.sprite = newPortrait;
        }
    }



    #region 고객 충돌감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stick"))
        {
            skewerInRange = other.GetComponent<Skewer>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stick") && skewerInRange == other.GetComponent<Skewer>())
        {
            skewerInRange = null;
        }
    }

    #endregion


}
