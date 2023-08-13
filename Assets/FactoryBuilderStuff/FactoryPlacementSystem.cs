using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FactoryPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject _mouseIndicator, _tileSelector, _originSelector;
    [SerializeField] private FactoryInputManager _factoryInputManager;
    [SerializeField] private Vector2Int _selectedTile;
    private Vector2Int _lastAddedTile = new Vector2Int(-1, -1);
    [SerializeField] public GameObject[] MachineList;
    private int _machineListIndex = 0;
    private int _placeRotation = 0;
    private List<Vector2Int> _listOfMachinesToBePlaced = new();
    private List<GameObject> _listOfMachinesToBePlacedPreview = new();
    private int _placementMode = 0;
    private bool _isPlacing = false;
    private Vector2Int _placementOrigin;
    private GameObject _tempOriginSelector;
    [SerializeField] private GameObject _removalBox;
    private GameObject _rectangleRemovalBox;


    private void Update()
    {
        Vector3 mousePosition = _factoryInputManager.GetSelectedMapPosition();
        _mouseIndicator.transform.position = mousePosition;
        _selectedTile = _factoryInputManager.GetSelectedMapTile();
        Vector3 tilePosition = new Vector3(_selectedTile.x * 10 + 5, 0.1f, _selectedTile.y * 10 + 5);
        _tileSelector.transform.position = tilePosition;

        if (Input.GetKeyDown(KeyCode.Alpha1) && !_isPlacing)
        {
            if (_machineListIndex == MachineList.Length - 1)
            {
                _machineListIndex = 0;
            } else
            {
                _machineListIndex++;
            }
            Debug.Log("Placing " + MachineList[_machineListIndex].GetComponent<Machine>().GetMachineType());
        }

        if (Input.GetKeyDown(KeyCode.R) && !_isPlacing)
        {
            if (_placeRotation == 3)
            {
                _placeRotation = 0;
            }
            else
            {
                _placeRotation++;
            }
            Debug.Log(_placeRotation);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isPlacing)
        {
            _placementMode = (_placementMode + 1) % 4;
            switch(_placementMode)
            {
                case 0:
                    Debug.Log("Drag Mode");
                    break;
                case 1:
                    Debug.Log("Line Mode");
                    break;
                case 2:
                    Debug.Log("Drag Remove Mode");
                    break;
                case 3:
                    Debug.Log("Rectangle Remove Mode");
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && !_isPlacing)
        {
            FactoryGrid.Instance.PrintOutTheGrid();
        }

        if (Input.GetMouseButtonDown(0))
        {
            _listOfMachinesToBePlaced.Clear();
            _listOfMachinesToBePlacedPreview.Clear();
            _isPlacing = true;
            if (_placementMode == 1)
            {
                _placementOrigin = _selectedTile;
                _tempOriginSelector = Instantiate(_originSelector, new Vector3(_tileSelector.transform.position.x, _tileSelector.transform.position.y, _tileSelector.transform.position.z), Quaternion.identity);
            }
            if (_placementMode == 3)
            {
                _placementOrigin = _selectedTile;
                _tempOriginSelector = Instantiate(_originSelector, new Vector3(_tileSelector.transform.position.x, _tileSelector.transform.position.y, _tileSelector.transform.position.z), Quaternion.identity);
                _rectangleRemovalBox = Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, _placeRotation * 90, 0));
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (_lastAddedTile != _selectedTile)
            {
                
                if (_placementMode == 0 && !_listOfMachinesToBePlaced.Contains(_selectedTile))
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(MachineList[_machineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, _placeRotation * 90, 0)));
                }
                if (_placementMode == 2 && !_listOfMachinesToBePlaced.Contains(_selectedTile))
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, _placeRotation * 90, 0)));
                }
                if (_placementMode == 3)
                {
                    _rectangleRemovalBox.transform.position = new Vector3(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 + 5, 0.1f + 5, ((float)_placementOrigin.y + (float)_selectedTile.y) / 2 * 10 + 5);
                    Debug.Log((float)_placementOrigin.x);
                    Debug.Log((float)_placementOrigin.x + (float)_selectedTile.x);
                    Debug.Log(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 );
                    Debug.Log(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 );
                    Debug.Log(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 + 5);
                    Debug.Log(_rectangleRemovalBox.transform.position);
                    _rectangleRemovalBox.transform.localScale = new Vector3(((_placementOrigin.x + 1) - (_selectedTile.x + 1) + Math.Sign(_placementOrigin.x - _selectedTile.x)) * 10, 10, ((_placementOrigin.y + 1) - (_selectedTile.y + 1) + Math.Sign(_placementOrigin.y - _selectedTile.y)) * 10);
                    if (_rectangleRemovalBox.transform.localScale.x == 0 )
                    {
                        _rectangleRemovalBox.transform.localScale = new Vector3(10, _rectangleRemovalBox.transform.localScale.y, _rectangleRemovalBox.transform.localScale.z);
                    }
                    if (_rectangleRemovalBox.transform.localScale.z == 0)
                    {
                        _rectangleRemovalBox.transform.localScale = new Vector3(_rectangleRemovalBox.transform.localScale.x, _rectangleRemovalBox.transform.localScale.y, 10);
                    }
                }
                _lastAddedTile = _selectedTile;
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_placementMode == 1)
            {
                Vector2Int mouseOffset = _selectedTile - _placementOrigin;
                Debug.Log((int)((Vector2.SignedAngle(mouseOffset, Vector2.right) + 225) / 90));
                Vector2Int direction = Vector2Int.zero;
                int placementNumber = 0;
                switch ((int)((Vector2.SignedAngle(mouseOffset, Vector2.right) + 225) / 90))
                {
                    case 1:
                        direction = Vector2Int.up;
                        placementNumber = mouseOffset.y + 1;
                        break;
                    case 2:
                        direction = Vector2Int.right;
                        placementNumber = mouseOffset.x + 1;
                        break;
                    case 3:
                        direction = Vector2Int.down;
                        placementNumber = Math.Abs(mouseOffset.y) + 1;
                        break;
                    case 4:
                    case 0:
                        direction = Vector2Int.left;
                        placementNumber = Math.Abs(mouseOffset.x) + 1;
                        break;
                }
                Debug.Log(direction);
                Vector2Int currentTile = _placementOrigin;
                _listOfMachinesToBePlaced.Clear();
                for (int i = 0; i < placementNumber; i++)
                {
                    _listOfMachinesToBePlaced.Add(currentTile);
                    currentTile += direction;
                }
            }
            if (_placementMode == 3)
            {
                Vector2Int mouseOffset = _selectedTile - _placementOrigin;                
                _listOfMachinesToBePlaced.Clear();
                for (int i = 0; i != mouseOffset.x + Math.Sign(mouseOffset.x); i += Math.Sign(mouseOffset.x))
                {
                    for (int j = 0; j != mouseOffset.y + Math.Sign(mouseOffset.y); j += Math.Sign(mouseOffset.y))
                    {
                        _listOfMachinesToBePlaced.Add(_placementOrigin + new Vector2Int(i,j));
                    }
                }
                Destroy(_rectangleRemovalBox);
            }

            if (_placementMode < 2)
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    PlaceMachine(selectedTile, _placeRotation, MachineList[_machineListIndex]);
                }
            } else
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    RemoveMachine(selectedTile);
                }
            }
            Destroy(_tempOriginSelector);
            _isPlacing = false;
            while (_listOfMachinesToBePlacedPreview.Count > 0)
            {
                Destroy(_listOfMachinesToBePlacedPreview[0]);
                _listOfMachinesToBePlacedPreview.RemoveAt(0);
            }
        }
    }

    private void PlaceMachine(Vector2Int selectedTile, int placeRotation, GameObject machineType)
    {
        if (FactoryGrid.Instance.GetMachine(selectedTile) == null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() ==  "BlockerMachine")
        {
            GameObject lastPlaced = Instantiate(machineType, new Vector3(selectedTile.x * 10 + 5, 0.1f + 2.5f, selectedTile.y * 10 + 5), Quaternion.Euler(0, placeRotation * 90, 0));
            FactoryGrid.Instance.PlaceMachine(selectedTile.x, selectedTile.y, placeRotation, lastPlaced.GetComponent<Machine>());
        }
    }

    private void RemoveMachine(Vector2Int selectedTile)
    {
        if (FactoryGrid.Instance.GetMachine(selectedTile) != null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() != "BlockerMachine")
        {
            Destroy(FactoryGrid.Instance.GetMachine(selectedTile).gameObject);
            FactoryGrid.Instance.RemoveMachine(selectedTile);
        }
    }
}


