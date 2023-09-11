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
    /// Cycles the variable by 1 clockwise
    /// </summary>
    /// <returns>This object</returns>
    public Orientation RotateClockwise()
    {
        FacingDirection = (Direction)((int)(FacingDirection + 1) % 4);
        return this;
    }
    /// <summary>
    /// Cycles the variable by 1 counterclockwise
    /// </summary>
    /// <returns>This object</returns>
    public Orientation RotateCounterclockwise()
    {
        FacingDirection = (Direction)((int)(FacingDirection + 3) % 4);
        return this;
    }
    /// <summary>
    /// Cycles the variable by amount clockwise
    /// </summary>
    /// <param name="amount">How much to rotate</param>
    /// <returns>This object</returns>
    public Orientation RotateClockwise(int amount)
    {
        FacingDirection = (Direction)((int)(FacingDirection + amount) % 4);
        return this;
    }
    /// <summary>
    /// Cycles the variable by amount counterclockwise
    /// </summary>
    /// <param name="amount">How much to rotate counterclockwise</param>
    /// <returns>This object</returns>
    public Orientation RotateCounterclockwise(int amount)
    {
        FacingDirection = (Direction)(((int)FacingDirection + 3 * amount) % 4);
        return this;
    }

    /// <summary>
    /// Flips to opposite side
    /// </summary>
    /// <returns>This object</returns>
    public Orientation Flip()
    {
        FacingDirection = (Direction)((int)(FacingDirection + 2) % 4);
        return this;
    }

    /// <summary>
    /// Returns the variable cycled by 1 clockwise
    /// </summary>
    /// <returns>The variable cycled by 1 clockwise</returns>
    public Direction GetDirectionRotatedClockwise()
    {
        return (Direction)((int)(FacingDirection + 1) % 4);
    }
    /// <summary>
    /// Returns the variable cycled by 1 counterclockwise
    /// </summary>
    /// <returns>The variable cycled by 1 counterclockwise</returns>
    public Direction GetDirectionRotatedCounterclockwise()
    {
        return (Direction)((int)(FacingDirection + 3) % 4);
    }
    /// <summary>
    /// Returns the variable cycled by amount clockwise
    /// </summary>
    /// <param name="amount">How much to rotate by</param>
    /// <returns>The variable cycled by amount clockwise</returns>
    public Direction GetDirectionRotatedClockwise(int amount)
    {
        return (Direction)((int)(FacingDirection + amount) % 4);
    }
    /// <summary>
    /// Returns the variable cycled by amount counterclockwise
    /// </summary>
    /// <param name="amount">How much to rotate by</param>
    /// <returns>The variable cycled by amount counterclockwise</returns>
    public Direction GetDirectionRotatedCounterclockwise(int amount)
    {
        return (Direction)(((int)FacingDirection + 3 * amount) % 4);
    }
    /// <summary>
    /// Returns the opposite side from what the variable is
    /// </summary>
    /// <returns>The opposite side from what the variable is</returns>
    public Direction GetOppositeDirection()
    {
        return (Direction)((int)(FacingDirection + 2) % 4);
    }

    /// <summary>
    /// Override of ToString
    /// </summary>
    /// <returns>The name of the variable's type</returns>
    public override string ToString()
    {
        return Enum.GetName(typeof(Direction), FacingDirection);
    }

    public bool Equals(Orientation other)
    {
        return other.FacingDirection == FacingDirection;
    }

    /// <summary>
    /// Default constructor. Defaults variable to North
    /// </summary>
    public Orientation() : base()
    {
        FacingDirection = Direction.North;
    }
    /// <summary>
    /// Parameterized constructor. Sets variable to enumValue
    /// </summary>
    /// <param name="enumValue">What the variable should be</param>
    public Orientation(Direction enumValue)
    {
        FacingDirection = enumValue;
    }

    /// <summary>
    /// Parameterized constructor. Sets variable to enumValue
    /// </summary>
    /// <param name="enumValue">What the variable should be</param>
    public Orientation(int enumValue)
    {
        FacingDirection = (Direction)enumValue;
    }
}
