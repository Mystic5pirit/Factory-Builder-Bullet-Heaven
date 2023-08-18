using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Machine), true)]
public class MachineInspectorEditor : Editor
{
    // Creates buttons for debug purposes
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Machine machine = (Machine)target;
        if (GUILayout.Button("Input Item From Top") && machine.PressButtonDebugItem != null)
        {
            machine.Input(0, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item From Right") && machine.PressButtonDebugItem != null)
        {
            machine.Input(1, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item From Bottom") && machine.PressButtonDebugItem != null)
        {
            machine.Input(2, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Input Item From Left") && machine.PressButtonDebugItem != null)
        {
            machine.Input(3, machine.PressButtonDebugItem);
        }
        if (GUILayout.Button("Dump Inputs"))
        {
            machine.DumpInputs();
        }
    }
}
