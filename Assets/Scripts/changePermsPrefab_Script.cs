using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class changePermsPrefab_Script : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button editPermsButton;
    [SerializeField] Image imageColor;
    bool isActive = false;
    int index;


    System.Action<int, bool, changePermsPrefab_Script> callBack;
    public void setup(string text, bool value, int index, System.Action<int, bool, changePermsPrefab_Script> callback)
    {
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAA" + (callback == null));
        this.index = index;
        nameText.text = text;
        this.callBack = callback;
        //Debug.Log("LOLOL:" + (callback == null) + ":" + (this.callBack == null));
        isActive = value;
        reflectColor();
    }

    private void Start()
    {
        editPermsButton.onClick.AddListener(() =>
        {
            HandleButtonClick();
        });
    }

    void changeColorToRed()
    {
        imageColor.color = Color.red;
    }

    void changeColorToGreen()
    {
        imageColor.color= Color.green;
    }

    public void reflectColor()
    {
        if (isActive)
        {
            changeColorToGreen();
        }
        else
        {
            changeColorToRed();
        }
    }

    public void HandleButtonClick()
    {
        //Debug.Log("Click!:" + isActive);
        isActive = !isActive;
        //Debug.Log("Click! 2:" + isActive);
        reflectColor();
        callBack?.Invoke(index, isActive, this);
    }

    public void setActiveUi()
    {
        isActive = true;
        reflectColor();
    }
}
