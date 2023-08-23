using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Direction = Orientation.Direction;

[CustomEditor(typeof(Machine), true)]
public class MachineInspectorEditor : Editor
{
    // Creates buttons for debug purposes
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Machine machine = (Machine)target;
        if (GUILayout.Button("Input Item North") && machine.PressButtonDebugItem != null)
        {
            machine.Input(Direction.North, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item East") && machine.PressButtonDebugItem != null)
        {
            machine.Input(Direction.East, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item South") && machine.PressButtonDebugItem != null)
        {
            machine.Input(Direction.South, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item West") && machine.PressButtonDebugItem != null)
        {
            machine.Input(Direction.West, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Dump Inputs"))
        {
            machine.DumpInputs();
        }
    }
}
