using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FactoryGrid : MonoBehaviour
{
    public static FactoryGrid Instance { get; private set; }
    
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
    public Machine[,] _factoryGrid;
    [SerializeField] private int _baseSize;
    private Machine _blockerMachine;
    

    // Start is called before the first frame update
    void Start()
    {
        _factoryGrid = new Machine[_baseSize, _baseSize];
        _blockerMachine = GetComponent<BlockerMachine>();
    }

    public ref Machine GetMachine (int row, int column)
    {
        return ref _factoryGrid[row, column];
    }

    public ref Machine GetMachine (Vector2Int coordinates)
    {
        if (coordinates.x >= _baseSize || coordinates.y >= _baseSize || coordinates.x < 0 || coordinates.y < 0 || _factoryGrid[coordinates.x, coordinates.y] == null) { return ref _blockerMachine; }
        return ref _factoryGrid[coordinates.x, coordinates.y];
    }

    public int GetSize()
    {
        return _baseSize;
    }

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
            return true;
        } else
        {
            return false;
        }

    }

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

    public bool RemoveMachine(int x, int y)
    {
        if (_factoryGrid[x, y] != null)
        {
            _factoryGrid[x, y] = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PrintOutTheGrid()
    {
        for (int column = _baseSize -1; column >= 0; column--)
        {
            string printOut = "|";
            for (int row = 0; row < _baseSize - 1; row++)
            {
                printOut += _factoryGrid[row, column]?.ToString() + ", ";
            }
            printOut += _factoryGrid[column, _baseSize - 1];
            printOut += "|";
            Debug.Log(printOut);
        }
    }

}
