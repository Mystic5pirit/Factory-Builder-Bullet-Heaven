using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInputManager : MonoBehaviour
{
    // Variables relating to finding where the mouse is looking at
    [SerializeField] private Camera _sceneCamera;
    private Vector3 _lastPosition;
    [SerializeField] private LayerMask _placementLayermask;

    [SerializeField] private FactoryHUDDisplayManager _hudDisplayManager;

    /// <summary>
    /// Returns position of mouse cursor
    /// </summary>
    /// <returns>Position of mouse cursor</returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,1000, _placementLayermask))
        {
            _lastPosition = hit.point;
        }
        return _lastPosition;
    }
    
    /// <summary>
    /// Returns tile of mouse cursor
    /// </summary>
    /// <returns>Tile of mouse cursor</returns>
    public Vector2Int GetSelectedMapTile()
    {
        Vector3 mousepos = Input.mousePosition;
        mousepos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousepos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, _placementLayermask))
        {
            _lastPosition = hit.point;
        }
        if (_lastPosition.x < 0) { _lastPosition.x = 0; }
        if (_lastPosition.z < 0) { _lastPosition.z = 0; }
        if (_lastPosition.x > FactoryGrid.Instance.GetSize() * 9) { _lastPosition.x = FactoryGrid.Instance.GetSize() * 9; }
        if (_lastPosition.z > FactoryGrid.Instance.GetSize() * 9) { _lastPosition.z = FactoryGrid.Instance.GetSize() * 9; }
        return new Vector2Int((int)(_lastPosition.x / 10), (int)(_lastPosition.z / 10));
    }

    private void Start()
    {
        _hudDisplayManager.ChangeMachineTypeDisplayText(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex].GetComponent<Machine>().GetMachineType());
        _hudDisplayManager.ChangeIODisplay(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex].GetComponent<Machine>().GetIOArray());

        _hudDisplayManager.SelectPrimaryImage(PlacementSettings.Instance.PlacementMode.SelectedPlacementMode);
        _hudDisplayManager.SelectSecondaryImage(PlacementSettings.Instance.SecondaryPlacementMode.SelectedPlacementMode);
    }

    private void Update()
    {
        // Ensures settings are not changed in the middle of placing
        if (!PlacementSettings.Instance.CurrentlyPlacing)
        {
            // Switch machine type
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlacementSettings.Instance.MachineListIndex = (PlacementSettings.Instance.MachineListIndex + 1) % PlacementSettings.Instance.MachineList.Length;
                _hudDisplayManager.ChangeMachineTypeDisplayText(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex].GetComponent<Machine>().GetMachineType());
                _hudDisplayManager.ChangeIODisplay(PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex].GetComponent<Machine>().GetIOArray());
            }

            // Switch rotation
            if (Input.GetKeyDown(KeyCode.R))
            {

                PlacementSettings.Instance.PlaceRotation = (PlacementSettings.Instance.PlaceRotation + 1) % 4;
                _hudDisplayManager.RotateRotationDisplay();
                _hudDisplayManager.RotateIODisplay();
            }

            // Switch placement mode
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlacementSettings.Instance.PlacementMode.Cycle();
                Debug.Log(PlacementSettings.Instance.PlacementMode.ToString());
                _hudDisplayManager.SelectPrimaryImage(PlacementSettings.Instance.PlacementMode.SelectedPlacementMode);

            }

            // Switch secondary placement mode
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                PlacementSettings.Instance.SecondaryPlacementMode.Cycle();
                Debug.Log(PlacementSettings.Instance.SecondaryPlacementMode.ToString());
                _hudDisplayManager.SelectSecondaryImage(PlacementSettings.Instance.SecondaryPlacementMode.SelectedPlacementMode);
            }

            // Print out the grid
            if (Input.GetKeyDown(KeyCode.P))
            {
                FactoryGrid.Instance.PrintOutTheGrid();
            }
        }
        
    }
}
