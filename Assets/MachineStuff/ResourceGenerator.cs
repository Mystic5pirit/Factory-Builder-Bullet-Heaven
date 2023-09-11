using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : Machine
{
    /// <summary>
    /// What the resource generator outputs
    /// </summary>
    [SerializeField] protected ItemSO ItemType;

    public override void Start()
    {
        base.Start();
        // Makes the ResourceGenerator not try to do anything in update
        this.enabled = false;
    }

    /// <summary>
    /// Calculated the item which should be outputted, ignores the parameter
    /// </summary>
    /// <param name="outputNumber">Ignored</param>
    /// <returns>ItemType</returns>
    /// <exception cref="No Item chosen to output"></exception>
    protected override ItemSO CalculateOutput(int outputNumber)
    {
        if (ItemType == null)
        {
            throw new Exception("No Item chosen to output");
        }
        return ItemType;
    }
    /// <summary>
    /// Sets recipe related variables
    /// </summary>
    protected override void CheckRecipe()
    {
        CurrentlyDoingARecipe = true;
        ProcessingCompletionTime = ItemType.CreationTime * SpeedFactor;
    }



    
}

