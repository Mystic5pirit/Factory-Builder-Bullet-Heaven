using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voider : Machine
{
    public override bool Input(int inputDirection, ItemSO inputtedItem)
    {
        return true;
    }

    protected override void CheckRecipe()
    {
        
    }
}
