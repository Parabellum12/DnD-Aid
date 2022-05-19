using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TokenInfoHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> ActiveTokenUi;
    [SerializeField] List<GameObject> InactiveTokenUI;
    [SerializeField] General_UI_DropDown_Handler_Script tokenInfoDropDownHandler;
    [SerializeField] PhotonView gameView;


    [SerializeField] Button InitativeListButton;
    [SerializeField] Button RemoveTokenButton;
    [SerializeField] Button AddTokenButton;

    public TokenHandler_Script ActiveSelectedToken;

    private void Start()
    {
        SetUIToInactive();
        tokenInfoDropDownHandler.setUiPositions();
    }

    public void SetUIToInactive()
    {
        foreach (GameObject go in ActiveTokenUi)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in InactiveTokenUI)
        {
            go.SetActive(true);
        }
        tokenInfoDropDownHandler.setUiPositions();
    }

    private void Update()
    {
        if (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.EditInitiativeList))
        {
            InitativeListButton.interactable = true;
        }
        else
        {
            InitativeListButton.interactable = false;
        }

        if (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.RemoveTokens))
        {
            RemoveTokenButton.interactable = true;
        }
        else
        {
            RemoveTokenButton.interactable = false;
        }

        if (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.AddToken))
        {
            AddTokenButton.interactable = true;
        }
        else
        {
            AddTokenButton.interactable = false;
        }
    }



    public void SetUIToActive()
    {
        foreach (GameObject go in ActiveTokenUi)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in InactiveTokenUI)
        {
            go.SetActive(false);
        }
        tokenInfoDropDownHandler.setUiPositions();
        
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


    public void spawnTokenPush()
    {
        gameView.RPC("spawnTokenHandle", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void spawnTokenHandle(Photon.Realtime.Player RequestingPlayer)
    {
        Vector3 temp = Camera.main.transform.position;
        temp.z = -5;
        GameObject go = PhotonNetwork.InstantiateRoomObject("Token", temp, Quaternion.identity);
        TokenHandler_Script scr = go.GetComponent<TokenHandler_Script>();
        scr.TokenInfoHandler_Script = this;
        if (PhotonNetwork.LocalPlayer.Equals(RequestingPlayer))
        {
            scr.setInfoToThis();
        }
    }

    public void removeTokenPush()
    {
        gameView.RPC("removeTokenHandle", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void removeTokenHandle()
    {
        if (ActiveSelectedToken == null)
        {
            return;
        }
        ActiveSelectedToken.KILLME();
    }

    
}
