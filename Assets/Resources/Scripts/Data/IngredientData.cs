using UnityEngine;
using static SeasoningBottle;

public enum IngredientType
{
    Scallion,
    Cheese, 
    Chicken
}

public class IngredientData : MonoBehaviour
{
    public IngredientType ingredientType;
    public SeasoningType? seasoning = null;
    public bool HasSeasoning => seasoning.HasValue;
}
