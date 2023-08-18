using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voider : Machine
{
    private void Start()
    {
        // Makes the voider not try to do anything in update
        IsActive = false;
    }
    /// <summary>
    /// Accepts input but does nothing with the inputted item
    /// </summary>
    /// <param name="inputDirection">Ignored</param>
    /// <param name="inputtedItem">Ignored</param>
    /// <returns>True</returns>
    public override bool Input(int inputDirection, ItemSO inputtedItem)
    {
        return true;
    }
    /// <summary>
    /// Does nothing
    /// </summary>
    protected override void CheckRecipe()
    {
        
    }
}
