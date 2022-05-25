using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
        if (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.GlobalMoveTokens))
        {
            InteractButton.interactable = false;
            ButtonImage.color = Color.green;
        }
        else
        {
            InteractButton.interactable = GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms);
        }
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
