using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenInfoHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> ActiveTokenUi;
    [SerializeField] List<GameObject> InactiveTokenUI;
    [SerializeField] General_UI_DropDown_Handler_Script tokenInfoDropDownHandler;



    TokenHandler_Script ActiveSelectedToken;

    private void Start()
    {
        SetUIToInactive();
        SetUIToActive();
        tokenInfoDropDownHandler.setUiPositions();
    }

    void SetUIToInactive()
    {
        foreach (GameObject go in ActiveTokenUi)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in InactiveTokenUI)
        {
            go.SetActive(true);
        }
    }





    void SetUIToActive()
    {
        foreach (GameObject go in ActiveTokenUi)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in InactiveTokenUI)
        {
            go.SetActive(false);
        }

    }

    public void setActiveSelected(TokenHandler_Script scr)
    {
        ActiveSelectedToken = scr;
        if (scr != null)
        {
            SetUIToActive();
        }
        else
        {
            SetUIToInactive();
        }
    }


}
