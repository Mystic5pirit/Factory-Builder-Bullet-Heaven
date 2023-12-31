using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Orientation.Direction;

public class Voider : Machine
{
    public override void Start()
    {
        base.Start();
        // Makes the voider not try to do anything in update
        this.enabled = false;
    }
    /// <summary>
    /// Accepts input but does nothing with the inputted item
    /// </summary>
    /// <param name="inputDirection">Ignored</param>
    /// <param name="inputtedItem">Ignored</param>
    /// <returns>True</returns>
    public override bool Input(Direction inputDirection, ItemSO inputtedItem)
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
