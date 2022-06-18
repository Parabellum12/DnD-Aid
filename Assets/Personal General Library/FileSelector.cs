using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileSelector : MonoBehaviour
{
    RectTransform UiHolder;
    int memSize = 10;
    List<string> pathMemory = new List<string>();
    int memIndex = 0;
    public void GenerateFileSelectorUi(GameObject holder)
    {
        UiHolder = holder.AddComponent<RectTransform>();
        UiHolder.localScale = Vector3.one;
        UiHolder.anchorMax = new Vector2(1,1);
        UiHolder.anchorMin = new Vector2(0, 0);
        UiHolder.sizeDelta = new Vector2(0,0);
        UiHolder.localPosition = Vector2.zero;
        UiHolder.gameObject.layer = LayerMask.NameToLayer("Popup Layer");
    }
    public void OpenFileSelector(string initialPath, string[] extentionsToSearchFor, bool lockSearchToInitialPath, System.Action<string[]> callback)
    {

    }


    void addToPathMemory(string pathToAdd)
    {
        pathMemory.Add(pathToAdd);
    }
}
