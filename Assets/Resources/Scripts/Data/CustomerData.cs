using System.Collections.Generic;
using UnityEngine;
using static SeasoningBottle;

public enum CustomerType
{
    Normal,     // 일반 손님
    Student,    // 초등학생 (외상)
    Beggar      // 거지 (아무거나 받음, 돈 안 냄)
}

[CreateAssetMenu(fileName = "CustomerData", menuName = "Scriptable Objects/CustomerData")]
public class CustomerData : ScriptableObject
{
    public CustomerType customerType = CustomerType.Normal; // 고객 타입

    // 스프라이트 번호
    public List<Sprite> portraitList;
    public RecipeData favoriteOrder; // 선호하는 주문

    // 대사 
    public int patience; // 인내심 (시간 제한)

    public List<string> thankYou; // 주문이 맞았을 때 대사
    public List<string> complaint; // 주문이 틀렸을 때 대사
    public List<string> orderPrompt; // 주문할 때 대사

    // 대사 목록에서 무작위로 하나를 반환하는 함수들
    public string GetRandomThankYou()
    {
        if (thankYou.Count == 0) return "Thank you!";
        return thankYou[Random.Range(0, thankYou.Count)];
    }
    public Sprite GetRandomPortrait()
    {
        if (portraitList.Count == 0)
        {
            Debug.LogWarning("고객 초상화 목록이 비어있습니다!");
            return null;
        }
        return portraitList[Random.Range(0, portraitList.Count)];
    }

    public string GetRandomComplaint()
    {
        if (complaint.Count == 0) return "This is not what I ordered!";
        return complaint[Random.Range(0, complaint.Count)];
    }

    public string GetRandomOrderHint()
    {
        if (orderPrompt.Count == 0) return "What a great meal!";
        return orderPrompt[Random.Range(0, orderPrompt.Count)];
    }
}
