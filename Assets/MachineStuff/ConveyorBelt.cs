using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Machine
{
    /// <summary>
    /// Which input the ConveyorBelt is currently using
    /// </summary>
    protected int CurrentInputNumber = 0;

    /// <summary>
    /// Assigns relevant variables if there is something in the input
    /// </summary>
    protected override void CheckRecipe()
    {
        if (InputArray[CurrentInputNumber] != null)
        {
            CurrentlyDoingARecipe = true;
        }
        ProcessingCompletionTime = SpeedFactor;

    }

    // DOES NOT WORK CORRECTLY
    /// <summary>
    /// Calculates output cycling through the inputs
    /// </summary>
    /// <param name="outputNumber">Ignored</param>
    /// <returns>The item in the current input</returns>
    protected override ItemSO CalculateOutput(int outputNumber)
    {
        CurrentInputNumber = (CurrentInputNumber + 1) % UsedInputDirectionList.Count;
        return InputArray[CurrentInputNumber];

    }

    

}
