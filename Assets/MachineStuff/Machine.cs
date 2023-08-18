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
    [SerializeField, Tooltip("Element 0 is North, Element 1 is East, etc.")] protected InputOutput[] IOArray = new InputOutput[4];
    /// <summary>
    /// List of recipes
    /// </summary>
    [SerializeField] protected RecipeListSO RecipeList;
    /// <summary>
    /// How fast the machine goes (multiplier)
    /// </summary>
    [SerializeField, Range(0f, 1f)] protected float SpeedFactor;
    /// <summary>
    /// The GameObjects of the item displays
    /// </summary>
    [SerializeField] protected GameObject[] InputItemDisplayerList, OutputItemDisplayerList;
    protected Vector3[] InputItemDisplayerStartingPosition;
    protected List<GameObject> UsedOutputItemDisplayerList = new();
    protected List<Vector3> UsedOutputItemDisplayerStartingPosition = new();
    /// <summary>
    /// Center of movement for the item displays to converge to
    /// </summary>
    [SerializeField] private GameObject _centerOfMovement;

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
    protected ItemSO[] InputArray;
    protected bool CurrentlyDoingARecipe = false;//can be removed
    protected bool HasSwappedItemTypes = false;

    // RECIPE VARIABLES
    protected RecipeSO CurrentRecipe;
    protected float ProcessingTimer;
    protected float ProcessingCompletionTime;

    // Used for debugging
    public ItemSO PressButtonDebugItem;

    /// <summary>
    /// Should the machine do anything on Update
    /// </summary>
    [SerializeField] protected bool IsActive = true;


    private void Start()
    {
        SetCalculatedValues();
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
    public void Rotate()
    {
        Rotation = (Rotation + 1) % 4;
        gameObject.transform.Rotate(0, 90, 0);
    }

    /// <summary>
    /// Sets the internal stored location
    /// </summary>
    /// <param name="location"></param>
    public void SetLocation(Vector2Int location) {
        Location = location;
    }

    protected virtual void Update()
    {
        if (IsActive)
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
                    for (int i = 0; i < InputItemDisplayerList.Length; i++)
                    {
                        InputItemDisplayerList[i].transform.localPosition = Vector3.Lerp(InputItemDisplayerStartingPosition[i], _centerOfMovement.transform.localPosition, ProcessingTimer / ProcessingCompletionTime * 2);
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
                        for (int i = 0;i < UsedOutputItemDisplayerList.Count; i++)
                        {
                            UsedOutputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = CalculateOutput(i).ItemMesh;
                            Destroy(UsedOutputItemDisplayerList[i].GetComponent<MeshRenderer>().material); // Removes the previous material to not create copies that do not have any references
                            UsedOutputItemDisplayerList[i].GetComponent<MeshRenderer>().material = CalculateOutput(i).ItemMaterial;
                        }
                        // Makes it only do this swap once per recipe opperation
                        HasSwappedItemTypes = true;
                        // Resets input display positions
                        for (int i = 0; i < InputItemDisplayerList.Length; i++)
                        {
                            InputItemDisplayerList[i].transform.localPosition = InputItemDisplayerStartingPosition[i];
                        }
                    } 
                    // Moves the output displays outward from _centerOfMovement.transform.position
                    for (int i = 0; i < UsedOutputItemDisplayerList.Count; i++)
                    {
                        UsedOutputItemDisplayerList[i].transform.localPosition = Vector3.Lerp(_centerOfMovement.transform.localPosition, UsedOutputItemDisplayerStartingPosition[i], (ProcessingTimer - 0.5f) / ProcessingCompletionTime * 2);
                    }
                
                }

            }
            // Increases the timer
            ProcessingTimer += Time.deltaTime;
        }
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
    protected ref Machine GetAdjacentMachine(int direction)
    {
        switch(direction)
        {
            case 0:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.up);
            case 1:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.right);
            case 2:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.down);
            case 3:
                return ref FactoryGrid.Instance.GetMachine(Location + Vector2Int.left);
            default: 
                throw new Exception("Inputted number is too high");
        }
    }

    /// <summary>
    /// Sets any values that need to be pre-calculated
    /// </summary>
    public void SetCalculatedValues()
    {
        // Adds used output displays to the list of used output displays
        UsedOutputItemDisplayerList.Clear();
        UsedOutputItemDisplayerStartingPosition.Clear();
        for (int i = 0; i < IOArray.Length; i++)
        {
            if (IOArray[i].IOType == InputOrOutput.Output)
            {
                UsedOutputItemDisplayerList.Add(OutputItemDisplayerList[i]);
                UsedOutputItemDisplayerStartingPosition.Add(UsedOutputItemDisplayerList[UsedOutputItemDisplayerList.Count - 1].transform.localPosition);
            }
        }

        // Calculates input arrays
        int inputCount = 0;
        foreach (InputOutput inputOutput in IOArray)
        {
            if (inputOutput.IOType == InputOrOutput.Input) { inputCount++; }
        }
        InputArray = new ItemSO[inputCount];

        // Calculates input display position values
        InputItemDisplayerStartingPosition = new Vector3[InputItemDisplayerList.Length];
        for (int i = 0; i < InputItemDisplayerList.Length; i++)
        {
            InputItemDisplayerStartingPosition[i] = InputItemDisplayerList[i].transform.localPosition;
        }
    }

    /// <summary>
    /// Inputs into this machine
    /// </summary>
    /// <param name="inputDirection">Which direction the input side is on</param>
    /// <param name="inputtedItem">Which item should be inputted</param>
    /// <returns>True if inputted, False if full and did not input</returns>
    /// <exception cref="machine does not have an input on the side with inputDirection"></exception>
    /// <exception cref="inputNumber greater than InputArray.Length"></exception>
    /// <exception cref="inputNumber less than 0"></exception>
    public virtual bool Input(int inputDirection, ItemSO inputtedItem)
    {
        // Calculating which internal side (based off of the rotation and inputDirection)
        int inputSide;
        if (inputDirection - Rotation < 0)
        {
            inputSide = inputDirection - Rotation + 4;
        } else
        {
            inputSide = inputDirection - Rotation;
        }

        // Exception
        if (IOArray[inputSide].IOType != InputOrOutput.Input) { throw new Exception("Machine does not have an input on the side with inputDirection" + inputDirection); }

        // Transverse IOArray for current input
        int inputNumber = -1;
        for (int i = 0; i <= inputSide; i++)
        {
            if (IOArray[i].IOType == InputOrOutput.Input) { inputNumber++; }
        }

        // Exceptions
        if (inputNumber >= InputArray.Length)
        {
            throw new Exception("inputNumber > InputArray.Length");
        }
        if (inputNumber < 0)
        {
            throw new Exception("inputNumber < 0");
        }

        // Returns false if the slot is already full
        if (InputArray[inputNumber] != null) { return false; }

        // Sets the input to the right item
        InputArray[inputNumber] = inputtedItem;

        // Sets item displayer mesh and material
        InputItemDisplayerList[inputDirection].GetComponent<MeshFilter>().mesh = inputtedItem.ItemMesh;
        Destroy(InputItemDisplayerList[inputDirection].GetComponent<MeshRenderer>().material);
        InputItemDisplayerList[inputDirection].GetComponent<MeshRenderer>().material = inputtedItem.ItemMaterial;
        return true;
    }

    /// <summary>
    /// Tries to output to all adjacent machines, voids any faliures if there is one success
    /// </summary>
    /// <returns>Success of outputting any items</returns>
    /// <exception cref="There are fewer outputs than the currentOutputNumber"></exception>
    /// <exception cref="Outputting to nonexistent direction"></exception>

    protected virtual bool Output()
    {
        // Goes through each output
        for (int currentOutputNumber = 0; currentOutputNumber < UsedOutputItemDisplayerList.Count; currentOutputNumber++) {
            // Determines which side the output is on (ignoring rotation)
            int outputDirection = -1;
            int outputCounter = -1;
            for (int i = 0; outputCounter < currentOutputNumber; i++, outputDirection++)
            {
                if (i >= 4) { throw new Exception("There are fewer outputs than the currentOutputNumber"); }
                if (IOArray[i].IOType == InputOrOutput.Output) { outputCounter++; }

            }

            // Adjusts outputDirection based off of rotation
            outputDirection = (outputDirection + Rotation) % 4;

            // Should never happen, but just in case
            if (outputDirection >= 4)
            {
                throw new Exception("Outputting to nonexistent direction");
            }

            // DOES NOT WORK
            // Gets the adjacent machine then tries to input into it
            // North inputs into South side of the machine, East into West, etc.
            // On success it prepares for next recipe
            if (GetAdjacentMachine(outputDirection).Input((outputDirection + 2) % 4, CalculateOutput(currentOutputNumber)))
            {
                CurrentlyDoingARecipe = false;
                ProcessingTimer = 0;
                DumpInputs();
            }

            // Loop around back to first output ... for some reason...
            if (currentOutputNumber == UsedOutputItemDisplayerList.Count - 1)
            {
                currentOutputNumber = 0;
            } else
            {
                currentOutputNumber++;
            }
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
    /// Clears the inputs
    /// </summary>
    public void DumpInputs()
    {
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i] = null;
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
}

