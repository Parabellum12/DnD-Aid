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
        UtilClass.CreateUiElementWithImage(transform, "stopMe", Vector2.zero, new Vector2(1920, 1080), "", new Color(0, 0, 0, 0), out GameObject gm, out RectTransform tr, out Image im);
        UiHolder = tr;
        UiHolder.gameObject.layer = LayerMask.NameToLayer("Popup Layer");
        //generate click lock background
        transform.localPosition = Vector2.zero;
        transform.localScale = Vector3.one;
        GameObject gameObject = Resources.Load("FileSelector/FileSelector") as GameObject;
        FileSelectorHandler = gameObject.GetComponent<FileSelectorHandler>();
        GameObject Selector = Instantiate(gameObject, transform);
        Selector.transform.localPosition = Vector2.zero;

    }
    public void OpenFileSelector(string initialPath, string[] extentionsToSearchFor, bool lockSearchToInitialPath, System.Action<string[]> callback)
    {
        FileSelectorHandler.OpenFileSelector(initialPath, extentionsToSearchFor, lockSearchToInitialPath, callback);
    }
}
