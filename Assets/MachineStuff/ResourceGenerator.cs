using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : Machine
{
    [SerializeField] protected ItemSO ItemType;
    [SerializeField] protected float BaseOutputSpeed = 1;
    [SerializeField] protected bool IsEnabled = false;

    protected override ItemSO CalculateOutput(int outputNumber)
    {
        return ItemType;
    }
    protected override void CheckRecipe()
    {
        CurrentlyDoingARecipe = true;
        ProcessingCompletionTime = BaseOutputSpeed * SpeedFactor;

    }
    protected override void Update()
    {
        if (IsEnabled) { base.Update(); }
    }

    
}

