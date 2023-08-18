using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementMode
{
    //Still need to deal with all of the spot that still use int instead of this
    /// <summary>
    /// Options for this enum: Drag, Line, DragRemove, RectangleRemove, Rotate
    /// </summary>
    public enum PlacementModeTypes { Drag = 0, Line, DragRemove, RectangleRemove, Rotate }
    /// <summary>
    /// The Variable
    /// </summary>
    public PlacementModeTypes SelectedPlacementMode;

    /// <summary>
    /// Cycles the variable by one
    /// </summary>
    public void Cycle()
    {
        SelectedPlacementMode = (PlacementModeTypes)((int)(SelectedPlacementMode + 1) % 5);
    }

    /// <summary>
    /// Sets the variable to type
    /// </summary>
    /// <param name="type">What the variable should be</param>
    public void SetMode(PlacementModeTypes type)
    {
        SelectedPlacementMode = type;
    }
    /// <summary>
    /// Override of ToString
    /// </summary>
    /// <returns>Name of selected mode + " Mode"</returns>
    public override string ToString()
    {
        return Enum.GetName(typeof(PlacementModeTypes), SelectedPlacementMode) + " Mode";
    }

    /// <summary>
    /// Default constructor. Defaults variable to Drag
    /// </summary>
    public PlacementMode()
    {
        SelectedPlacementMode = PlacementModeTypes.Drag;
    }
    /// <summary>
    /// Parameterized constructor. Sets variable to value
    /// </summary>
    /// <param name="value">What the variable should be</param>
    public PlacementMode(PlacementModeTypes value)
    {
        SelectedPlacementMode = value;
    }
}
