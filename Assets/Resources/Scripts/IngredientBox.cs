// IngredientBox.cs
using TMPro;
using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가!
using System.Linq; // LINQ 기능을 사용하기 위해 추가! (LINQ는 C#의 데이터 쿼리 기능입니다.)

public class IngredientBox : MonoBehaviour
{
    // -- 인스펙터 연결 -- 
    public Skewer currentSkewer;
    public TextMeshProUGUI stockText;
    public GameObject ingredientPrefab; // 재료 프리팹 (인스펙터에서 연결)

    // === 재료 관련 변수 === 
    private List<GameObject> ingredientStockList = new List<GameObject>(); // 재고를 리스트로 관리

    void Awake()
    {
        // 2. 시작할 때, 자신의 모든 자식 오브젝트를 찾아서 리스트에 추가
        foreach (Transform child in transform)
        {
            ingredientStockList.Add(child.gameObject);
        }

        UpdateStockText(); // UI 텍스트 초기화
    }

    // UI 텍스트 업데이트 함수
    void UpdateStockText()
    {
        if (stockText != null)
        {
            // 현재 활성화된 아이템의 개수를 세어서 재고로 표시
            int currentStock = ingredientStockList.Count(item => item.activeInHierarchy);
            stockText.text = "재고: " + currentStock;
        }
    }

    // 마우스로 이 오브젝트를 클릭했을 때 호출되는 함수
    private void OnMouseDown()
    {
        // 꼬치가 있고, 꽉 차지 않았다면
        if (currentSkewer != null && !currentSkewer.IsFull())
        {
            // 1. 현재 켜져 있는(사용 가능한) 재고 오브젝트를 찾음
            GameObject itemToDeactivate = ingredientStockList.FirstOrDefault(item => item.activeInHierarchy);

            // 2. 만약 활성화된 아이템이 있다면 (재고가 있다면)
            if (itemToDeactivate != null)
            {
                // 3. 찾은 재고를 끔 (시각적으로만 재고 감소)
                itemToDeactivate.SetActive(false);

                // 4. 꼬치에 꽂아줄 실제 재료를 프리팹으로 새로 생성
                GameObject newIngredient = Instantiate(ingredientPrefab);

                // 5. 생성한 재료를 꼬치에 추가
                currentSkewer.AddIngredient(newIngredient);

                // 6. UI 업데이트
                UpdateStockText();
            }
            else
            {
                Debug.Log("재료가 모두 소진되었습니다!");
            }
        }
    }

    public void RefillStock()
    {
        // 모든 시각적 재고 아이템들을 순회하면서
        foreach (GameObject item in ingredientStockList)
        {
            // 다시 켜주기만 하면 끝!
            item.SetActive(true);
        }
        UpdateStockText();
        Debug.Log("모든 재료를 다시 채웠습니다!");
    }

   

}