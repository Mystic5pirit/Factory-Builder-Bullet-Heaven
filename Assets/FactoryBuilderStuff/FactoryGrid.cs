using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FactoryGrid : MonoBehaviour
{
    /// <summary>
    /// The instance of FactoryGrid, making it a singleton
    /// </summary>
    public static FactoryGrid Instance { get; private set; }
    
    // Ensures that there is one and only one instance of FactoryGrid
    private void Awake()
    {
        if (_factoryGrid != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// The array of all of the machines in the factory
    /// </summary>
    public Machine[,] _factoryGrid;
    /// <summary>
    /// How big the side length of the factory should be
    /// </summary>
    [SerializeField] private int _baseSize;
    /// <summary>
    /// Instance of a BlockerMachine
    /// </summary>
    private Machine _blockerMachine;



    // Initializes _factoryGrid on startup 
    void Start()
    {
        _factoryGrid = new Machine[_baseSize, _baseSize];
        _blockerMachine = GetComponent<BlockerMachine>();
    }

    /// <summary>
    /// Returns a reference to the targeted tile's machine <br/>
    /// Returns a reference to a BlockerMachine if the targeted tile is empty or outside of the grid
    /// </summary>
    /// <param name="row">Targeted row</param>
    /// <param name="column">Targeted column</param>
    /// <returns>A reference to the targeted tile or an instance of a BlockerMachine</returns>
    public ref Machine GetMachine (int row, int column)
    {
        if (row >= _baseSize || column >= _baseSize || row < 0 || column < 0 || _factoryGrid[row, column] == null) { return ref _blockerMachine; }
        return ref _factoryGrid[row, column];
    }
    /// <summary>
    /// Returns a reference to the targeted tile's machine <br/>
    /// Returns a reference to a BlockerMachine if the targeted tile is empty or outside of the grid
    /// </summary>
    /// <param name="position">Vector2 of the coordinates of the targeted tile</param>
    /// <returns>A reference to the targeted tile or an instance of a BlockerMachine</returns>
    public ref Machine GetMachine (Vector2Int position)
    {
        if (position.x >= _baseSize || position.y >= _baseSize || position.x < 0 || position.y < 0 || _factoryGrid[position.x, position.y] == null) { return ref _blockerMachine; }
        return ref _factoryGrid[position.x, position.y];
    }

    /// <summary>
    /// Returns whether or not there is a machine at the targeted tile
    /// </summary>
    /// <param name="position">Vector2 of the coordinates of the targeted tile</param>
    /// <returns>A bool of if there is a machine at the targeted tile</returns>
    /// <exception cref="OutOfBoundsException"></exception>
    public bool IsThereAMachineThere (Vector2Int position)
    {
        if (position.x >= _baseSize || position.y >= _baseSize || position.x < 0 || position.y < 0) { throw new Exception("OutOfBoundsException"); }
        if (_factoryGrid[position.x, position.y] == null) { return false; }
        return true;
    }

    /// <summary>
    /// Returns the size of the factory grid
    /// </summary>
    /// <returns>The size of the factory grid</returns>
    public int GetSize()
    {
        return _baseSize;
    }

    /// <summary>
    /// Adds the inputted Machine into the factory grid at the inputted position with the inputted rotation
    /// </summary>
    /// <param name="row">Targeted row</param>
    /// <param name="column">Targeted column</param>
    /// <param name="rotation">Rotation of the machine</param>
    /// <param name="newMachine">Machine to be added</param>
    /// <returns>Whether or not it placed (false if there is something there already)</returns>
    public bool PlaceMachine (int row, int column, int rotation, Machine newMachine)
    {
        if (_factoryGrid[row, column] == null)
        {
            _factoryGrid[row, column] = newMachine;
            for (int i = 0; i < rotation; i++)
            {
                _factoryGrid[row, column].Rotate();
            }
            _factoryGrid[row, column].SetLocation(new Vector2Int(row, column));
            _factoryGrid[row, column].FigureOutAllSides();
            return true;
        } else
        {
            return false;
        }

    }

    /// <summary>
    /// Adds the inputted Machine into the factory grid at the inputted position with the inputted rotation
    /// </summary>

    /// <param name="position">Targeted position</param>
    /// <param name="rotation">Rotation of the machine</param>
    /// <param name="newMachine">Machine to be added</param>
    /// <returns>Whether or not it placed (false if there is something there already)</returns>
    public bool PlaceMachine(Vector2Int position, int rotation, Machine newMachine)
    {
        if (_factoryGrid[position.x, position.y] == null)
        {
            _factoryGrid[position.x, position.y] = newMachine;
            for (int i = new(); i < rotation; i++)
            {
                _factoryGrid[position.x, position.y].Rotate();
            }
            _factoryGrid[position.x, position.y].SetLocation(new Vector2Int(position.x, position.y));
            _factoryGrid[position.x, position.y].FigureOutAllSides();
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Removes the machine at targeted position
    /// </summary>
    /// <param name="position">Position of machine to be removed</param>
    /// <returns>Success of removal (false if nothing is there to be removed)</returns>
    public bool RemoveMachine(Vector2Int position)
    {
        if (_factoryGrid[position.x, position.y] != null)
        {
            _factoryGrid[position.x, position .y] = null;
            return true;
        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the machine at targeted position
    /// </summary>
    /// <param name="row">targeted row</param>
    /// <param name="column">targeted column</param>
    /// <returns>Success of removal (false if nothing is there to be removed)</returns>
    public bool RemoveMachine(int row, int column)
    {
        if (_factoryGrid[row, column] != null)
        {
            _factoryGrid[row, column] = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Prints out the grid with Debug.Log() <br/>
    /// Starts with top left to bottom right
    /// </summary>
    public void PrintOutTheGrid()
    {
        /*for (int column = _baseSize -1; column >= 0; column--)
        {
            string printOut = "|";
            for (int row = 0; row < _baseSize - 1; row++)
            {
                printOut += _factoryGrid[row, column]?.ToString() + ", ";
            }
            printOut += _factoryGrid[column, _baseSize - 1];
            printOut += "|";
            Debug.Log(printOut);
        }*/

        for (int column = _baseSize - 1; column >= 0; column--)
        {
            string printOut = "| ";
            for (int row = 0; row < _baseSize - 1; row++)
            {
                string machineName = GetMachine(row, column).name;
                if (machineName != "BlockerMachine" && machineName != "FactoryGridSingleton")
                {
                    printOut += machineName;
                }
                printOut += ", ";
            }
            printOut += _factoryGrid[column, _baseSize - 1];
            printOut += "|";
            Debug.Log(printOut);
        }
    }

}
