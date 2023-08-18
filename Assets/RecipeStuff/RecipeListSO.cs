using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeListSO", menuName = "ScriptableObjects/RecipeListSO")]
public class RecipeListSO : ScriptableObject
{
    /// <summary>
    /// List of Recipes
    /// </summary>
    public RecipeSO[] RecipeList;
}
