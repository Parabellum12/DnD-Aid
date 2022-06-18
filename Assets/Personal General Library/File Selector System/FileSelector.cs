using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class FileSelector : MonoBehaviour
{
    RectTransform UiHolder;
    FileSelectorHandler FileSelectorHandler;
    public void GenerateFileSelectorUi()
    {
        //generate click lock background
        transform.localPosition = Vector2.zero;
        transform.localScale = Vector3.one;
        GameObject gameObject = Resources.Load("FileSelector/FileSelector") as GameObject;
        GameObject Selector = Instantiate(gameObject, transform);
        Selector.transform.localPosition = Vector2.zero;
    }
    public void OpenFileSelector(string initialPath, string[] extentionsToSearchFor, bool lockSearchToInitialPath, System.Action<string[]> callback)
    {

    }
}
