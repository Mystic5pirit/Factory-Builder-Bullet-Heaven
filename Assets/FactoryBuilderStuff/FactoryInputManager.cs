using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInputManager : MonoBehaviour
{
    [SerializeField] private Camera _sceneCamera;
    private Vector3 _lastPosition;
    [SerializeField] private LayerMask _placementLayermask;

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
    
    public Vector2Int GetSelectedMapTile()
    {
        Vector3 mousepos = Input.mousePosition;
        mousepos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousepos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, _placementLayermask))
        {
            _lastPosition = hit.point;
        }
        if (_lastPosition.x < 0) { _lastPosition.x = 0; }
        if (_lastPosition.z < 0) { _lastPosition.z = 0; }
        if (_lastPosition.x > FactoryGrid.Instance.GetSize() * 9) { _lastPosition.x = FactoryGrid.Instance.GetSize() * 9; }
        if (_lastPosition.z > FactoryGrid.Instance.GetSize() * 9) { _lastPosition.z = FactoryGrid.Instance.GetSize() * 9; }
        return new Vector2Int((int)(_lastPosition.x / 10), (int)(_lastPosition.z / 10));
    }
}
