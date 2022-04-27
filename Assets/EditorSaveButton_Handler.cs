using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSaveButton_Handler : MonoBehaviour
{
    [SerializeField] SaveLoad_Handler_Script saveLoad;
    [SerializeField] Button saveButton;
    [SerializeField] Canvas Canvas;
    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(() => createSavePopUp());
    }

    [SerializeField] GameObject promptPrefab;
    bool alreadyActive = false;
    void createSavePopUp()
    {
        if (alreadyActive)
        {
            return;
        }

        alreadyActive = true;
        GameObject go = Instantiate(promptPrefab);
        General_InputPopup_Handler popupHandler = go.GetComponent<General_InputPopup_Handler>();
        popupHandler.setupPopup("Save File As", "Accept", "Cancel", (text) => saveLoad.saveToFile(text));
        go.transform.parent = Canvas.transform;
        go.transform.localPosition = Vector2.zero;
        popupHandler.addListenerToBothButtons(() =>
        {
            alreadyActive = false;
            Destroy(go);
        });
    }
}
