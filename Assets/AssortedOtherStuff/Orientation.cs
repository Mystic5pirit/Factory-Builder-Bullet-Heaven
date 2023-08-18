using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class Orientation
{
    //Still need to deal with all of the spot that still use int instead of this
    /// <summary>
    /// Options for this enum: North, East, South, West
    /// </summary>
    public enum Direction { North = 0, East, South, West };
    /// <summary>
    /// The Variable
    /// </summary>
    public Direction FacingDirection;

    /// <summary>
    /// Cycles the variable by one
    /// </summary>
    public void RotateClockwise()
    {
        FacingDirection = (Direction)((int)(FacingDirection + 1) % 4);
    }
    /// <summary>
    /// Cycles the variable by -1
    /// </summary>
    public void RotateCounterclockwise()
    {
        FacingDirection = (Direction)((int)(FacingDirection - 1) % 4);
    }
    /// <summary>
    /// Cycles the variable by amout
    /// </summary>
    /// <param name="amount">How much to rotate</param>
    public void RotateClockwise(int amount)
    {
        FacingDirection = (Direction)((int)(FacingDirection + amount) % 4);
    }
    /// <summary>
    /// Cycles the variable by -amount
    /// </summary>
    /// <param name="amount">How much to rotate counterclockwise</param>
    public void RotateCounterclockwise(int amount)
    {
        FacingDirection = (Direction)((int)(FacingDirection - amount) % 4);
    }

    /// <summary>
    /// Override of ToString
    /// </summary>
    /// <returns>The name of the variable's type</returns>
    public override string ToString()
    {
        return Enum.GetName(typeof(Direction), FacingDirection);
    }

    /// <summary>
    /// Default constructor. Defaults variable to North
    /// </summary>
    public Orientation() : base()
    {
        FacingDirection = Direction.North;
    }
    /// <summary>
    /// Parameterized constructor. Sets variable to value
    /// </summary>
    /// <param name="enumValue">What the variable should be</param>
    public Orientation(Direction enumValue)
    {
        FacingDirection = enumValue;
    }

}
