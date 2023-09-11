using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInputManager : MonoBehaviour
{
    // Variables relating to finding where the mouse is looking at
    [SerializeField] private Camera _sceneCamera;
    private Vector3 _lastPosition;
    [SerializeField] private LayerMask _placementLayermask;

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


    private void Update()
    {
        // Switch machine type
        if (Input.GetKeyDown(KeyCode.Alpha1) && !PlacementSettings.Instance.CurrentlyPlacing)
        {
            PlacementSettings.Instance.MachineListIndex = (PlacementSettings.Instance.MachineListIndex + 1) % PlacementSettings.Instance.MachineList.Length;
            Debug.Log("Placing " + PlacementSettings.Instance.MachineList[PlacementSettings.Instance.MachineListIndex].GetComponent<Machine>().GetMachineType());
        }

        // Switch rotation
        if (Input.GetKeyDown(KeyCode.R) && !PlacementSettings.Instance.CurrentlyPlacing)
        {

            PlacementSettings.Instance.PlaceRotation = (PlacementSettings.Instance.PlaceRotation + 1) % 4;
            
            switch(PlacementSettings.Instance.PlaceRotation)
            {
                case 0:
                    Debug.Log("North");
                    break;
                case 1:
                    Debug.Log("East");
                    break;
                case 2:
                    Debug.Log("South");
                    break;
                case 3:
                    Debug.Log("West");
                    break;
                default:
                    break;
            }
        }

        // Switch placement mode
        if (Input.GetKeyDown(KeyCode.LeftShift) && !PlacementSettings.Instance.CurrentlyPlacing)
        {
            PlacementSettings.Instance.PlacementMode.Cycle();
            Debug.Log(PlacementSettings.Instance.PlacementMode.ToString());
            
        }

        // Print out the grid
        if (Input.GetKeyDown(KeyCode.P) && !PlacementSettings.Instance.CurrentlyPlacing)
        {
            FactoryGrid.Instance.PrintOutTheGrid();
        }
    }
}
