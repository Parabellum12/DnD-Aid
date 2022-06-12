using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class TokenInfoHandler : MonoBehaviourPunCallbacks
{
    /*
     * handles the ui for the token showing info on it
     */
    [SerializeField] List<GameObject> ActiveTokenUi;
    [SerializeField] List<GameObject> InactiveTokenUI;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 tokenInfoDropDownHandler;
    [SerializeField] PhotonView gameView;


    [SerializeField] Button InitativeListButton;
    [SerializeField] Button RemoveTokenButton;
    [SerializeField] Button AddTokenButton;
    [SerializeField] Button AddTokenFromMenuButton;
    [SerializeField] TMP_InputField tokenNameInput;
    public TokenHandler_Script ActiveSelectedToken;

    private void Start()
    {
        SetUIToActive();
        tokenInfoDropDownHandler.setUIPositionsNoCallback();
        tokenInfoDropDownHandler.setUIPositions();
        SetUIToInactive();
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
        tokenInfoDropDownHandler.setUIPositions();
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
            AddTokenFromMenuButton.interactable = true;
        }
        else
        {
            AddTokenButton.interactable = false;
            AddTokenFromMenuButton.interactable = false;
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
        tokenInfoDropDownHandler.setUIPositions();


    }

    public bool IsLocked = false;

    public void setActiveSelected(TokenHandler_Script scr)
    {
        if (IsLocked)
        {
            return;
        }
        ActiveSelectedToken = scr;
        Debug.Log("setActiveSelected:" + (scr != null));
        if (scr != null)
        {
            tokenNameInput.text = scr.tokenName;
            tokenUiPermsHandler.setPlayerPermUIUp(ActiveSelectedToken.getMoveAllowedPlayers(), (plr, value) =>
            {
                ActiveSelectedToken.changePlayerMovePerm(plr, value, false);
                updateUI(false);
            });
            SetUIToActive();
        }
        else
        {
            SetUIToInactive();
        }
    }

    [PunRPC]
    public void updateUI(bool networkedCall)
    {
        Debug.Log("Handling updateUI");
        if (!networkedCall)
        {
            gameView.RPC("updateUI", RpcTarget.Others, true);
        }
        if (ActiveSelectedToken != null)
        {
            tokenUiPermsHandler.setPlayerPermUIUp(ActiveSelectedToken.getMoveAllowedPlayers(), (plr, value) =>
            {
                ActiveSelectedToken.changePlayerMovePerm(plr, value, false);
                updateUI(false);
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
    public void updateTokenInitiativeList()
    {
        if (ActiveSelectedToken.inInitiativeList)
        {
            ActiveSelectedToken.removeMeFromInitiativeList(false);
            //initiativeListHandler.removeUiTokenElement(ActiveSelectedToken);
        }
        else
        {
            ActiveSelectedToken.addMeToInitiativeList(false);
            //initiativeListHandler.addTokenUiElement(ActiveSelectedToken);
        }
    }

    public void handleInitaitveListUpdatePush()
    {
        ActiveSelectedToken.UpdateInitiativeList();
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
        scr.setInfoToThis(RequestingPlayer, false);
    }

    public void removeTokenPush()
    {
        ActiveSelectedToken.KILLME(false);
    }

    [PunRPC]
    public void removeTokenHandle(TokenHandler_Script token)
    {
        usedIDs.Remove(token.tokenId);
        initiativeListHandler.removeUiTokenElement(token);
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




    /*
     * token sprite mask handling
     */
    public List<Canvas> TokenCanvases = new List<Canvas>();
    public List<SpriteMask> TokenSpriteMasks = new List<SpriteMask>();
    int tokenSpriteMaskSortingIndex = 0;

    public void setUpMaskingData()
    {
        tokenSpriteMaskSortingIndex = 0;
        for (int i = 0; i < TokenCanvases.Count; i++)
        {
            TokenSpriteMasks[i].frontSortingOrder = tokenSpriteMaskSortingIndex;
            TokenCanvases[i].sortingOrder = tokenSpriteMaskSortingIndex;
            TokenSpriteMasks[i].backSortingOrder = tokenSpriteMaskSortingIndex+1;
            tokenSpriteMaskSortingIndex += 2;
        }
    }
    
}
