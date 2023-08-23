using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using PlacementModeTypes = PlacementMode.PlacementModeTypes;


public class FactoryPlacementSystem : MonoBehaviour
{
    /// <summary>
    /// Indicator for selected tile
    /// </summary>
    [SerializeField] private GameObject _tileSelector;
    [SerializeField] private FactoryInputManager _factoryInputManager;
    /// <summary>
    /// Coordinates of the selected tile
    /// </summary>
    [SerializeField] private Vector2Int _selectedTile;
    /// <summary>
    /// Previously selected tile <br/>
    /// (Used to prevent checking whether or not to add the same tile over and over)
    /// </summary>
    private Vector2Int _lastAddedTile = new Vector2Int(-1, -1);
    /// <summary>
    /// List of locations of where to add machines
    /// </summary>
    private List<Vector2Int> _listOfMachinesToBePlaced = new();
    /// <summary>
    /// List of preview machines and preview boxes
    /// </summary>
    private List<GameObject> _listOfMachinesToBePlacedPreview = new();
    /// <summary>
    /// Where dragging started
    /// </summary>
    private Vector2Int _placementOrigin;
    /// <summary>
    /// Box used to highlight selected tiles
    /// </summary>
    [SerializeField] private GameObject _removalBox, _previewBox;
    /// <summary>
    /// Box used to highlight selected tiles
    /// </summary>
    private GameObject _rectangleRemovalBox, _linePreviewBox;


    private void Update()
    {
        //
        PlacementModeTypes placementMode = PlacementSettings.Instance.PlacementMode.SelectedPlacementMode;

        // Puts the _tileSelector in the correct spot
        _selectedTile = _factoryInputManager.GetSelectedMapTile();
        Vector3 tilePosition = new(_selectedTile.x * 10 + 5, 0.1f, _selectedTile.y * 10 + 5);
        _tileSelector.transform.position = tilePosition;

        // When mouse is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Clears lists just in case
            _listOfMachinesToBePlaced.Clear();
            _listOfMachinesToBePlacedPreview.Clear();
            PlacementSettings.Instance.CurrentlyPlacing = true;
            // Sets origin and creates selection boxes for placement modes which require it 
            if (placementMode == PlacementMode.PlacementModeTypes.Line)
            {
                _placementOrigin = _selectedTile;
                //_linePreviewBox = Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, (int)PlacementSettings.Instance.PlaceRotation.FacingDirection * 90, 0));

            }
            if (placementMode == PlacementModeTypes.RectangleRemove)
            {
                _placementOrigin = _selectedTile;
                _rectangleRemovalBox = Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0));
            }
        }

        // Every frame that the mouse is pressed
        if (Input.GetMouseButton(0))
        {
            // Ensures that it doesn't try adding every tick
            if (_lastAddedTile != _selectedTile)
            {
                // Drag mode - Adds each tile went over to the list, does not add if it is already on the list, Previews machines and boxes also
                if (placementMode == PlacementModeTypes.Drag && !_listOfMachinesToBePlaced.Contains(_selectedTile))
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                }
                // Line mode - Previews machines and boxes in a cardinal direction towards the _selectedTile
                if (placementMode == PlacementModeTypes.Line)
                {
                    // Clears the previews
                    while (_listOfMachinesToBePlacedPreview.Count > 0)
                    {
                        Destroy(_listOfMachinesToBePlacedPreview[0]);
                        _listOfMachinesToBePlacedPreview.RemoveAt(0);
                    }
                    // Calculates direction and amount
                    Vector2Int mouseOffset = _selectedTile - _placementOrigin;
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

                    // Places the previews
                    Vector2Int currentTile = _placementOrigin;
                    for (int i = 0; i < placementNumber; i++)
                    {
                        _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(currentTile.x * 10 + 5, 0.1f + 2.5f, currentTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                        _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(currentTile.x * 10 + 5, 0.1f + 2.5f, currentTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                        currentTile += direction;
                    }
                }
                // DragRemove mode - Adds each tile went over to the list, does not add if it is already on the list, Previews boxes also
                if (placementMode == PlacementModeTypes.DragRemove && !_listOfMachinesToBePlaced.Contains(_selectedTile))
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                }
                // RectangleRemove mode - Previews a box of all machines to remove
                if (placementMode == PlacementModeTypes.RectangleRemove)
                {
                    _rectangleRemovalBox.transform.position = new Vector3(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 + 5, 0.1f + 5, ((float)_placementOrigin.y + (float)_selectedTile.y) / 2 * 10 + 5);
                    _rectangleRemovalBox.transform.localScale = new Vector3(((_placementOrigin.x + 1) - (_selectedTile.x + 1) + Math.Sign(_placementOrigin.x - _selectedTile.x)) * 10, 10, ((_placementOrigin.y + 1) - (_selectedTile.y + 1) + Math.Sign(_placementOrigin.y - _selectedTile.y)) * 10);
                    // Ensures the scale is not zero when only a line or single tile
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

        // When Mouse is released
        if (Input.GetMouseButtonUp(0))
        {

            // Line mode - Adds each tile in the line to the list
            if (placementMode == PlacementModeTypes.Line)
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
            // Adds all of the tiles in the rectangle to the list
            if (placementMode == PlacementModeTypes.RectangleRemove)
            {
                Vector2Int mouseOffset = _selectedTile - _placementOrigin;                
                _listOfMachinesToBePlaced.Clear();
                // do/while instead of for loop so it does it once if it is just a line or single tile
                int i = 0, j = 0;
                do
                {
                    do
                    {
                        _listOfMachinesToBePlaced.Add(_placementOrigin + new Vector2Int(i, j));
                        j += Math.Sign(mouseOffset.y);
                    } while (j != mouseOffset.y + Math.Sign(mouseOffset.y));
                    i += Math.Sign(mouseOffset.x);
                } while (i != mouseOffset.x + Math.Sign(mouseOffset.x));
                Destroy(_rectangleRemovalBox);
            }

            // Placing for placement modes which are for placing
            if (placementMode == PlacementModeTypes.Drag || placementMode == PlacementModeTypes.Line)
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    PlaceMachine(selectedTile, PlacementSettings.Instance.PlaceRotation, PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex]);
                }
            }
            // Removal for placement modes which are for removing
            else if (placementMode == PlacementModeTypes.DragRemove || placementMode == PlacementModeTypes.RectangleRemove)
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    RemoveMachine(selectedTile);
                }
            } else if (placementMode == PlacementModeTypes.Rotate)
            {
                FactoryGrid.Instance.GetMachine(_selectedTile).Rotate();
            }

            PlacementSettings.Instance.CurrentlyPlacing = false;

            // Clears preview list
            while (_listOfMachinesToBePlacedPreview.Count > 0)
            {
                Destroy(_listOfMachinesToBePlacedPreview[0]);
                _listOfMachinesToBePlacedPreview.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Places the machine at selectedTile if there is not already something there
    /// </summary>
    /// <param name="selectedTile">Where to place</param>
    /// <param name="placeRotation">What rotation to rotate the machine</param>
    /// <param name="machineType">What type of machine to place</param>
    private void PlaceMachine(Vector2Int selectedTile, int placeRotation, GameObject machineType)
    {
        if (FactoryGrid.Instance.GetMachine(selectedTile) == null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() ==  "BlockerMachine")
        {
            GameObject lastPlaced = Instantiate(machineType, new Vector3(selectedTile.x * 10 + 5, 0.1f + 2.5f, selectedTile.y * 10 + 5), Quaternion.identity);
            FactoryGrid.Instance.PlaceMachine(selectedTile.x, selectedTile.y, placeRotation, lastPlaced.GetComponent<Machine>());
        }
    }

    /// <summary>
    /// Removes the machine at selectedTile
    /// </summary>
    /// <param name="selectedTile">Where to remove</param>
    private void RemoveMachine(Vector2Int selectedTile)
    {
        if (FactoryGrid.Instance.GetMachine(selectedTile) != null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() != "BlockerMachine")
        {
            Destroy(FactoryGrid.Instance.GetMachine(selectedTile).gameObject);
            FactoryGrid.Instance.RemoveMachine(selectedTile);
        }
    }
}


