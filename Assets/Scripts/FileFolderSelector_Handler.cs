using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class FileFolderSelector_Handler : MonoBehaviour
{
    [SerializeField] TMP_Text fileFolderName;

    [SerializeField] Image TypeImage;



    [SerializeField] Sprite FolderSprite;

    [SerializeField] Sprite TxtSprite;
    [SerializeField] Sprite ExeSprite;

    //image 
    [SerializeField] Sprite PngSprite;
    [SerializeField] Sprite JpegSprite;


    [SerializeField] Sprite OtherFileSprite;




    System.Action<string> callback;
    public void setup(bool FolderOrFile, string name, string extentionType, System.Action<string> callback)
    {
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
        }


    }
}
