using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;
using static UnityEngine.Rendering.DebugUI;
using InputOrOutput = InputOutput.InputOrOutput;
using Direction = Orientation.Direction;
using System.Buffers.Text;
using System.Drawing;

[Serializable]
public class Machine : MonoBehaviour
{
    // Configuration settings
    [Header("Configuration Settings")]
    /// <summary>
    /// Type of machine
    /// </summary>
    [SerializeField] protected string Type;
    /// <summary>
    /// Array of if a side is an input or output
    /// </summary>
    [SerializeField, Tooltip("Element 0 is North, Element 1 is East, etc.")] public IOLock[] IOArray = new IOLock[4];
    /// <summary>
    /// List of recipes
    /// </summary>
    [SerializeField] protected RecipeListSO RecipeList;
    /// <summary>
    /// How fast the machine goes (multiplier)
    /// </summary>
    [SerializeField] protected float SpeedFactor;
    /// <summary>
    /// The GameObjects of the item displays and miniature conveyor belts
    /// </summary>
    [SerializeField] protected GameObject[] InputItemDisplayerList, OutputItemDisplayerList;
    protected List<Orientation> UsedInputDirectionList = new(), UsedOutputDirectionList = new();
    protected Vector3[] ItemDisplayStartingPositionList = new Vector3[4];
    [SerializeField] protected GameObject[] SideConveyors = new GameObject[4];
    /// <summary>
    /// Center of movement for the item displays to converge to
    /// </summary>
    [SerializeField] protected GameObject CenterOfMovement;
    [SerializeField] protected bool VoidBlockedOutputs = false;

    // POSITIONING VARIABLES
    /// <summary>
    /// Which direction the machine is facing
    /// </summary>
    protected int Rotation = 0;
    /// <summary>
    /// Location of the machine
    /// </summary>
    protected Vector2Int Location;

    // CONTENT OF THE MACHINE VARIABLES
    [SerializeField] protected ItemSO[] InputArray;
    protected bool CurrentlyDoingARecipe = false;//can be removed
    protected bool HasSwappedItemTypes = false;

    // RECIPE VARIABLES
    protected RecipeSO CurrentRecipe;
    protected float ProcessingTimer;
    protected float ProcessingCompletionTime;

    // Used for debugging
    public ItemSO PressButtonDebugItem;




    public virtual void Start()
    {
        if (PlacementSettings.Instance.CurrentlyPlacing)
        {
            // Ensures the machine doesn't do anything as a preview
            this.enabled = false;
            return;
        }
        SetCalculatedValues();
        for (int i = 0; i < 4; i++)
        {
            Orientation adjustedOrientation = new Orientation(i).RotateClockwise(Rotation);
            Machine adjacentMachine = GetAdjacentMachine(adjustedOrientation.FacingDirection);
            adjacentMachine.RecalculateSide(adjustedOrientation.GetOppositeDirection());
        }
    }

    /// <summary>
    /// ToString method
    /// </summary>
    /// <returns>Type of machine ": R = " Rotation of machine</returns>
    public override string ToString()
    {
        return (Type + ": R = " + Rotation);
    }

    /// <summary>
    /// Rotates the machine
    /// </summary>
    public virtual void Rotate()
    {
        Rotation = (Rotation + 1) % 4;
        gameObject.transform.Rotate(0, 90, 0);
        for (int i = 0; i < 4; i++)
        {
            Orientation adjustedOrientation = new Orientation(i).RotateClockwise(Rotation);
            Machine adjacentMachine = GetAdjacentMachine(adjustedOrientation.FacingDirection);
            adjacentMachine.RecalculateSide(adjustedOrientation.GetOppositeDirection());
        }
    }

    /// <summary>
    /// Sets the internal stored location
    /// </summary>
    /// <param name="location"></param>
    public void SetLocation(Vector2Int location)
    {
        Location = location;
    }
    /// <summary>
    /// Returns the location
    /// </summary>
    /// <returns>The Location</returns>
    public Vector2Int GetLocation()
    {
        return Location;
    }

    protected virtual void Update()
    {
        // If it isn't doing a recipe then it checks if it can
        if (!CurrentlyDoingARecipe)
        {
            CheckRecipe();
            ProcessingTimer = 0;
        } else if (ProcessingTimer >= ProcessingCompletionTime) // Checks if the recipe is complete
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
            }
        } else // Currently doing a recipe that has not completed
        {
            // First half of processing
            if (ProcessingTimer < ProcessingCompletionTime / 2)
            {
                // Moves all input displays to _centerOfMovement.transform.position
                for (int i = 0; i < UsedInputDirectionList.Count; i++)
                {
                    InputItemDisplayerList[(int)UsedInputDirectionList[i].FacingDirection].transform.localPosition = Vector3.Lerp(ItemDisplayStartingPositionList[(int)UsedInputDirectionList[i].FacingDirection], CenterOfMovement.transform.localPosition, ProcessingTimer / ProcessingCompletionTime * 2);
                }
            } else // Second half of processing
            {
                // Called when it switches over from first half to second half
                if (!HasSwappedItemTypes)
                {
                    // Makes the input displays not render
                    foreach (GameObject item in InputItemDisplayerList)
                    {
                        item.GetComponent<MeshFilter>().mesh = null;
                    }
                    // Sets the output displays to the right mesh and material
                    for (int i = 0;i < UsedOutputDirectionList.Count; i++)
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

    /// <summary>
    /// Checks if there is a valid recipe and assigns relevant variables
    /// </summary>
    protected virtual void CheckRecipe()
    {
        // do/while ending variable
        bool foundRecipe = false;
        // Index to transverse RecipeList.RecipeList
        int tempIndex = -1;
        do
        {
            tempIndex++;
            // Exit the loop if no recipes were found
            if (tempIndex == RecipeList.RecipeList.Length) { break; }
            // Checks if each item in the recipe matches the inputted item in the slot
            int correctnessCounter = 0;
            for (int i = 0; i < InputArray.Length; i++)
            {
                if (InputArray[i] == RecipeList.RecipeList[tempIndex].InputArray[i])
                {
                    correctnessCounter++;
                }
            }
            foundRecipe = correctnessCounter == InputArray.Length;
        } while (!foundRecipe);
        // Sets variables for setting the current recipe
        if (foundRecipe)
        {
            CurrentlyDoingARecipe = true;
            CurrentRecipe = RecipeList.RecipeList[tempIndex];
            ProcessingCompletionTime = CurrentRecipe.BaseTimeToComplete * SpeedFactor;
        }
    }

    /// <summary>
    /// Gets a reference to the adjacent machine
    /// </summary>
    /// <param name="direction">Which direction to get from</param>
    /// <returns>A reference to the adjacent machine</returns>
    /// <exception cref="Inputted number is too high"></exception>
    protected ref Machine GetAdjacentMachine(Direction direction)
    {
        switch(direction)
        {
            case Direction.North:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.up);
            case Direction.East:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.right);
            case Direction.South:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.down);
            case Direction.West:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.left);
            default: 
                throw new Exception("Inputted number is too high");
        }
    }

    /// <summary>
    /// Sets any values that need to be pre-calculated
    /// </summary>
    public virtual void SetCalculatedValues()
    {
        // Adds used output directions to the list of used output directions
        UsedOutputDirectionList.Clear();
        for (int i = 0; i < IOArray.Length; i++)
        {
            if (IOArray[i].IO.IOType == InputOrOutput.Output)
            {
                UsedOutputDirectionList.Add(new Orientation(i));
                
            }
        }
        // Adds used input directions to the list of used input directions
        UsedInputDirectionList.Clear();
        for (int i = 0; i < IOArray.Length; i++)
        {
            if (IOArray[i].IO.IOType == InputOrOutput.Input)
            {
                UsedInputDirectionList.Add(new Orientation(i));
            }
        }
        List<ItemSO> tempInputs = new List<ItemSO>();
        for (int i = 0; i < InputArray.Length; i++)
        {
            tempInputs.Add(InputArray[i]);
        }
        InputArray = new ItemSO[UsedInputDirectionList.Count];
        for (int i = 0; i < InputArray.Length && i < tempInputs.Count; i++)
        {
            InputArray[i] = tempInputs[i];
        }

        // Sets starting positions for item displays in the list of starting positions
        for (int i = 0; i < InputItemDisplayerList.Length; i++)
        {
            ItemDisplayStartingPositionList[i] = InputItemDisplayerList[i].transform.localPosition;
        }
        // Makes the little conveyor belts correct
        for (int i = 0; i < 4; i++)
        {
            FixDisplayedConveyorBelt((Direction)i);
        }
    }

    

    /// <summary>
    /// Inputs into this machine
    /// </summary>
    /// <param name="inputDirection">Which direction the inputted item is going</param>
    /// <param name="inputtedItem">Which item should be inputted. Leave out if only checking</param>
    /// <returns>True if inputted, False if full and did not input</returns>
    /// <exception cref="machine does not have an input on the side with inputDirection"></exception>
    /// <exception cref="inputNumber greater than InputArray.Length"></exception>
    /// <exception cref="inputNumber less than 0"></exception>
    public virtual bool Input(Direction inputDirection, ItemSO inputtedItem = null)
    {
        Orientation inputOrientation = new Orientation(inputDirection).Flip();
        // Calculating which internal side (based off of the rotation and inputDirection)
        inputDirection = inputOrientation.GetDirectionRotatedCounterclockwise(Rotation);
        
        // Exception
        if (IOArray[(int)inputDirection].IO.IOType != InputOrOutput.Input)
        {
            if (inputtedItem == null)
            {
                return false;
            } else
            {
                throw new Exception("Input: Machine does not have an input on the side with inputDirection:" + inputDirection);
            }
        }

        // Determines which Input is being inputted into
        int inputNumber = 0;
        for (; inputNumber < UsedInputDirectionList.Count; inputNumber++)
        {
            if (UsedInputDirectionList[inputNumber].FacingDirection == inputDirection)
            {
                break;
            }
        }

        // Exceptions which should never happen
        if (inputNumber >= InputArray.Length)
        {
            throw new Exception("inputNumber > InputArray.Length");
        }
        if (inputNumber < 0)
        {
            throw new Exception("inputNumber < 0");
        }

        // Returns false if the slot is already full
        if (InputArray[inputNumber] != null) 
        {
            Debug.Log(inputDirection + " is full");
            return false; 
        }

        // Returns true at this point if only checking
        if (inputtedItem == null) { return true; }

        // Sets the input to the right item
        InputArray[inputNumber] = inputtedItem;

        // Sets item displayer mesh and material
        InputItemDisplayerList[(int)inputDirection].GetComponent<MeshFilter>().mesh = inputtedItem.ItemMesh;
        Destroy(InputItemDisplayerList[(int)inputDirection].GetComponent<MeshRenderer>().material);
        InputItemDisplayerList[(int)inputDirection].GetComponent<MeshRenderer>().material = inputtedItem.ItemMaterial;
        return true;
    }

    /// <summary>
    /// Tries to output to all adjacent machines, voids any faliures if there is one success
    /// </summary>
    /// <returns>Success of outputting any items</returns>
    protected virtual bool Output()
    {
        // Checks if all outputs can output
        int countOfValidOutputs = 0;
        for (int i = 0; i < UsedOutputDirectionList.Count; i++)
        {
            Direction realDirection = UsedOutputDirectionList[i].GetDirectionRotatedClockwise(Rotation);
            if(GetAdjacentMachine(realDirection).Input(realDirection))
            {
                countOfValidOutputs++;
            }
        }

        // Outputs if possible or if VoidBlockedOutputs is true, then it will delete any outputs which cannot be outputted
        if ((VoidBlockedOutputs || countOfValidOutputs == UsedOutputDirectionList.Count))
        {
            for (int i = 0;i < UsedOutputDirectionList.Count;i++)
            {
                Direction realDirection = UsedOutputDirectionList[i].GetDirectionRotatedClockwise(Rotation);
                GetAdjacentMachine(realDirection).Input(realDirection, CalculateOutput(i));
                OutputItemDisplayerList[(int)UsedOutputDirectionList[i].FacingDirection].GetComponent<MeshFilter>().mesh = null;
            }
            CurrentlyDoingARecipe = false;
            ProcessingTimer = 0;
            
            DumpUsedInputs();
        }
        // Returns the success of outputting
        return !CurrentlyDoingARecipe;
    }

    /// <summary>
    /// Calculates the output of the recipe for the inputted output number
    /// </summary>
    /// <param name="outputNumber">Which output in the recipe to output</param>
    /// <returns>Item to output</returns>
    /// <exception cref="outputNumber greater than CurrentRecipe.OutputArray.Length"></exception>
    protected virtual ItemSO CalculateOutput(int outputNumber)
    {
        // Just in case
        if (outputNumber >= CurrentRecipe.OutputArray.Length)
        {
            throw new Exception("outputNumber > CurrentRecipe.OutputArray.Length");
        }
        return CurrentRecipe.OutputArray[outputNumber];
    }


    /// <summary>
    /// Clears the designated input
    /// </summary>
    /// <param name="inputNumber">Which input to clear</param>
    public void DumpInput(int inputNumber)
    {
        InputArray[inputNumber] = null;
        InputItemDisplayerList[(int)UsedInputDirectionList[inputNumber].FacingDirection].GetComponent<MeshFilter>().mesh = null;
    }
    /// <summary>
    /// Clears all used inputs 
    /// </summary>
    public virtual void DumpUsedInputs()
    {
        for (int i = 0; i < UsedInputDirectionList.Count; i++)
        {
            DumpInput(i);
        }

        HasSwappedItemTypes = false;
        CurrentlyDoingARecipe = false;
        for (int i = 0; i < OutputItemDisplayerList.Length; i++)
        {
            OutputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = null;

        }
        for (int i = 0; i < InputItemDisplayerList.Length; i++)
        {
            InputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = null;

        }
    }

    /// <summary>
    /// Returns the name of the machine
    /// </summary>
    /// <returns>The name of the machine</returns>
    public string GetMachineType()
    {
        return Type;
    }

    /// <summary>
    /// Returns the Input/Output and if it is locked
    /// </summary>
    /// <param name="side">Which direction the IOLock should be gotten from</param>
    /// <returns>The Input/Output and if it is locked on that side</returns>
    public IOLock GetIOLock(Direction side)
    {
        Direction adjustedSide = new Orientation(side).GetDirectionRotatedCounterclockwise(Rotation);
        return IOArray[(int)adjustedSide];
    }

    /// <summary>
    /// Sets the side's IOLock
    /// </summary>
    /// <param name="side">Which side to change</param>
    /// <param name="iOLock">What to change to</param>
    public virtual void SetSideIOLock(Direction side, IOLock iOLock)
    {
        Direction adjustedSide = new Orientation(side).GetDirectionRotatedCounterclockwise(Rotation);
        IOArray[(int)adjustedSide].IO = new InputOutput(iOLock.IO.IOType);
        IOArray[(int)adjustedSide].Locked = iOLock.Locked;
        SetCalculatedValues();
    }
    /// <summary>
    /// Sets the side's IOLock.IO
    /// </summary>
    /// <param name="side">Which side to change</param>
    /// <param name="inputOutput">What to change to</param>
    public virtual void SetSideIOLock(Direction side, InputOrOutput inputOrOutput)
    {
        Direction adjustedSide = new Orientation(side).GetDirectionRotatedCounterclockwise(Rotation);
        //IOArray[(int)adjustedSide].IO.SetIO(inputOrOutput);
        IOArray[(int)adjustedSide].IO = new InputOutput(inputOrOutput);
        SetCalculatedValues();
    }
    /// <summary>
    /// Sets the side's IOLock.Locked
    /// </summary>
    /// <param name="side">Which side to change</param>
    /// <param name="locked">What to change to</param>
    public virtual void SetSideIOLock(Direction side, bool locked)
    {
        Direction adjustedSide = new Orientation(side).GetDirectionRotatedCounterclockwise(Rotation);
        IOArray[(int)adjustedSide].Locked = locked;
    }
    /// <summary>
    /// Makes the little conveyor belts face the right direction or not exist based off of the Input/Output on that side
    /// </summary>
    /// <param name="side">Which side to fix</param>
    protected void FixDisplayedConveyorBelt(Direction side)
    {
        // Disables the renderer if the IO is None
        if (IOArray[(int)side].IO.IOType == InputOrOutput.None)
        {
            SideConveyors[(int)side].GetComponent<Renderer>().enabled = false;
        }
        else
        {
            // Enables the renderer if the IO is not None
            SideConveyors[(int)side].GetComponent<Renderer>().enabled = true;
            // Makes the little conveyor belts be in the correct direction (Output is default in PreFab so Input needs to be adjusted 180 degrees)
            if (IOArray[(int)side].IO.IOType == InputOrOutput.Input)
            {
                SideConveyors[(int)side].transform.localRotation = Quaternion.Euler(SideConveyors[(int)side].transform.localRotation.x, 180 + (90 * (int)side), SideConveyors[(int)side].transform.localRotation.z);
            }
            if (IOArray[(int)side].IO.IOType == InputOrOutput.Output)
            {
                SideConveyors[(int)side].transform.localRotation = Quaternion.Euler(SideConveyors[(int)side].transform.localRotation.x, 90 * (int)side, SideConveyors[(int)side].transform.localRotation.z);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 4; i++)
        {
            Orientation adjustedOrientation = new Orientation(i).RotateClockwise(Rotation);
            Machine adjacentMachine = GetAdjacentMachine(adjustedOrientation.FacingDirection);
            adjacentMachine.RecalculateSide(adjustedOrientation.GetOppositeDirection());
        }
    }

    // Methods for ConveyorBelt, but needs to be generically called
    public virtual void RecalculateSide(Direction side) { }
    public virtual void FigureOutAllSides() { }

}

