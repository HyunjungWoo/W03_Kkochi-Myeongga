using UnityEngine;
using System.Collections.Generic;
using static SeasoningBottle;

// 재료와 필요한 양념을 한 쌍으로 묶는 클래스
[System.Serializable]
public class IngredientRecipe
{
    public IngredientType ingredient;
    public SeasoningType seasoningType;
}

// 여러 IngredientRecipe를 담는 레시피 클래스
[System.Serializable]
public class Recipe
{
    public string recipeName;
    public List<IngredientRecipe> ingredientRecipes;
    public bool isSpecial; // -- 소스까지 정확히 지켜야 하는 특수 레시피 여부
}

// 실제 레시피 데이터를 담는 스크립터블 오브젝트
[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe DataNew")]
public class RecipeData : ScriptableObject
{
    public Recipe recipe;
}