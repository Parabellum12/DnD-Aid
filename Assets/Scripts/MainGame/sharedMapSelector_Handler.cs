using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sharedMapSelector_Handler : MonoBehaviour
{
    [SerializeField] TMP_Text mapName;
    [SerializeField] Button saveButton;

    SaveLoad_Handler_Script.saveClass referenceMap;
    System.Action<SaveLoad_Handler_Script.saveClass> callback;
    public void setup(SaveLoad_Handler_Script.saveClass referenceMap, System.Action<SaveLoad_Handler_Script.saveClass> callback)
    {
        this.callback = callback;
        this.referenceMap = referenceMap;
        mapName.text = referenceMap.MapName;
        saveButton.onClick.AddListener(() =>
        {
            handleSave();
        });
    }

    public void handleSave()
    {
        SaveLoad_Handler_Script scr = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveLoad_Handler_Script>();
        scr.saveToFile(referenceMap);
        callback.Invoke(referenceMap);
    }

    public void KILLME()
    {
        Destroy(gameObject);
    }
}
