using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;


public class FileFolderSelector_Handler : MonoBehaviour
{
    [SerializeField] TMP_Text fileFolderName;

    [SerializeField] Image BackgroundImage;
    [SerializeField] Image TypeImage;



    [SerializeField] Sprite FolderSprite;

    [SerializeField] Sprite TxtSprite;
    [SerializeField] Sprite ExeSprite;

    //image 
    [SerializeField] Sprite PngSprite;
    [SerializeField] Sprite JpegSprite;


    [SerializeField] Sprite OtherFileSprite;

    bool selected;
    FileSelectorHandler SelectorHandler;


    System.Action<string> callback;

    bool isFolder;

    public void Awake()
    {
        DeselectMe();
    }
    public void setup(FileSelectorHandler SelectorHandler, bool FolderOrFile, string name, string extentionType, System.Action<string> callback)
    {
        isFolder = FolderOrFile;
        this.SelectorHandler = SelectorHandler;
        this.callback = callback;
        fileFolderName.text = name;
        if (FolderOrFile)
        {
            TypeImage.sprite = FolderSprite;
        }
        else
        {
            switch (extentionType)
            {
                case "txt":
                    TypeImage.sprite = TxtSprite;
                    break;
                case "exe":
                    TypeImage.sprite = ExeSprite;
                    break;
                case "png":
                    TypeImage.sprite = PngSprite;
                    break;
                case "jpeg":
                    TypeImage.sprite = JpegSprite;
                    break;
                default:
                    TypeImage.sprite = OtherFileSprite;
                    break;
            }
        }


    }

    float lastClickTime = 0;
    float maxTimeForDoubleCLick = .3f;
    public void handleClick()
    {

        if (Time.realtimeSinceStartup - lastClickTime <= maxTimeForDoubleCLick)
        {
            callback.Invoke(fileFolderName.text);
        }
        else
        {
            lastClickTime = Time.realtimeSinceStartup;
            SelectorHandler.HandleSelection(this);
        }
    }
    void handleSelectEvent()
    {
        if (isFolder)
        {
            return;
        }
        if (selected)
        {
            SelectorHandler.SelectedFiles.Add(this);
        }
        else
        {
            SelectorHandler.SelectedFiles.Remove(this);
        }
    }

    public void SelectMe()
    {
        if (selected)
        {
            return;
        }
        Debug.Log("SelectMe");
       BackgroundImage.color = new Color(0, 176, 251);


       selected = true;
        handleSelectEvent();
    }

    public void DeselectMe()
    {
        if (!selected)
        {
            return;
        }
        Debug.Log("DeselectMe");
        BackgroundImage.color = new Color(239, 235, 235, 120/255);



        selected = false;
        handleSelectEvent();
    }

    public string getText()
    {
        return fileFolderName.text;
    }
}
