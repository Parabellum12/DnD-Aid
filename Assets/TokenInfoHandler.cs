using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class TokenInfoHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> ActiveTokenUi;
    [SerializeField] List<GameObject> InactiveTokenUI;
    [SerializeField] General_UI_DropDown_Handler_Script tokenInfoDropDownHandler;
    [SerializeField] PhotonView gameView;


    [SerializeField] Button InitativeListButton;
    [SerializeField] Button RemoveTokenButton;
    [SerializeField] Button AddTokenButton;
    [SerializeField] TMP_InputField tokenNameInput;
    public TokenHandler_Script ActiveSelectedToken;

    private void Start()
    {
        SetUIToInactive();
        tokenInfoDropDownHandler.setUiPositions();
    }

    [SerializeField] General_2D_Camera_Handler_Script camMoveHandle;

    bool changingName = false;
    public void onInputSelected()
    {
        changingName = true;
        camMoveHandle.WASDPan = false;
        camMoveHandle.ArrowKeyPan = false;
    }
    public void onInputUnSelected()
    {
        changingName = false;
        camMoveHandle.WASDPan = true;
        camMoveHandle.ArrowKeyPan = true;
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

        if (ActiveSelectedToken != null)
        {
            if (!changingName)
            {
                tokenNameInput.text = ActiveSelectedToken.tokenName;
            }

            if (!ActiveSelectedToken.inInitiativeList)
            {
                initiativeListButtonText.text = "Add To Initiative List";
            }
            else
            {
                initiativeListButtonText.text = "Remove From Initiative List";
            }
        }
    }


    [SerializeField] tokenUiPlayerPerms_Handler tokenUiPermsHandler;
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
            tokenNameInput.text = scr.tokenName;
            tokenUiPermsHandler.setPlayerPermUIUp(ActiveSelectedToken.getMoveAllowedPlayers(), (plr, value) =>
            {
                ActiveSelectedToken.changePlayerMovePerm(plr, value, false);
            });
            SetUIToActive();
        }
        else
        {
            SetUIToInactive();
        }
    }


    [SerializeField] InitiativeList_Handler initiativeListHandler;
    [SerializeField] TMP_Text initiativeListButtonText;
    public void addTokenToInitiativeList()
    {
        if (ActiveSelectedToken.inInitiativeList)
        {
            initiativeListHandler.removeUiTokenElement(ActiveSelectedToken);
        }
        else
        {
            initiativeListHandler.addTokenUiElement(ActiveSelectedToken);
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
        scr.setID(getID(), false);
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
        usedIDs.Remove(ActiveSelectedToken.tokenId);
        ActiveSelectedToken.KILLME();
    }

    public void setTokenName()
    {
        if (ActiveSelectedToken == null)
        {
            return;
        }
        ActiveSelectedToken.setName(tokenNameInput.text, false);
    }



    List<long> usedIDs = new List<long>();
   
    long getID()
    {
        long test = 0;
        while (usedIDs.Contains(test))
        {
            test++;
        }
        usedIDs.Add(test);
        return test;
    }
    
}
