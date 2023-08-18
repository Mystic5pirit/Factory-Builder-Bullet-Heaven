using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class InputOutput
{
    //Still need to deal with all of the spot that still use int instead of this
    /// <summary>
    /// Options for this enum: None, Input, Output
    /// </summary>
    public enum InputOrOutput { None, Input, Output};
    /// <summary>
    /// The Variable
    /// </summary>
    public InputOrOutput IOType;

    /// <summary>
    /// Cycles the variable between None, Input, and Output
    /// </summary>
    public void CycleIO()
    {
        IOType = (InputOrOutput)((int)(IOType + 1) % 3);
    }
    /// <summary>
    /// Sets the variable to value
    /// </summary>
    /// <param name="value">Targeted Input/Output</param>
    public void SetIO(InputOrOutput value)
    {
        IOType = value;
    }
    /// <summary>
    /// Override of ToString
    /// </summary>
    /// <returns>Name of the variable's type</returns>
    public override string ToString()
    {
        return Enum.GetName(typeof(InputOrOutput), IOType);
    }

    /// <summary>
    /// Default constructor. Defaults variable to none
    /// </summary>
    public InputOutput()
    {
        IOType = InputOrOutput.None;
    }
    /// <summary>
    /// Parameterized constructor. Sets variable to value
    /// </summary>
    /// <param name="value">What the variable should be</param>
    public InputOutput(InputOrOutput value)
    {
        IOType = value;
    }

}
