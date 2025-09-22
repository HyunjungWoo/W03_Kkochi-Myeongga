// Skewer.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SeasoningBottle;

public class Skewer : MonoBehaviour
{
    public Transform[] ingredientSlots;

    // 현재 몇 번째 슬롯까지 재료가 찼는지 기록
    private int nextSlotIndex = 0;
    private List<GameObject> addedIngredients = new List<GameObject>();

    private bool isSeasoned = false;
    public float price = 0f; // 꼬치의 가격

    private Vector3 originalPosition; // 꼬치의 원래 위치를 저장할 변수
    private Transform handTransform;  // 꼬치를 잡은 손의 Transform

    private void Awake()
    {
        originalPosition = transform.position; // 시작할 때 위치 저장
    }

    public void Grab(Transform hand)
    {
        handTransform = hand;
        transform.SetParent(handTransform);
        transform.localPosition = Vector3.zero; // 손 위치에 맞춤
    }

    public void Release()
    {
        handTransform = null;
        transform.SetParent(null);
        transform.position = originalPosition; // 원래 위치로 돌아감
    }


    #region 꼬치관련 함수들

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
        price += 10; // 재료 하나당 가격 5 증가
    }

    public void ApplySeasoning(GameObject saucePrefab, SeasoningType type)
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
                var data = ingredient.GetComponent<IngredientData>();

                GameObject sauceEffect = Instantiate(saucePrefab, ingredient.transform.position, Quaternion.identity);
                sauceEffect.transform.SetParent(ingredient.transform);
                data.seasoning = type;

                Debug.Log($"{type} 양념이 {ingredient.name}에 발라졌습니다.");

            }
        }

        isSeasoned = true;
        price += 5;
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

    public void ResetSkewer()
    {
        // 1. 꼬치에 꽂힌 모든 재료 오브젝트를 파괴
        foreach (GameObject ingredient in addedIngredients)
        {
            Destroy(ingredient);
        }
        // 2. 재료 목록 리스트를 비움
        addedIngredients.Clear();

        // 3. 모든 상태 변수를 초기값으로 리셋
        nextSlotIndex = 0;
        isSeasoned = false; // (isSeasoned 변수가 있다면)
        price = 0f; // (price 변수가 있다면)
        Release();

    }
    #endregion

    #region 고객과 상호작용 코드
    public bool MatchesRecipe(RecipeData recipeData)
    {
        var skewerItems = addedIngredients.Select(i => i.GetComponent<IngredientData>()).ToList();
        var requiredItems = recipeData.recipe.ingredientRecipes;

        // 1. 레시피에 없는 재료가 꼬치에 있는지 확인 (있으면 실패)
        var recipeIngredientTypes = requiredItems.Select(ir => ir.ingredient).ToHashSet();
        if (skewerItems.Any(si => !recipeIngredientTypes.Contains(si.ingredientType)))
        {
            Debug.Log($"실패: 레시피에 없는 재료가 추가되었습니다.");
            return false;
        }

        // 2. isSpecial이 true일 때만 정확한 개수와 양념을 확인
        if (recipeData.recipe.isSpecial)
        {
            if (skewerItems.Count != requiredItems.Count)
            {
                Debug.Log("실패: 스페셜 레시피는 재료 개수가 정확히 일치해야 합니다.");
                return false;
            }

            var skewerItemsWithSeasoning = skewerItems.Select(i => new { Type = i.ingredientType, Seasoning = i.seasoning ?? SeasoningType.None }).OrderBy(x => x.Type).ThenBy(x => x.Seasoning).ToList();
            var requiredItemsWithSeasoning = requiredItems.Select(ir => new { Type = ir.ingredient, Seasoning = ir.seasoningType }).OrderBy(x => x.Type).ThenBy(x => x.Seasoning).ToList();

            if (!skewerItemsWithSeasoning.SequenceEqual(requiredItemsWithSeasoning))
            {
                Debug.Log("실패: 스페셜 레시피의 재료 또는 양념 조합이 일치하지 않습니다.");
                return false;
            }
        }
        else // isSpecial이 false일 경우 재료별 개수를 확인
        {
            var skewerCounts = skewerItems.GroupBy(si => si.ingredientType).ToDictionary(g => g.Key, g => g.Count());
            var requiredCounts = requiredItems.GroupBy(ir => ir.ingredient).ToDictionary(g => g.Key, g => g.Count());

            // 3. 재료 종류별로 꼬치에 레시피 개수 이상이 있는지 확인
            foreach (var requiredCount in requiredCounts)
            {
                if (!skewerCounts.ContainsKey(requiredCount.Key) || skewerCounts[requiredCount.Key] < requiredCount.Value)
                {
                    Debug.Log($"실패: 재료 '{requiredCount.Key}'의 개수가 부족합니다. 최소 {requiredCount.Value}개가 필요합니다.");
                    return false;
                }
            }
        }

        Debug.Log("성공: 레시피를 만족합니다!");
        return true;
    }
    #endregion

}