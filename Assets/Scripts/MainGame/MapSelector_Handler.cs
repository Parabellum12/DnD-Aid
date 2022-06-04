using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelector_Handler : MonoBehaviour
{
    [SerializeField] GameObject MapSelectorPrefab;
    [SerializeField] GameObject SharedMapSelectorPrefab;
    [SerializeField] MainGame_Handler_Script mainHandler_Script;
    [SerializeField] SaveLoad_Handler_Script saveLoadHandler_Script;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 contentDropDownHandler_Script;
    List<General_ViewportContentItemMapSelector_Script> mapSelectors = new List<General_ViewportContentItemMapSelector_Script>();
    List<sharedMapSelector_Handler> SharedMapSelectors = new List<sharedMapSelector_Handler>();
    private void Start()
    {
        contentDropDownHandler_Script.setUIPositionsNoCallback();
        loadLocalFiles();
    }



    [SerializeField] General_UI_DropDown_Handler_ScriptV2 localFilesDropDown;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 cachedFilesDropdown;




    public void loadLocalFiles()
    {
        foreach (General_ViewportContentItemMapSelector_Script ms in mapSelectors)
        {
            ms.KILLME();        
        }
        string[] fileNames = saveLoadHandler_Script.getSaveFileNames();
        localFilesDropDown.clearChildDropDowns();
        for (int i = 0; i < fileNames.Length; i++)
        {
            GameObject go = Instantiate(MapSelectorPrefab, localFilesDropDown.ChildrenObjectHolder.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            General_UI_DropDown_Handler_ScriptV2 tempDropDown = go.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
            localFilesDropDown.addToChildDropDowns(tempDropDown);
            General_ViewportContentItemMapSelector_Script scr = go.GetComponent<General_ViewportContentItemMapSelector_Script>();
            mapSelectors.Add(scr);

            SaveLoad_Handler_Script sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveLoad_Handler_Script>();
            SaveLoad_Handler_Script.saveClass data = sc.getMapData(fileNames[i]);


            scr.setup(data.MapName, fileNames[i], false, () =>
            {
                mainHandler_Script.LoadMapDataPush(scr.mapID); 
            }, () =>
            {
                handleShareButtonUpdate(scr);
            });
        }
        localFilesDropDown.setUIPositions();
    }

    void addToLoadLocalFiles(SaveLoad_Handler_Script.saveClass savedMap)
    {
        GameObject go = Instantiate(MapSelectorPrefab, localFilesDropDown.ChildrenObjectHolder.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        General_UI_DropDown_Handler_ScriptV2 tempDropDown = go.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
        localFilesDropDown.addToChildDropDowns(tempDropDown);
        General_ViewportContentItemMapSelector_Script scr = go.GetComponent<General_ViewportContentItemMapSelector_Script>();
        mapSelectors.Add(scr);



        scr.setup(savedMap.MapName, savedMap.MapID, false, () =>
        {
            mainHandler_Script.LoadMapDataPush(scr.mapID);
        }, () =>
        {
            handleShareButtonUpdate(scr);
        });
        localFilesDropDown.setUIPositions();
    }

    public void loadSharedFiles()
    {
        foreach (sharedMapSelector_Handler scr in SharedMapSelectors)
        {
            scr.KILLME();
        }
        string[] arr = saveLoadHandler_Script.getSaveFileNames();
        foreach (SaveLoad_Handler_Script.saveClass sc in mainHandler_Script.SharedMaps)
        {
            bool alreadySaved = false;
            foreach (string s in arr)
            {
                if (s.Equals(sc.MapID))
                {
                    alreadySaved = true;
                    break;
                }
            }
            if (alreadySaved)
            {
                continue;
            }
            GameObject go = Instantiate(SharedMapSelectorPrefab, cachedFilesDropdown.ChildrenObjectHolder.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            sharedMapSelector_Handler scr = go.GetComponent<sharedMapSelector_Handler>();
            scr.setup(sc, (mapToAdd) =>
            {
                addToLoadLocalFiles(mapToAdd);
                scr.KILLME();
            });
        }
    }




    void handleShareButtonUpdate(General_ViewportContentItemMapSelector_Script scr)
    {
        if (scr.isCached)
        {
            mainHandler_Script.addToSharedMaps(UtilClass.ObjectToByteArray(saveLoadHandler_Script.getMapData(scr.mapID)), false);
        }
        else
        {
            mainHandler_Script.removeFromSharedMaps(UtilClass.ObjectToByteArray(saveLoadHandler_Script.getMapData(scr.mapID)), false);
        }
    }
}
