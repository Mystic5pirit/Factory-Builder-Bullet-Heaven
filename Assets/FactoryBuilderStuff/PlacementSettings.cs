using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSettings : MonoBehaviour
{
    public static PlacementSettings Instance { get; private set; }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] public GameObject[] MachineList;
    public int MachineListIndex = 0;
    public int PlaceRotation = 0;
    public int PlacementMode = 0;

    public int[] ConveyorBeltOrientation = { 1, 0, 2, 0 };
}
