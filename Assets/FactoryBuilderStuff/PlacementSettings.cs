using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputOrOutput = InputOutput.InputOrOutput;

public class PlacementSettings : MonoBehaviour
{
    /// <summary>
    /// The instance of PlacementSettings, making it a singleton
    /// </summary>
    public static PlacementSettings Instance { get; private set; }

    // Ensures that there is one and only one instance of PlacementSettings
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Whether or not the player is currently placing
    /// </summary>
    public bool CurrentlyPlacing = false;
    /// <summary>
    /// List of all machines which can be placed
    /// </summary>
    [SerializeField] public GameObject[] MachineList;
    /// <summary>
    /// Which machine should currently be placed
    /// </summary>
    public int MachineListIndex = 0;
    /// <summary>
    /// Which direction the machines should be placed facing
    /// </summary>
    public int PlaceRotation;
    /// <summary>
    /// Which mode of placing is active
    /// </summary>
    public PlacementMode PlacementMode = new();

    /// <summary>
    /// Input/Output configuration for placed conveyor belts
    /// </summary>
    public IOLock[] ConveyorBeltIOLocks = new IOLock[4];

    
}
