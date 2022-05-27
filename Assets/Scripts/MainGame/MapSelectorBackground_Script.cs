using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelectorBackground_Script : MonoBehaviour
{
    /*
     * handles the map selection ui
     */
    /*
    [SerializeField] RectTransform ScrollViewTransform;
    [SerializeField] RectTransform ContentTransform;
    [SerializeField] GameObject GameHandler;
    [SerializeField] GameObject prefab;
    [SerializeField] MainGame_Handler_Script mainHandler;
    List<General_ViewportContentItemMapSelector_Script> mapSelectors = new List<General_ViewportContentItemMapSelector_Script>();
    // Start is called before the first frame update
    void Start()
    {
        string[] fileNames = GameHandler.GetComponent<SaveLoad_Handler_Script>().getSaveFileNames();
        //ScrollViewTransform.rect.Set(ScrollViewTransform.rect.x, ScrollViewTransform.rect.y, ScrollViewTransform.rect.width, 20 + (100 * fileNames.Length));
        ScrollViewTransform.sizeDelta = new Vector2(ScrollViewTransform.sizeDelta.x, 20 + (100 * fileNames.Length));
        //Debug.Log(ScrollViewTransform.rect.height);
        //Debug.Log(20 + (100 * fileNames.Length));
        for (int i = 0; i < fileNames.Length; i++)
        {
            GameObject go = Instantiate(prefab, ContentTransform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localPosition = new Vector2(0, (-102 * i) - 55);
            General_ViewportContentItemMapSelector_Script scr = go.GetComponent<General_ViewportContentItemMapSelector_Script>();
            mapSelectors.Add(scr);
            SaveLoad_Handler_Script sc = go.GetComponent<SaveLoad_Handler_Script>();
            SaveLoad_Handler_Script.saveClass data = sc.getMapData(fileNames[i]);



            scr.setup(data.MapName, fileNames[i], false, () => { mainHandler.LoadMapDataPush(scr.mapName); scr.isCached = true; scr.reflectCachedValueValue(); }, () => { handleCacheButtonUpdate(scr); });
        }
    }


    void handleCacheButtonUpdate(General_ViewportContentItemMapSelector_Script scr)
    {
        if (scr.isCached)
        {
            mainHandler.addMapToGlobalCachePush(scr.mapID);
        }
        else
        {
            mainHandler.removeMapFromGlobalCachePush(scr.mapID);
        }
    }

    */

}
