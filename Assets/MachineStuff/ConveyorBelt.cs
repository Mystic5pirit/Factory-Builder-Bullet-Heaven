using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Machine
{
    protected int CurrentInput = 0;
    protected float BaseConveyorBeltSpeed = 1;
    
    protected override void CheckRecipe()
    {
        if (InputArray[CurrentInput] != null)
        {
            CurrentlyDoingARecipe = true;
        }
        ProcessingCompletionTime = BaseConveyorBeltSpeed;

    }
    protected override ItemSO CalculateOutput(int outputNumber)
    {
        CurrentInput = (CurrentInput + 1) % InputArray.Length;
        return InputArray[CurrentInput];

    }

    

}
