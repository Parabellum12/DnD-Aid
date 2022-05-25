using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolBarDropDown_Handler_Script : MonoBehaviour
{
    /*
     * handles the ui for the dropdown buttons in the editor
     */
    [SerializeField] Button mainButton;
    [SerializeField] GameObject dropdownMenu;

    private void Start()
    {
        mainButton.onClick.AddListener(() => handleButtonCLick());
        dropdownMenu.SetActive(false);
    }
    bool toggle = false;
    void handleButtonCLick()
    {
        if (toggle)
        {
            dropdownMenu.SetActive(false);
        }
        else
        {
            dropdownMenu.SetActive(true);
        }
        toggle = !toggle;
    }

}
