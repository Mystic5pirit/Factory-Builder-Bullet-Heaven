using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Direction = Orientation.Direction;
using InputOrOutput = InputOutput.InputOrOutput;

public class ConveyorBelt : Machine
{
    /// <summary>
    /// Which input the ConveyorBelt is currently using
    /// </summary>
    [SerializeField] protected int CurrentInputNumber = 0, CurrentOutputNumber = 0;

    protected List<Orientation> StoredOutputDirectionList = new List<Orientation>();

    public override void Start()
    {
        FigureOutAllSides();
        base.Start();
    }

    /// <summary>
    /// Assigns relevant variables if there is something in the input
    /// </summary>
    protected override void CheckRecipe()
    {
        if (InputArray.Length > 0)
        {
            if (InputArray[CurrentInputNumber] != null)
            {
                if (StoredOutputDirectionList.Count == 0)
                {
                    return;
                }
                CurrentlyDoingARecipe = true;
                CurrentOutputNumber = (CurrentOutputNumber + 1) % StoredOutputDirectionList.Count;
                UsedOutputDirectionList.Clear();
                UsedOutputDirectionList.Add(StoredOutputDirectionList[CurrentOutputNumber]);
            } else
            {
                if (UsedInputDirectionList.Count == 0)
                {
                    Debug.Log("0!");
                }
                CurrentInputNumber = (CurrentInputNumber + 1) % UsedInputDirectionList.Count;
            }
            ProcessingCompletionTime = SpeedFactor;
        }

    }

    // DOES NOT WORK CORRECTLY
    /// <summary>
    /// Calculates output cycling through the inputs
    /// </summary>
    /// <param name="outputNumber">Ignored</param>
    /// <returns>The item in the current input</returns>
    protected override ItemSO CalculateOutput(int outputNumber)
    {
        // CurrentInputNumber is not what is wanted
        return InputArray[CurrentInputNumber];

    }

    /// <summary>
    /// Returns the Input/Output array of a new conveyorbelt that would be placed
    /// </summary>
    /// <returns>The Input/Output array of a new conveyorbelt that would be placed</returns>
    public override IOLock[] GetIOArray()
    {
        return PlacementSettings.Instance.ConveyorBeltIOLocks;
    }

    public override void RecalculateSide(Direction side)
    {
        
        if (!IOArray[(int)side].Locked)
        {

            Machine adjacentMachine = GetAdjacentMachine(side);

            SetSideIOLock(side, adjacentMachine.GetIOLock(new Orientation(side).GetOppositeDirection()).IO.GetSwappedIO());

        }
    }

    public override void FigureOutAllSides()
    {

        for (int i = 0; i < IOArray.Length; i++)
        {
            IOLock iOLock = PlacementSettings.Instance.ConveyorBeltIOLocks[(i + PlacementSettings.Instance.PlaceRotation) % 4];
            SetSideIOLock((Direction)i, iOLock);
            Direction direction = (Direction)i;
            Direction oppositeDirection = new Orientation(i).GetOppositeDirection();
            Machine adjacentMachine = GetAdjacentMachine(direction);
            
            if (!IOArray[i].Locked)
            {
                SetSideIOLock(direction, adjacentMachine.GetIOLock(oppositeDirection).IO.GetSwappedIO());
                if (IOArray[i].IO.IOType == InputOrOutput.None)
                {

                    if (adjacentMachine.GetMachineType() == Type && !adjacentMachine.GetIOLock(oppositeDirection).Locked && adjacentMachine.GetIOLock(oppositeDirection).IO.IOType == InputOrOutput.None)
                    {
                        
                        SetSideIOLock(direction, InputOrOutput.Input);
                        
                    }
                }
            }
            adjacentMachine.RecalculateSide(oppositeDirection);
        }
    }

    public override void Rotate()
    {

    }

    public override void SetCalculatedValues()
    {
        base.SetCalculatedValues();
        // Adds used output directions to the list of used output directions
        StoredOutputDirectionList.Clear();
        for (int i = 0; i < IOArray.Length; i++)
        {
            if (IOArray[i].IO.IOType == InputOrOutput.Output)
            {
                StoredOutputDirectionList.Add(new Orientation(i));

            }
        }
    }

    /// <summary>
    /// Tries to output to all adjacent machines, voids any faliures if there is one success
    /// </summary>
    /// <returns>Success of outputting any items</returns>
    protected override bool Output()
    {
        // Checks if all outputs can output
        int countOfValidOutputs = 0;
        for (int i = 0; i < UsedOutputDirectionList.Count; i++)
        {
            Direction realDirection = UsedOutputDirectionList[i].GetDirectionRotatedClockwise(Rotation);
            if (GetAdjacentMachine(realDirection).Input(realDirection))
            {
                countOfValidOutputs++;
            }
        }

        // Outputs if possible or if VoidBlockedOutputs is true, then it will delete any outputs which cannot be outputted
        if ((VoidBlockedOutputs || countOfValidOutputs == UsedOutputDirectionList.Count))
        {
            for (int i = 0; i < UsedOutputDirectionList.Count; i++)
            {
                Direction realDirection = UsedOutputDirectionList[i].GetDirectionRotatedClockwise(Rotation);
                GetAdjacentMachine(realDirection).Input(realDirection, CalculateOutput(i));
                OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].GetComponent<MeshFilter>().mesh = null;
            }
            CurrentlyDoingARecipe = false;
            ProcessingTimer = 0;

            DumpInput(CurrentInputNumber);
        }
        // Returns the success of outputting
        return !CurrentlyDoingARecipe;
    }

    protected override void Update()
    {
        // If it isn't doing a recipe then it checks if it can
        if (!CurrentlyDoingARecipe)
        {
            CheckRecipe();
            ProcessingTimer = 0;
        }
        else if (ProcessingTimer >= ProcessingCompletionTime) // Checks if the recipe is complete
        {
            // Tries to output then resets variables if it did
            if (Output())
            {
                HasSwappedItemTypes = false;
                CurrentlyDoingARecipe = false;
                for (int i = 0; i < OutputItemDisplayerList.Length; i++)
                {
                    OutputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = null;
                }
                CurrentInputNumber = (CurrentInputNumber + 1) % UsedInputDirectionList.Count;
            }
        }
        else // Currently doing a recipe that has not completed
        {
            // First half of processing
            if (ProcessingTimer < ProcessingCompletionTime / 2)
            {
                // Moves all input displays to _centerOfMovement.transform.position
                InputItemDisplayerList[(int)UsedInputDirectionList[CurrentInputNumber].FacingDirection].transform.localPosition = Vector3.Lerp(ItemDisplayStartingPositionList[(int)UsedInputDirectionList[CurrentInputNumber].FacingDirection], CenterOfMovement.transform.localPosition, ProcessingTimer / ProcessingCompletionTime * 2);
            }
            else // Second half of processing
            {
                // Called when it switches over from first half to second half
                if (!HasSwappedItemTypes)
                {
                    // Makes the input displays not render
                    InputItemDisplayerList[(int)UsedInputDirectionList[CurrentInputNumber].FacingDirection].GetComponent<MeshFilter>().mesh = null;
                    
                    // Sets the output displays to the right mesh and material
                    for (int i = 0; i < UsedOutputDirectionList.Count; i++)
                    {

                        OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].GetComponent<MeshFilter>().mesh = CalculateOutput(i).ItemMesh;
                        Destroy(OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].GetComponent<MeshRenderer>().material); // Removes the previous material to not create copies that do not have any references
                        OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].GetComponent<MeshRenderer>().material = CalculateOutput(i).ItemMaterial;
                    }
                    // Resets input display positions
                    for (int i = 0; i < UsedInputDirectionList.Count; i++)
                    {
                        InputItemDisplayerList[(int)UsedInputDirectionList[i].FacingDirection].transform.localPosition = ItemDisplayStartingPositionList[(int)UsedInputDirectionList[i].FacingDirection];
                    }
                    // Makes it only do this swap once per recipe opperation
                    HasSwappedItemTypes = true;
                }
                // Moves the output displays outward from _centerOfMovement.transform.position
                for (int i = 0; i < UsedOutputDirectionList.Count; i++)
                {
                    OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].transform.localPosition = Vector3.Lerp(CenterOfMovement.transform.localPosition, ItemDisplayStartingPositionList[(int)UsedOutputDirectionList[i].FacingDirection], (ProcessingTimer - 0.5f) / ProcessingCompletionTime * 2);
                }

            }

        }
        // Increases the timer
        ProcessingTimer += Time.deltaTime;

    }
}
