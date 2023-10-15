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
    /// Previously selected tile which was to be placed <br/>
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
    /// List of tiles which have been selected in drag/dragRemove mode
    /// </summary>
    private List<Vector2Int> _listOfSelectedTiles = new();
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
    private bool _leftClicking, _rightClicking;


    private void Update()
    {
        // Making code cleaner
        PlacementModeTypes placementMode = PlacementSettings.Instance.PlacementMode.SelectedPlacementMode;
        PlacementModeTypes secondaryPlacementMode = PlacementSettings.Instance.SecondaryPlacementMode.SelectedPlacementMode;

        // Puts the _tileSelector in the correct spot
        _selectedTile = _factoryInputManager.GetSelectedMapTile();
        Vector3 tilePosition = new(_selectedTile.x * 10 + 5, 0.1f, _selectedTile.y * 10 + 5);
        _tileSelector.transform.position = tilePosition;

        // Ensures machines are not placed when the mouse is over UI elements
        if (!PlacementSettings.Instance.IsHoveringOverUI)
        {
            // When left mouse is clicked
            if (Input.GetMouseButtonDown(0) && !_rightClicking)
            {
                _leftClicking = true;
                // Clears lists just in case
                _listOfMachinesToBePlaced.Clear();
                _listOfMachinesToBePlacedPreview.Clear();
                _listOfSelectedTiles.Clear();
                PlacementSettings.Instance.CurrentlyPlacing = true;
                // Adds the first tile for Drag mode
                if (placementMode == PlacementModeTypes.Drag)
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                    _listOfSelectedTiles.Add(_selectedTile);
                }
                // Sets origin and creates selection boxes for Line mode 
                if (placementMode == PlacementModeTypes.Line)
                {
                    _placementOrigin = _selectedTile;
                    //_linePreviewBox = Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, (int)PlacementSettings.Instance.PlaceRotation.FacingDirection * 90, 0));

                }
                // Adds the first tile for DragRemove mode
                if (placementMode == PlacementModeTypes.DragRemove)
                {
                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                    _listOfSelectedTiles.Add(_selectedTile);

                }
                // Sets origin and creates selection boxes for RectangleRemove mode
                if (placementMode == PlacementModeTypes.RectangleRemove)
                {
                    _placementOrigin = _selectedTile;
                    _rectangleRemovalBox = Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0));
                    _listOfSelectedTiles.Add(_selectedTile);

                }
            }

            // Every frame that the left mouse is pressed
            if (Input.GetMouseButton(0) && !_rightClicking)
            {
                // Ensures that it doesn't try adding every tick
                if (_lastAddedTile != _selectedTile)
                {
                    // Drag mode - Adds each tile went over to the list, does not add if it is already on the list, Previews machines and boxes also
                    if (placementMode == PlacementModeTypes.Drag && !FactoryGrid.Instance.IsThereAMachineThere(_selectedTile))
                    {
                        // Ensures that the list contains only one of any given position
                        if (!_listOfMachinesToBePlaced.Contains(_selectedTile))
                        {
                            _listOfMachinesToBePlaced.Add(_selectedTile);
                            _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                            _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                            _lastAddedTile = _selectedTile;
                            _listOfSelectedTiles.Add(_selectedTile);


                        } // Allows for dragging backwards to remove from the list
                        else if (_listOfSelectedTiles.Count >= 2 && _selectedTile == _listOfSelectedTiles[_listOfSelectedTiles.Count - 2])
                        {
                            if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] == _listOfMachinesToBePlaced[_listOfMachinesToBePlaced.Count - 1])
                            {
                                // Removes the tile from the list
                                _listOfMachinesToBePlaced.RemoveAt(_listOfMachinesToBePlaced.Count - 1);
                                // Destroys the preview box
                                Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                                _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                                // Destroys the preview machine
                                Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                                _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                            }
                            // Removes last one on list and fixes _lastAddedTile
                            _listOfSelectedTiles.RemoveAt(_listOfSelectedTiles.Count - 1);
                            _lastAddedTile = _listOfSelectedTiles[_listOfSelectedTiles.Count - 1];
                        } // Adds the new tile to the list of where the mouse has been, as long as it is not the most recent tile on the list
                        if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] != _selectedTile)
                        {
                            _listOfSelectedTiles.Add(_selectedTile);
                        }



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
                        _lastAddedTile = _selectedTile;

                    }
                }
                Debug.Log("_selectedTile: " + _selectedTile.ToString());
                Debug.Log("_lastAddedTile: " + _lastAddedTile.ToString());
                string debugString1 = "Machines: ";
                for (int i = 0; i < _listOfMachinesToBePlaced.Count; i++)
                {
                    debugString1 += _listOfMachinesToBePlaced[i].ToString() + ", ";
                }
                Debug.Log(debugString1);
                string debugString2 = "Selected Tiles: ";
                for (int i = 0; i < _listOfSelectedTiles.Count; i++)
                {
                    debugString2 += _listOfSelectedTiles[i].ToString() + ", ";
                }
                Debug.Log(debugString2);
                // DragRemove mode - Adds each tile went over to the list, does not add if it is already on the list, Previews boxes also
                if (placementMode == PlacementModeTypes.DragRemove)
                {
                    // Ensures that the list contains only one of any given position
                    if (!_listOfMachinesToBePlaced.Contains(_selectedTile))
                    {

                        _listOfMachinesToBePlaced.Add(_selectedTile);
                        _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                        _lastAddedTile = _selectedTile;
                        _listOfSelectedTiles.Add(_selectedTile);


                    } // Allows for dragging backwards to remove from the list
                    else if (_listOfSelectedTiles.Count >= 2 && _selectedTile == _listOfSelectedTiles[_listOfSelectedTiles.Count - 2])
                    {
                        if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] == _listOfMachinesToBePlaced[_listOfMachinesToBePlaced.Count - 1])
                        {
                            // Removes the tile from the list
                            _listOfMachinesToBePlaced.RemoveAt(_listOfMachinesToBePlaced.Count - 1);
                            // Destroys the preview box
                            Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                            _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                        }
                        // Removes last one on list and fixes _lastAddedTile
                        _listOfSelectedTiles.RemoveAt(_listOfSelectedTiles.Count - 1);
                        _lastAddedTile = _listOfSelectedTiles[_listOfSelectedTiles.Count - 1];

                    } // Adds the new tile to the list of where the mouse has been, as long as it is not the most recent tile on the list
                    if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] != _selectedTile)
                    {
                        _listOfSelectedTiles.Add(_selectedTile);
                    }

                }
                // RectangleRemove mode - Previews a box of all machines to remove
                if (placementMode == PlacementModeTypes.RectangleRemove)
                {
                    _rectangleRemovalBox.transform.position = new Vector3(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 + 5, 0.1f + 5, ((float)_placementOrigin.y + (float)_selectedTile.y) / 2 * 10 + 5);
                    _rectangleRemovalBox.transform.localScale = new Vector3(((_placementOrigin.x + 1) - (_selectedTile.x + 1) + Math.Sign(_placementOrigin.x - _selectedTile.x)) * 10, 10, ((_placementOrigin.y + 1) - (_selectedTile.y + 1) + Math.Sign(_placementOrigin.y - _selectedTile.y)) * 10);
                    // Ensures the scale is not zero when only a line or single tile
                    if (_rectangleRemovalBox.transform.localScale.x == 0)
                    {
                        _rectangleRemovalBox.transform.localScale = new Vector3(10, _rectangleRemovalBox.transform.localScale.y, _rectangleRemovalBox.transform.localScale.z);
                    }
                    if (_rectangleRemovalBox.transform.localScale.z == 0)
                    {
                        _rectangleRemovalBox.transform.localScale = new Vector3(_rectangleRemovalBox.transform.localScale.x, _rectangleRemovalBox.transform.localScale.y, 10);
                    }
                    _lastAddedTile = _selectedTile;

                }


            }

        }

        // When left mouse is released
        if (Input.GetMouseButtonUp(0) && !_rightClicking)
        {
            _leftClicking = false;
            PlacementSettings.Instance.CurrentlyPlacing = false;

            // Line mode - Adds each tile in the line to the list
            if (placementMode == PlacementModeTypes.Line)
            {
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
                int i = 0;
                do
                {
                    int j = 0;
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


            // Clears preview list
            while (_listOfMachinesToBePlacedPreview.Count > 0)
            {
                Destroy(_listOfMachinesToBePlacedPreview[0]);
                _listOfMachinesToBePlacedPreview.RemoveAt(0);
            }
        }

        // When right mouse is clicked
        if (Input.GetMouseButtonDown(1) && !_leftClicking)
        {
            _rightClicking = true;
            // Clears lists just in case
            _listOfMachinesToBePlaced.Clear();
            _listOfMachinesToBePlacedPreview.Clear();
            _listOfSelectedTiles.Clear();
            PlacementSettings.Instance.CurrentlyPlacing = true;
            // Adds the first tile for Drag mode
            if (secondaryPlacementMode == PlacementModeTypes.Drag)
            {
                _listOfMachinesToBePlaced.Add(_selectedTile);
                _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                _listOfSelectedTiles.Add(_selectedTile);
            }
            // Sets origin and creates selection boxes for Line mode 
            if (secondaryPlacementMode == PlacementModeTypes.Line)
            {
                _placementOrigin = _selectedTile;
                //_linePreviewBox = Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, (int)PlacementSettings.Instance.PlaceRotation.FacingDirection * 90, 0));

            }
            // Adds the first tile for DragRemove mode
            if (secondaryPlacementMode == PlacementModeTypes.DragRemove)
            {
                _listOfMachinesToBePlaced.Add(_selectedTile);
                _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                _listOfSelectedTiles.Add(_selectedTile);

            }
            // Sets origin and creates selection boxes for RectangleRemove mode
            if (secondaryPlacementMode == PlacementModeTypes.RectangleRemove)
            {
                _placementOrigin = _selectedTile;
                _rectangleRemovalBox = Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0));
                _listOfSelectedTiles.Add(_selectedTile);

            }
        }

        // Every frame that the right mouse is pressed
        if (Input.GetMouseButton(1) && !_leftClicking)
        {
            // Ensures that it doesn't try adding every tick
            if (_lastAddedTile != _selectedTile)
            {
                // Drag mode - Adds each tile went over to the list, does not add if it is already on the list, Previews machines and boxes also
                if (secondaryPlacementMode == PlacementModeTypes.Drag && !FactoryGrid.Instance.IsThereAMachineThere(_selectedTile))
                {
                    // Ensures that the list contains only one of any given position
                    if (!_listOfMachinesToBePlaced.Contains(_selectedTile))
                    {
                        _listOfMachinesToBePlaced.Add(_selectedTile);
                        _listOfMachinesToBePlacedPreview.Add(Instantiate(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex], new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                        _listOfMachinesToBePlacedPreview.Add(Instantiate(_previewBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 2.5f, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                        _lastAddedTile = _selectedTile;
                        _listOfSelectedTiles.Add(_selectedTile);


                    } // Allows for dragging backwards to remove from the list
                    else if (_listOfSelectedTiles.Count >= 2 && _selectedTile == _listOfSelectedTiles[_listOfSelectedTiles.Count - 2])
                    {
                        if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] == _listOfMachinesToBePlaced[_listOfMachinesToBePlaced.Count - 1])
                        {
                            // Removes the tile from the list
                            _listOfMachinesToBePlaced.RemoveAt(_listOfMachinesToBePlaced.Count - 1);
                            // Destroys the preview box
                            Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                            _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                            // Destroys the preview machine
                            Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                            _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                        }
                        // Removes last one on list and fixes _lastAddedTile
                        _listOfSelectedTiles.RemoveAt(_listOfSelectedTiles.Count - 1);
                        _lastAddedTile = _listOfSelectedTiles[_listOfSelectedTiles.Count - 1];
                    } // Adds the new tile to the list of where the mouse has been, as long as it is not the most recent tile on the list
                    if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] != _selectedTile)
                    {
                        _listOfSelectedTiles.Add(_selectedTile);
                    }



                }
                // Line mode - Previews machines and boxes in a cardinal direction towards the _selectedTile
                if (secondaryPlacementMode == PlacementModeTypes.Line)
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
                    _lastAddedTile = _selectedTile;

                }
            }
            Debug.Log("_selectedTile: " + _selectedTile.ToString());
            Debug.Log("_lastAddedTile: " + _lastAddedTile.ToString());
            string debugString1 = "Machines: ";
            for (int i = 0; i < _listOfMachinesToBePlaced.Count; i++)
            {
                debugString1 += _listOfMachinesToBePlaced[i].ToString() + ", ";
            }
            Debug.Log(debugString1);
            string debugString2 = "Selected Tiles: ";
            for (int i = 0; i < _listOfSelectedTiles.Count; i++)
            {
                debugString2 += _listOfSelectedTiles[i].ToString() + ", ";
            }
            Debug.Log(debugString2);
            // DragRemove mode - Adds each tile went over to the list, does not add if it is already on the list, Previews boxes also
            if (secondaryPlacementMode == PlacementModeTypes.DragRemove)
            {
                // Ensures that the list contains only one of any given position
                if (!_listOfMachinesToBePlaced.Contains(_selectedTile))
                {

                    _listOfMachinesToBePlaced.Add(_selectedTile);
                    _listOfMachinesToBePlacedPreview.Add(Instantiate(_removalBox, new Vector3(_selectedTile.x * 10 + 5, 0.1f + 5, _selectedTile.y * 10 + 5), Quaternion.Euler(0, PlacementSettings.Instance.PlaceRotation * 90, 0)));
                    _lastAddedTile = _selectedTile;
                    _listOfSelectedTiles.Add(_selectedTile);


                } // Allows for dragging backwards to remove from the list
                else if (_listOfSelectedTiles.Count >= 2 && _selectedTile == _listOfSelectedTiles[_listOfSelectedTiles.Count - 2])
                {
                    if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] == _listOfMachinesToBePlaced[_listOfMachinesToBePlaced.Count - 1])
                    {
                        // Removes the tile from the list
                        _listOfMachinesToBePlaced.RemoveAt(_listOfMachinesToBePlaced.Count - 1);
                        // Destroys the preview box
                        Destroy(_listOfMachinesToBePlacedPreview[_listOfMachinesToBePlacedPreview.Count - 1]);
                        _listOfMachinesToBePlacedPreview.RemoveAt(_listOfMachinesToBePlacedPreview.Count - 1);
                    }
                    // Removes last one on list and fixes _lastAddedTile
                    _listOfSelectedTiles.RemoveAt(_listOfSelectedTiles.Count - 1);
                    _lastAddedTile = _listOfSelectedTiles[_listOfSelectedTiles.Count - 1];

                } // Adds the new tile to the list of where the mouse has been, as long as it is not the most recent tile on the list
                if (_listOfSelectedTiles[_listOfSelectedTiles.Count - 1] != _selectedTile)
                {
                    _listOfSelectedTiles.Add(_selectedTile);
                }

            }
            // RectangleRemove mode - Previews a box of all machines to remove
            if (secondaryPlacementMode == PlacementModeTypes.RectangleRemove)
            {
                _rectangleRemovalBox.transform.position = new Vector3(((float)_placementOrigin.x + (float)_selectedTile.x) / 2 * 10 + 5, 0.1f + 5, ((float)_placementOrigin.y + (float)_selectedTile.y) / 2 * 10 + 5);
                _rectangleRemovalBox.transform.localScale = new Vector3(((_placementOrigin.x + 1) - (_selectedTile.x + 1) + Math.Sign(_placementOrigin.x - _selectedTile.x)) * 10, 10, ((_placementOrigin.y + 1) - (_selectedTile.y + 1) + Math.Sign(_placementOrigin.y - _selectedTile.y)) * 10);
                // Ensures the scale is not zero when only a line or single tile
                if (_rectangleRemovalBox.transform.localScale.x == 0)
                {
                    _rectangleRemovalBox.transform.localScale = new Vector3(10, _rectangleRemovalBox.transform.localScale.y, _rectangleRemovalBox.transform.localScale.z);
                }
                if (_rectangleRemovalBox.transform.localScale.z == 0)
                {
                    _rectangleRemovalBox.transform.localScale = new Vector3(_rectangleRemovalBox.transform.localScale.x, _rectangleRemovalBox.transform.localScale.y, 10);
                }
                _lastAddedTile = _selectedTile;

            }


        }

        // When right mouse is released
        if (Input.GetMouseButtonUp(1) && !_leftClicking)
        {
            _rightClicking = false;
            PlacementSettings.Instance.CurrentlyPlacing = false;

            // Line mode - Adds each tile in the line to the list
            if (secondaryPlacementMode == PlacementModeTypes.Line)
            {
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
                Vector2Int currentTile = _placementOrigin;
                _listOfMachinesToBePlaced.Clear();
                for (int i = 0; i < placementNumber; i++)
                {
                    _listOfMachinesToBePlaced.Add(currentTile);
                    currentTile += direction;
                }
            }
            // Adds all of the tiles in the rectangle to the list
            if (secondaryPlacementMode == PlacementModeTypes.RectangleRemove)
            {
                Vector2Int mouseOffset = _selectedTile - _placementOrigin;
                _listOfMachinesToBePlaced.Clear();
                // do/while instead of for loop so it does it once if it is just a line or single tile
                int i = 0;
                do
                {
                    int j = 0;
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
            if (secondaryPlacementMode == PlacementModeTypes.Drag || secondaryPlacementMode == PlacementModeTypes.Line)
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    PlaceMachine(selectedTile, PlacementSettings.Instance.PlaceRotation, PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex]);
                }
            }
            // Removal for placement modes which are for removing
            else if (secondaryPlacementMode == PlacementModeTypes.DragRemove || secondaryPlacementMode == PlacementModeTypes.RectangleRemove)
            {
                foreach (Vector2Int selectedTile in _listOfMachinesToBePlaced)
                {
                    RemoveMachine(selectedTile);
                }
            }
            else if (secondaryPlacementMode == PlacementModeTypes.Rotate)
            {
                FactoryGrid.Instance.GetMachine(_selectedTile).Rotate();
            }


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
        if (FactoryGrid.Instance.GetMachine(selectedTile) == null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() ==  "Blocker Machine")
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
        if (FactoryGrid.Instance.GetMachine(selectedTile) != null || FactoryGrid.Instance.GetMachine(selectedTile).GetMachineType() != "Blocker Machine")
        {
            Destroy(FactoryGrid.Instance.GetMachine(selectedTile).gameObject);
            FactoryGrid.Instance.RemoveMachine(selectedTile);
        }
    }
}


