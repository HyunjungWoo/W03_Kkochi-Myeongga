// Skewer.cs
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Skewer : MonoBehaviour
{
    public Transform[] ingredientSlots;
    
    // 현재 몇 번째 슬롯까지 재료가 찼는지 기록
    private int nextSlotIndex = 0;

    private List<GameObject> addedIngredients = new List<GameObject>();

    private bool isSeasoned = false;

    public float price = 0f; // 꼬치의 가격

    // 외부에서 재료를 받아서 꼬치에 추가하는 함수
    public void AddIngredient(GameObject ingredient)
    {
        if (IsFull()) return;
        isSeasoned = false; // 재료가 추가되면 양념 상태 초기화

        // 1. 현재 채워야 할 슬롯 Transform을 가져옴
        Transform slotTransform = ingredientSlots[nextSlotIndex];
        ingredient.transform.SetParent(slotTransform);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;
        nextSlotIndex++;

        addedIngredients.Add(ingredient);
        price += 5; // 재료 하나당 가격 5 증가
    }

    public void ApplySeasoning(GameObject saucePrefab)
    {
        if (isSeasoned)
        {
            Debug.Log("이미 양념이 발라져 있습니다.");
            return;
        }

        // 꼬치에 꽂힌 모든 재료를 순회
        foreach (GameObject ingredient in addedIngredients)
        {
            bool alreadyHasSauce = false;

            // 2. 재료의 모든 자식 오브젝트를 검사
            foreach (Transform child in ingredient.transform)
            {
                // 3. 만약 자식 중에 "Sauce" 태그를 가진 오브젝트가 있다면
                if (child.CompareTag("Sauce"))
                {
                    alreadyHasSauce = true; // 양념이 있다고 표시
                    break; // 더 이상 검사할 필요 없으므로 반복 중단
                }
            }

            // 4. 검사를 마친 후, 양념이 없는 경우에만 새로 생성
            if (!alreadyHasSauce)
            {
                GameObject sauceEffect = Instantiate(saucePrefab, ingredient.transform.position, Quaternion.identity);
                sauceEffect.transform.SetParent(ingredient.transform);
            }
        }

        isSeasoned = true;
        price += 10;
        Debug.Log("양념 완료! 현재 꼬치 가격: " + price);
    }

    // 꼬치가 꽉 찼는지 확인하는 함수
    public bool IsFull()
    {
        return nextSlotIndex >= ingredientSlots.Length;
    }

    public void ClearSkewer()
    {
        foreach (GameObject ingredient in addedIngredients)
        {
            Destroy(ingredient);
        }
        addedIngredients.Clear();
        nextSlotIndex = 0;
        isSeasoned = false;
        price = 0f;
    }

    // 꼬치가 비어있는지 확인하는 함수
    public bool IsEmpty()
    {
        return addedIngredients.Count == 0;
    }
}