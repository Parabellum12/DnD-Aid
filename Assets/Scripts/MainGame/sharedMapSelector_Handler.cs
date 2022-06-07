using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sharedMapSelector_Handler : MonoBehaviour
{
    [SerializeField] TMP_Text mapName;
    [SerializeField] Button saveButton;
    [SerializeField] TMP_Text buttonText;

    SaveLoad_Handler_Script.saveClass referenceMap;
    System.Action<SaveLoad_Handler_Script.saveClass, bool> callback;
    bool newOrUpdate;
    public void setup(SaveLoad_Handler_Script.saveClass referenceMap, bool newOrUpdate, System.Action<SaveLoad_Handler_Script.saveClass, bool> callback)
    {
        this.newOrUpdate = newOrUpdate;
        this.callback = callback;
        this.referenceMap = referenceMap;
        mapName.text = referenceMap.MapName;
        if (newOrUpdate)
        {
            buttonText.text = "Update";
        }
        else
        {
            buttonText.text = "Save";
        }
        saveButton.onClick.AddListener(() =>
        {
            handleSave();
        });
    }

    public void handleSave()
    {
        SaveLoad_Handler_Script scr = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveLoad_Handler_Script>();
        scr.saveToFile(referenceMap, false);
        callback.Invoke(referenceMap, newOrUpdate);
    }

    public void KILLME()
    {
        Destroy(gameObject);
    }
}
