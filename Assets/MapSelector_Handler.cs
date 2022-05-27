using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelector_Handler : MonoBehaviour
{
    [SerializeField] GameObject MapSelectorPrefab;
    [SerializeField] MainGame_Handler_Script mainHandler_Script;
    [SerializeField] SaveLoad_Handler_Script saveLoadHandler_Script;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 contentDropDownHandler_Script;
    List<General_ViewportContentItemMapSelector_Script> mapSelectors = new List<General_ViewportContentItemMapSelector_Script>();
    private void Start()
    {
        contentDropDownHandler_Script.setUIPositionsNoCallback();
        loadLocalFiles();
    }



    [SerializeField] General_UI_DropDown_Handler_ScriptV2 localFilesDropDown;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 cachedFilesDropdown;




    public void loadLocalFiles()
    {
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
            scr.setup(fileNames[i], false, () =>
            {
                mainHandler_Script.LoadMapDataPush(scr.mapName); 
                scr.isCached = true; 
                scr.reflectCachedValueValue();
            }, () =>
            {
                handleCacheButtonUpdate(scr);
            });
        }
        localFilesDropDown.setUIPositions();
    }

    public void loadCachedFiles()
    {
        
    }




    void handleCacheButtonUpdate(General_ViewportContentItemMapSelector_Script scr)
    {
        if (scr.isCached)
        {
            mainHandler_Script.addMapToGlobalCachePush(scr.mapName);
        }
        else
        {
            mainHandler_Script.removeMapFromGlobalCachePush(scr.mapName);
        }
    }
}
