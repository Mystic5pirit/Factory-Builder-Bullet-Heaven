using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoveringOverGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Changes when the mouse hovers over this UI element
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        PlacementSettings.Instance.IsHoveringOverUI = true;
    }

    // Changes when the mouse hovers over this UI element
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        PlacementSettings.Instance.IsHoveringOverUI = false;
    }
}
