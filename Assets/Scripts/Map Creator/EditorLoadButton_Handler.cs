using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorLoadButton_Handler : MonoBehaviour
{
    /*
     * handles  a load map request
     */
    [SerializeField] SaveLoad_Handler_Script saveLoad;
    [SerializeField] Button loadButton;
    [SerializeField] Canvas Canvas;
    [SerializeField] GameObject dropDownPrefab;

    private void Start()
    {
        loadButton.onClick.AddListener(() => createLoadDropDown());
    }

    bool alreadyActive = false;
    void createLoadDropDown()
    {
        if (alreadyActive)
        {
            return;
        }
        alreadyActive = true;
        GameObject go = Instantiate(dropDownPrefab);
        go.transform.parent = Canvas.transform;
        go.transform.localPosition = Vector3.zero;
        General_DropdownPopup_Handler DropHandler = go.GetComponent<General_DropdownPopup_Handler>();
        DropHandler.setupPopup("Select File To Load", saveLoad.getSaveFileMapDataNames(), "Load", "Cancel", (text, index) =>
        {
            alreadyActive = false;
            saveLoad.loadFromFile(saveLoad.getSaveFileNames()[index]);
        });
        DropHandler.addListenerToBothButtons(() =>
        {
            Destroy(go);
            alreadyActive = false;
        });
    }
}
