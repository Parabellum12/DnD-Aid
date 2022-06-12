using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class tokenPlayerPermUiInteraction_Handler : MonoBehaviour
{
    /*
     * handles the specific players ability to move token
     */
    [SerializeField] Button InteractButton;
    [SerializeField] Image ButtonImage;
    [SerializeField] TMP_Text playerName;

    Photon.Realtime.Player referencePlayer;
    bool canMove;
    System.Action<Photon.Realtime.Player, bool> OnValueChangedCallback;
    MainGame_Handler_Script mainHandler;

    private void Awake()
    {
        mainHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
    }

    private void Start()
    {
        InteractButton.onClick.AddListener(() => { handleClick(); });
    }

    public void setup(Photon.Realtime.Player plr, bool canMove, System.Action<Photon.Realtime.Player, bool> OnValueChangedCallback)
    {
        this.OnValueChangedCallback = OnValueChangedCallback;
        referencePlayer = plr;
        this.canMove = canMove;
        playerName.text = referencePlayer.NickName;
        if (referencePlayer.Equals(PhotonNetwork.LocalPlayer))
        {
            playerName.text += "-you";
        }
        else if (referencePlayer.Equals(PhotonNetwork.MasterClient))
        {
            playerName.text += "-host";
        }

        if (canMove)
        {
            ButtonImage.color = Color.green;
        }
        else
        {
            ButtonImage.color = Color.red;
        }
    }

    private void Update()
    {
        if (mainHandler == null)
        {
            mainHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        }

        if (plrListContains(referencePlayer))
        {


            if (mainHandler.returnPlayerPerms(referencePlayer)[(int)GlobalPermissionsHandler.PermisionNameToValue.GlobalMoveTokens])
            {
                InteractButton.interactable = false;
                ButtonImage.color = Color.green;
            }
            else
            {
                InteractButton.interactable = GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms);
            }
        }
        else
        {
            //call error
        }
    }

    bool plrListContains(Photon.Realtime.Player plr)
    {
        foreach (Photon.Realtime.Player curPlr in PhotonNetwork.PlayerList)
        {
            if (curPlr.Equals(plr))
            {
                return true;
            }
        }
        return false;
    }

    public void handleClick()
    {
        canMove = !canMove;
        if (canMove)
        {
            ButtonImage.color = Color.green;
        }
        else
        {
            ButtonImage.color= Color.red;
        }
        Debug.Log("Changed:" + referencePlayer.NickName + " To:" + canMove);
        OnValueChangedCallback.Invoke(referencePlayer, canMove);

    }

    public void KILLME()
    {
        Debug.Log("KILLME DO IT NOW");
        Destroy(gameObject);
    }
}
