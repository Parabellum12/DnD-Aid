using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class TokenHandler_Script : MonoBehaviourPunCallbacks
{
    /*
     * handles the code for the tokens
     */
    [SerializeField] Image TokenPfp;
    [SerializeField] TMP_Text tokenNameText;
    public TokenInfoHandler TokenInfoHandler_Script = null;
    public InitiativeList_Handler InitiativeListHandler_Script = null;
    [SerializeField] PhotonView localView;
    List<Photon.Realtime.Player> MoveAllowedPlayers = new List<Photon.Realtime.Player>();
    public string tokenName;
    public long tokenId;
    public bool inInitiativeList = false;
    public int initiativeValue = 0;
    public Canvas canvas;
    public SpriteMask spriteMask;


    private void Start()
    {
         
        setupMe();
        TokenInfoHandler_Script.TokenCanvases.Add(canvas);
        TokenInfoHandler_Script.TokenSpriteMasks.Add(spriteMask);
        TokenInfoHandler_Script.setUpMaskingData();
    }

    bool alreadySetup = false;

    void setupMe()
    {
        if (alreadySetup)
        {
            return;
        }
         
        MainGame_Handler_Script mainscr= GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        TokenInfoHandler_Script = mainscr.GetTokenInfoHandler();
        InitiativeListHandler_Script = mainscr.GetInitiativeList_Handler();
        if (InitiativeListHandler_Script != null && TokenInfoHandler_Script != null)
        {
            alreadySetup = true;
        }
    }


    public void setTokenPFP(Texture2D tex)
    {
         
        setupMe();
        TokenPfp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        localView.RPC("setTokenPFP", RpcTarget.Others, UtilClass.ObjectToByteArray(new Wrapper(tex)));
    }

    [PunRPC]
    public void setTokenPFP(byte[] data)
    {
         
        Texture2D tex = UtilClass.ByteArrayToObject<Wrapper>(data).tex;
        setupMe();
        TokenPfp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2)); ;
    }

    class Wrapper
    {
        public Texture2D tex;

        public Wrapper(Texture2D tex)
        {
            this.tex = tex;
        }
    }

    public void setInfoToThis()
    {
         
        setupMe();
        TokenInfoHandler_Script.setActiveSelected(this);
    }

    [PunRPC]
    public void setInfoToThis(Photon.Realtime.Player requestingPlayer, bool networkCall)
    {
         
        if (!networkCall)
        {
            localView.RPC("setInfoToThis", RpcTarget.Others, requestingPlayer, true);
        }
        if (!requestingPlayer.Equals(PhotonNetwork.LocalPlayer))
        {
            return;
        }
        setupMe();
        TokenInfoHandler_Script.setActiveSelected(this);
    }

    [PunRPC]
    public void setInitiativeValue(int value, bool networkCall)
    {
         
        initiativeValue = value;
        if (!networkCall)
        {
            localView.RPC("setInitiativeValue", RpcTarget.Others, value, true);
        }
    }

    [PunRPC]
    public void addMeToInitiativeList(bool networkCall)
    {
         
        setupMe();
        if (InitiativeListHandler_Script == null)
        {
            TokenInfoHandler_Script = GameObject.FindGameObjectWithTag("TokenUIHandler").GetComponent<TokenInfoHandler>();
        }
        InitiativeListHandler_Script.addTokenUiElement(this);
        inInitiativeList = true;
        if (!networkCall)
        {
            localView.RPC("addMeToInitiativeList", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void removeMeFromInitiativeList(bool networkCall)
    {
         
        setupMe();
        InitiativeListHandler_Script.removeUiTokenElementCallFromToken(this);
        inInitiativeList = false;
        if (!networkCall)
        {
            localView.RPC("removeMeFromInitiativeList", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void KILLME(bool networkCall)
    {
         
        setupMe();
        if (!networkCall)
        {
            localView.RPC("KILLME", RpcTarget.All, true);
        }
        else
        {

            TokenInfoHandler_Script.TokenCanvases.Remove(canvas);
            TokenInfoHandler_Script.TokenSpriteMasks.Remove(spriteMask);
            TokenInfoHandler_Script.setUpMaskingData();
            if (inInitiativeList)
            {
                removeMeFromInitiativeList(true);
            }
            localView.RPC("UpdateUiIfSelectedKilled", RpcTarget.All);
            TokenInfoHandler_Script.removeTokenHandle(this);
        }


    }






    [PunRPC]
    public void setName(string name, bool NetworkedCall)
    {
         
        setupMe();
        tokenName = name;
        tokenNameText.text = name;
        if (!NetworkedCall)
        {
            localView.RPC("setName", RpcTarget.Others, name, true);
        }
    }

    [PunRPC]
    public void setID(long id, bool NetworkedCall)
    {
         
        setupMe();
        tokenId = id;
        if (!NetworkedCall)
        {
            localView.RPC("setID", RpcTarget.Others, id, true);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
         
        setupMe();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        localView.RPC("setName", newPlayer, tokenName, true);
        localView.RPC("setID", newPlayer, tokenId, true);
        localView.RPC("setInitiativeValue", newPlayer, initiativeValue, true);
        if (PhotonNetwork.LocalPlayer.Equals(newPlayer))
        {
            addMeToInitiativeList(false);
        }
    }


    [PunRPC]
    public void UpdateInitiativeList()
    {
         
        setupMe();
        TokenHandler_Script temp = TokenInfoHandler_Script.ActiveSelectedToken;
        TokenInfoHandler_Script.updateTokenInitiativeList();
        TokenInfoHandler_Script.ActiveSelectedToken = temp;
        localView.RPC("UpdateInitiativeList", RpcTarget.Others);
    }


    public void addPlayerToMoveListPush()
    {
         
        localView.RPC("addPlayerToMoveListHandle", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void addPlayerToMoveListHandle(Photon.Realtime.Player plr)
    {
         
        setupMe();
        MoveAllowedPlayers.Add(plr);
    }

    public void removePlayerToMoveListPush()
    {
         
        localView.RPC("removePlayerToMoveListHandle", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void removePlayerToMoveListHandle(Photon.Realtime.Player plr)
    {
         
        MoveAllowedPlayers.Remove(plr);
    }


    [PunRPC]
    public void changePlayerMovePerm(Photon.Realtime.Player plr, bool value, bool networkedCall)
    {
         
        if (!networkedCall)
        {
            localView.RPC("changePlayerMovePerm", RpcTarget.Others, plr, value, true);
        }
        setupMe();
        if (value)
        {
            if (!MoveAllowedPlayers.Contains(plr))
            {
                MoveAllowedPlayers.Add(plr);
            }
        }
        else
        {
            if (MoveAllowedPlayers.Contains(plr))
            {
                MoveAllowedPlayers.Remove(plr);
            }
        }
    }




    [PunRPC]
    public void UpdateUiIfSelectedKilled()
    {
         
        setupMe(); ;
        if (TokenInfoHandler_Script != null && this != null && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            TokenInfoHandler_Script.ActiveSelectedToken = null;
            TokenInfoHandler_Script.SetUIToInactive();
        }
        if (this.gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject.GetPhotonView());
        }
    }

    public bool mouseOver = false;
    bool moving = false;
    private void OnMouseEnter()
    {
         
        mouseOver = true;
    }

    private void OnMouseExit()
    {
         
        if (moving || clickedOn)
        {
            return;
        }
        mouseOver = false;
    }

    Vector2 mouseInitialPos = Vector2.zero;
    bool firstClick = true;
    bool allowed = false;
    bool clickedOn = false;


    private void Update()
    {
        setupMe();
        if (UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
        {
            return;
        }

        if (Input.GetMouseButton(0) && mouseOver && TokenInfoHandler_Script.ActiveSelectedToken != this && !moving)
        {
            TokenInfoHandler_Script.setActiveSelected(this);
        }
        else if (Input.GetMouseButton(0) && !mouseOver && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            TokenInfoHandler_Script.setActiveSelected(null);
        }

        if (Input.GetMouseButtonDown(0) && mouseOver && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            clickedOn = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickedOn = false;
        }

        if (Input.GetMouseButton(0) && mouseOver && TokenInfoHandler_Script.ActiveSelectedToken == this && (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.GlobalMoveTokens) || MoveAllowedPlayers.Contains(PhotonNetwork.LocalPlayer)))
        {
            if (firstClick)
            {
                Debug.LogWarning("Want Ownership");
                mouseInitialPos = UtilClass.getMouseWorldPosition();
                localView.TransferOwnership(PhotonNetwork.LocalPlayer);
                firstClick = false;
            }
            else if (!mouseInitialPos.AlmostEquals(new Vector2(UtilClass.getMouseWorldPosition().x, UtilClass.getMouseWorldPosition().y), 1) || allowed)
            {
                moving = true;
                allowed = true;
                Vector3 temp = UtilClass.getMouseWorldPosition();
                temp.z = -5;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.position = new Vector3(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y), temp.z);
                }
                else
                {
                    transform.position = temp;
                }
            }
        }
        else
        {
            if (!firstClick)
            {
                firstClick = true;
            }
            allowed = false;
            moving = false;
        }

    }



    public List<Photon.Realtime.Player> getMoveAllowedPlayers()
    {
        return MoveAllowedPlayers;
    }


}
