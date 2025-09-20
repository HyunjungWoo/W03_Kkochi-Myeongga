using UnityEngine;

public class SeasoningBottle : MonoBehaviour
{
    // 이 양념통이 적용할 양념 재질(Material)
    public GameObject saucePrefab;

    // 이 양념이 무엇인지 구분하기 위한 타입 (옵션이지만 추천)
    public enum SeasoningType { Spicy, SoySauce, Salt }
    public SeasoningType type;

    // 게임 매니저나 다른 스크립트를 통해 현재 활성화된 꼬치를 받아와야 함
    // 여기서는 간단하게 public 변수로 선언
    public Skewer currentSkewer;

    private void OnMouseDown()
    {
        // 현재 선택된 꼬치가 있고, 그 꼬치가 비어있지 않다면
        if (currentSkewer != null && !currentSkewer.IsEmpty())
        {
            Debug.Log(type + " 양념을 바릅니다!");
            currentSkewer.ApplySeasoning(saucePrefab);
        }
        else
        {
            Debug.Log("양념을 바를 꼬치가 없습니다.");
        }
    }
}