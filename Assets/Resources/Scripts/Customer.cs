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

    void Start()
    {
        SetupCustomer();
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

        if (isCorrect)
        {
            finalPrice = skewer.price;
            // 주문이 맞을 때, 무작위 감사 인사 출력
            DialogueManager.Instance.Speak(customerData.GetRandomThankYou());
            skewer.ResetSkewer();
        }
        else
        {
            finalPrice = skewer.price * 0.3f;
            // 주문이 틀릴 때, 무작위 불평 출력
            DialogueManager.Instance.Speak(customerData.GetRandomComplaint());
            skewer.ResetSkewer();
        }

        return finalPrice;
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
