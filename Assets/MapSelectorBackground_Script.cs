using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelectorBackground_Script : MonoBehaviour
{
    [SerializeField] RectTransform ScrollViewTransform;
    [SerializeField] RectTransform ContentTransform;
    [SerializeField] GameObject GameHandler;
    [SerializeField] GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        string[] fileNames = GameHandler.GetComponent<SaveLoad_Handler_Script>().getSaveFileNames();
        //ScrollViewTransform.rect.Set(ScrollViewTransform.rect.x, ScrollViewTransform.rect.y, ScrollViewTransform.rect.width, 20 + (100 * fileNames.Length));
        ScrollViewTransform.sizeDelta = new Vector2(ScrollViewTransform.sizeDelta.x, 20 + (100 * fileNames.Length));
        Debug.Log(ScrollViewTransform.rect.height);
        Debug.Log(20 + (100 * fileNames.Length));
        for (int i = 0; i < fileNames.Length; i++)
        {
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(ContentTransform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localPosition = new Vector3(0, (-102 * i) - 55, 0);
            go.transform.localScale = Vector3.one;
            General_ViewportContentItemMapSelector_Script scr = go.GetComponent<General_ViewportContentItemMapSelector_Script>();
            scr.setup(fileNames[i], false, () => {  }, () => {  });
        }
    }
}