using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class Machine : MonoBehaviour
{
    [SerializeField] protected string Type;
    [SerializeField, Tooltip("0 = None, 1 = Input, 2 = Output \n Starting from the top going clockwise")] protected int[] IOArray = new int[4];
    protected int Rotation = 0;
    [SerializeField] protected Vector2Int Location;
    [SerializeField] protected ItemSO[] InputArray;
    protected int OutputCount;
    [SerializeField] protected RecipeListSO RecipeList;
    protected bool CurrentlyDoingARecipe = false;
    protected RecipeSO CurrentRecipe;
    protected float ProcessingTimer;
    protected float ProcessingCompletionTime;
    [SerializeField, Range(0f, 1f)] protected float SpeedFactor;
    [SerializeField] protected GameObject[] InputItemDisplayerList;
    protected Vector3[] InputItemDisplayerStartingPosition;
    [SerializeField] protected GameObject[] OutputItemDisplayerList;
    protected List<GameObject> UsedOutputItemDisplayerList = new();
    protected List<Vector3> OutputItemDisplayerStartingPosition = new();
    protected bool HasSwappedItemTypes = false;
    public ItemSO PressButtonDebugItem;

    public bool IsFull { get; protected set; }

    private void Start()
    {
        SetCalculatedValues();
    }

    public override string ToString()
    {
        return (Type + ": R = " + Rotation);
    }

    public void Rotate()
    {
        if (Rotation == 3)
        {
            Rotation = 0;
        }
        else
        {
            Rotation++;
        }

    }

    public void SetLocation(Vector2Int location) {
        Location = location;
    }

    protected virtual void Update()
    {
        if (!CurrentlyDoingARecipe)
        {
            CheckRecipe();
            ProcessingTimer = 0;
        } else if (ProcessingTimer >= ProcessingCompletionTime)
        {
            if (Output())
            {
                HasSwappedItemTypes = false;
                CurrentlyDoingARecipe = false;
                for (int i = 0; i < OutputItemDisplayerList.Length; i++)
                {
                    OutputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = null;

                }
            }
        } else
        {
            if (ProcessingTimer < ProcessingCompletionTime / 2)
            {
                for (int i = 0; i < InputItemDisplayerList.Length; i++)
                {
                    InputItemDisplayerList[i].transform.localPosition = Vector3.Lerp(InputItemDisplayerStartingPosition[i], Vector3.zero, ProcessingTimer / ProcessingCompletionTime * 2);
                }
            } else
            {
                if (!HasSwappedItemTypes)
                {
                    foreach (GameObject item in InputItemDisplayerList)
                    {
                        item.GetComponent<MeshFilter>().mesh = null;
                    }
                    for (int i = 0;i < UsedOutputItemDisplayerList.Count; i++)
                    {
                        UsedOutputItemDisplayerList[i].GetComponent<MeshFilter>().mesh = CalculateOutput(i).ItemMesh;
                        Destroy(UsedOutputItemDisplayerList[i].GetComponent<MeshRenderer>().material);
                        UsedOutputItemDisplayerList[i].GetComponent<MeshRenderer>().material = CalculateOutput(i).ItemMaterial;
                    }
                    HasSwappedItemTypes = true;
                    for (int i = 0; i < InputItemDisplayerList.Length; i++)
                    {
                        InputItemDisplayerList[i].transform.localPosition = InputItemDisplayerStartingPosition[i];
                    }
                } 
                for (int i = 0; i < UsedOutputItemDisplayerList.Count; i++)
                {
                    UsedOutputItemDisplayerList[i].transform.localPosition = Vector3.Lerp(Vector3.zero, OutputItemDisplayerStartingPosition[i], (ProcessingTimer - 0.5f) / ProcessingCompletionTime * 2);
                }
                
            }

        }
        ProcessingTimer += Time.deltaTime;
    }

    protected virtual void CheckRecipe()
    {
        bool foundRecipe = false;
        int tempIndex = -1;
        do
        {
            tempIndex++;
            if (tempIndex == RecipeList.RecipeList.Length) { break; }
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
        if (tempIndex < RecipeList.RecipeList.Length)
        {
            CurrentlyDoingARecipe = true;
            CurrentRecipe = RecipeList.RecipeList[tempIndex];
            ProcessingCompletionTime = CurrentRecipe.BaseTimeToComplete * SpeedFactor;
        }
    }

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

    public void SetCalculatedValues()
    {
        OutputCount = 0;
        UsedOutputItemDisplayerList.Clear();
        OutputItemDisplayerStartingPosition.Clear();
        for (int i = 0; i < IOArray.Length; i++)
        {
            if (IOArray[i] == 2)
            {
                OutputCount++;
                UsedOutputItemDisplayerList.Add(OutputItemDisplayerList[i]);
                OutputItemDisplayerStartingPosition.Add(UsedOutputItemDisplayerList[i].transform.localPosition);
            }
        }

        int inputCount = 0;
        foreach (int value in IOArray)
        {
            if (value == 1) { inputCount++; }
        }
        InputArray = new ItemSO[inputCount];
        InputItemDisplayerStartingPosition = new Vector3[InputItemDisplayerList.Length];
        for (int i = 0; i < InputItemDisplayerList.Length; i++)
        {
            InputItemDisplayerStartingPosition[i] = InputItemDisplayerList[i].transform.localPosition;
        }
    }

    public virtual bool Input(int inputDirection, ItemSO inputtedItem)
    {
        int inputSide;
        if (inputDirection - Rotation < 0)
        {
            inputSide = inputDirection - Rotation + 4;
        } else
        {
            inputSide = inputDirection - Rotation;
        }

        if (IOArray[inputSide] != 1) { throw new Exception("Machine does not have an input on the side with inputDirection = " + inputDirection); }

        int inputNumber = -1;
        for (int i = 0; i <= inputSide; i++)
        {
            if (IOArray[i] == 1) { inputNumber++; }
        }

        if (inputNumber >= InputArray.Length)
        {
            throw new Exception("inputNumber > InputArray.Length");
        }

        if (inputNumber < 0)
        {
            throw new Exception("inputNumber < 0");
        }

        if (InputArray[inputNumber] != null) { return false; }

        InputArray[inputNumber] = inputtedItem;

        InputItemDisplayerList[inputDirection].GetComponent<MeshFilter>().mesh = inputtedItem.ItemMesh;
        Destroy(InputItemDisplayerList[inputDirection].GetComponent<MeshRenderer>().material);
        InputItemDisplayerList[inputDirection].GetComponent<MeshRenderer>().material = inputtedItem.ItemMaterial;
        return true;
    }
    protected virtual bool Output()
    {
        for (int currentOutputNumber = 0; currentOutputNumber < OutputCount; currentOutputNumber++) {
            int outputDirection = -1;
            int outputCounter = -1;
            for (int i = 0; outputCounter < currentOutputNumber; i++, outputDirection++)
            {
                if (i >= 4) { throw new Exception("There are fewer outputs than the currentOutputNumber"); }
                if (IOArray[i] == 2) { outputCounter++; }

            }

            outputDirection = (outputDirection + Rotation) % 4;

            if (outputDirection >= 4)
            {
                throw new Exception("Outputting to nonexistent direction");
            }

            if (GetAdjacentMachine(outputDirection).Input((outputDirection + 2) % 4, CalculateOutput(currentOutputNumber)))
            {
                CurrentlyDoingARecipe = false;
                ProcessingTimer = 0;
                DumpInputs();
            }

            if (currentOutputNumber == OutputCount - 1)
            {
                currentOutputNumber = 0;
            } else
            {
                currentOutputNumber++;
            }
        }
        return !CurrentlyDoingARecipe;
    }

    protected virtual ItemSO CalculateOutput(int outputNumber)
    {
        if (outputNumber >= CurrentRecipe.OutputArray.Length)
        {
            throw new Exception("outputNumber > CurrentRecipe.OutputArray.Length");
        }
        return CurrentRecipe.OutputArray[outputNumber];
    }

    public void DumpInputs()
    {
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i] = null;
        }
    }

    public string GetMachineType()
    {
        return Type;
    }
}

