using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Orientation.Direction;

public class BlockerMachine : Machine
{
    /// <summary>
    /// BlockerMachine always returns false and does not allow anything to be inputted into it
    /// </summary>
    /// <param name="inputDirection"></param>
    /// <param name="inputtedItem"></param>
    /// <returns>false</returns>
    public override bool Input(Direction inputDirection, ItemSO inputtedItem = null)
    {
        return false;
    }



    private void Start()
    {
        // Makes the BlockerMachine not try to do anything in update
        this.enabled = false;
    }
}
