using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerPerm_UIHandler : MonoBehaviour
{
    /*
     * handles changing player perms ui
     */
    [SerializeField] TMP_Text playerName;
    [SerializeField] List<changePermsPrefab_Script> uiElements;
    Photon.Realtime.Player plr;
    MainGame_Handler_Script gameHandler;


    System.Action<Photon.Realtime.Player, int, bool> OnPermChangeCallback;

    [SerializeField] Button showHidePermissionsButton;
    [SerializeField] Button KickPlayerButton;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 permissionsUIDropDownHandler;

    public void Start()
    {
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms))
        {
            showHidePermissionsButton.interactable = false;
        }
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.KickPlayers) || plr.Equals(PhotonNetwork.LocalPlayer))
        {
            KickPlayerButton.interactable = false;
        }
    }





    public void setup(ref Photon.Realtime.Player plr, bool[] permValues, System.Action<Photon.Realtime.Player, int, bool> OnPermChangeCallback, System.Action<Photon.Realtime.Player> kickPlayerCallback)
    {
        //this.updateUiPositions = updateUiPositions;
        //Debug.Log("DDDDDDDDDDDDDDDDDDD:" + (OnPermChangeCallback == null));
        this.OnPermChangeCallback = OnPermChangeCallback;
        gameHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        playerName.text = plr.NickName;
        if (plr.Equals(PhotonNetwork.LocalPlayer))
        {
            playerName.text = plr.NickName + "-you";
        }
        else if (plr.Equals(PhotonNetwork.MasterClient))
        {
            playerName.text = plr.NickName + "-host";
        }
        this.plr = plr;
        setValues(permValues);
        KickPlayerButton.onClick.AddListener(() => { kickPlayerCallback.Invoke(this.plr); });
    }

    public void setValues(Photon.Realtime.Player plr, bool[] perms)
    {
        if (this.plr.Equals(plr))
        {
            setValues(perms);
        }
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms))
        {
            showHidePermissionsButton.interactable = false;
            permissionsUIDropDownHandler.setDropDownToInactive();
            permissionsUIDropDownHandler.setUIPositions();
        }
        else
        {
            showHidePermissionsButton.interactable = true;
        }
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.KickPlayers) || this.plr.Equals(PhotonNetwork.LocalPlayer))
        {
            KickPlayerButton.interactable = false;
        }
        else
        {
            KickPlayerButton.interactable = true;
        }
    }

    void setValues(bool[] perms)
    {
        if (perms == null)
        {
            //Debug.Log("setValues(bool[]) perms is null");
            return;
        }
        for (int i = 0; i < uiElements.Count; i++)
        {
            //Debug.Log("whatHappened:" + i);
            //Debug.Log(uiElements.Count);
            //Debug.Log(perms.Length);
            int index = i;
            uiElements[i].setup(GlobalPermissionsHandler.getPermFromIndex(i).ToString(), perms[i], i, (index, value, scr) =>
            {
                if (GlobalPermissionsHandler.getPermFromIndex(index) == GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms && plr.Equals(PhotonNetwork.LocalPlayer) || plr.Equals(PhotonNetwork.MasterClient))
                {
                    scr.setActiveUi();
                    uiElements[index].setActiveToTrue();
                    return;
                }
                updatePlayerPermsPush(index, value);
            });
        }
    }

    void updatePlayerPermsPush(int index, bool value)
    {
        Debug.Log("Update Player Perms:" + GlobalPermissionsHandler.getPermFromIndex(index) + " To:" + value);
        OnPermChangeCallback.Invoke(plr, index, value);
    }


}
