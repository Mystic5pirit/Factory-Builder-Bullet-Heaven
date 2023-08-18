using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    /// <summary>
    /// Recipe's inputs
    /// </summary>
    public ItemSO[] InputArray;
    /// <summary>
    /// Recipe's outputs
    /// </summary>
    public ItemSO[] OutputArray;
    /// <summary>
    /// How long the recipe takes
    /// </summary>
    public float BaseTimeToComplete;
}
