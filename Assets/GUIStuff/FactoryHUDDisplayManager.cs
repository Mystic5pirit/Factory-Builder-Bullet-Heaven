using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryHUDDisplayManager : MonoBehaviour
{
    /// <summary>
    /// List of mode displayers
    /// </summary>
    [SerializeField] private Image[] _primaryModeImages, _secondaryModeImages;
    /// <summary>
    /// The RectTransform of the current rotation display
    /// </summary>
    [SerializeField] private RectTransform _rotationDisplay;
    /// <summary>
    /// The TextMeshPro of the machine type displayer
    /// </summary>
    [SerializeField] private TextMeshProUGUI _machineTypeDisplay;

    [SerializeField] private GameObject[] _iODisplays;

    [SerializeField] private GameObject[] _iOLockDisplays;

    [SerializeField] private RectTransform _iODisplayPanel;

    /// <summary>
    /// Sets the color of the mode displayers to grey and the selected one to white
    /// </summary>
    /// <param name="placementMode">Which one to set to white</param>
    public void SelectPrimaryImage(PlacementMode.PlacementModeTypes placementMode)
    {
        for (int i = 0; i < _primaryModeImages.Length; i++)
        {
            _primaryModeImages[i].color = new Color(0.6f, 0.6f, 0.6f);
        }
        _primaryModeImages[(int) placementMode].color = new Color(255, 255, 255);
    }
    /// <summary>
    /// Sets the color of the mode displayers to grey and the selected one to white
    /// </summary>
    /// <param name="placementMode">Which one to set to white</param>
    public void SelectSecondaryImage(PlacementMode.PlacementModeTypes placementMode)
    {
        for (int i = 0; i < _secondaryModeImages.Length; i++)
        {
            _secondaryModeImages[i].color = new Color(0.6f, 0.6f, 0.6f);
        }
        _secondaryModeImages[(int)placementMode].color = new Color(255, 255, 255);
    }
    /// <summary>
    /// Rotates the rotation 90 degrees clockwise
    /// </summary>
    /// <param name="rotation"></param>
    public void RotateRotationDisplay()
    {
        _rotationDisplay.Rotate(Vector3.forward, -90);
    }
    /// <summary>
    /// Changes the text of the machine type display box
    /// </summary>
    /// <param name="text"></param>
    public void ChangeMachineTypeDisplayText(string text)
    {
        _machineTypeDisplay.text = text;
    }

    /// <summary>
    /// Changes the Input/Output display, including the locks
    /// </summary>
    /// <param name="iOArray"></param>
    public void ChangeIODisplay(IOLock[] iOArray)
    {
        // Changes each direction
        for (int i = 0; i < iOArray.Length && i < _iODisplays.Length; i++)
        {
            // Changes if Input
            if (iOArray[i].IO.IOType == InputOutput.InputOrOutput.Input)
            {
                _iODisplays[i].GetComponent<Image>().color = new Color(0, 0.5f, 1);
                _iODisplays[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180 - (i * 90));
                if (iOArray[i].Locked)
                {
                    _iOLockDisplays[i].GetComponent<Image>().color = Color.white;

                }
                else
                {
                    _iOLockDisplays[i].GetComponent<Image>().color = Color.clear;
                }
            // Changes if Output
            } else if (iOArray[i].IO.IOType == InputOutput.InputOrOutput.Output)
            {
                _iODisplays[i].GetComponent<Image>().color = new Color(1, 0.5f, 0);
                _iODisplays[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, i * -90);
                if (iOArray[i].Locked)
                {
                    _iOLockDisplays[i].GetComponent<Image>().color = Color.white;

                }
                else
                {
                    _iOLockDisplays[i].GetComponent<Image>().color = Color.clear;
                }
            // Hides if neither
            } else
            {
                _iODisplays[i].GetComponent<Image>().color = Color.clear;
                _iOLockDisplays[i].GetComponent<Image>().color = Color.clear;
            }
            
        }
    }

    /// <summary>
    /// Rotates the Input/Output display, adjusting the positions and rotations of the locks
    /// </summary>
    public void RotateIODisplay()
    {
        _iODisplayPanel.Rotate(Vector3.forward, -90);
        for (int i = 0; i < _iOLockDisplays.Length ; i++)
        {
            _iOLockDisplays[i].GetComponent<RectTransform>().Rotate(Vector3.forward, 90);
            _iOLockDisplays[i].GetComponent<RectTransform>().position = new Vector3(_iODisplays[i].GetComponent<RectTransform>().position.x + 10, _iODisplays[i].GetComponent<RectTransform>().position.y + 10, 0);
        }
    }
}
