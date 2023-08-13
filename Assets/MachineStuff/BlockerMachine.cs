using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerMachine : Machine
{
    public override bool Input(int inputDirection, ItemSO inputtedItem)
    {
        return false;
    }

    protected override void Update()
    {
    }
}
