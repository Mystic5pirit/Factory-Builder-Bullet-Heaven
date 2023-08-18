using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerMachine : Machine
{
    /// <summary>
    /// BlockerMachine always returns false and does not allow anything to be inputted into it
    /// </summary>
    /// <param name="inputDirection"></param>
    /// <param name="inputtedItem"></param>
    /// <returns>false</returns>
    public override bool Input(int inputDirection, ItemSO inputtedItem)
    {
        return false;
    }

    // Makes it not do anything during Update
    private void Start()
    {
        IsActive = false;
    }
}
