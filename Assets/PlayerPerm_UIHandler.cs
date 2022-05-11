using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerPerm_UIHandler : MonoBehaviour
{
    [SerializeField] Image backgorund;
    [SerializeField] Image permsBackgournd;
    [SerializeField] GameObject permsBackgourndGameObject;
    [SerializeField] GameObject PermsGameobject;
    [SerializeField] TMP_Text playerName;
    [SerializeField] List<changePermsPrefab_Script> uiElements;
    Photon.Realtime.Player plr;
    MainGame_Handler_Script gameHandler;

    System.Action updateUiPositions;

    System.Action<Photon.Realtime.Player, int, bool> OnPermChangeCallback;

    [SerializeField] Button showHidePermissionsButton;
    [SerializeField] Button KickPlayerButton;

    public void Start()
    {
        openClosePermissions();
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms))
        {
            showHidePermissionsButton.interactable = false;
        }
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.KickPlayers) || plr.Equals(PhotonNetwork.LocalPlayer))
        {
            KickPlayerButton.interactable = false;
        }
    }





    public void setup(ref Photon.Realtime.Player plr, bool[] permValues, System.Action<Photon.Realtime.Player, int, bool> OnPermChangeCallback, System.Action updateUiPositions)
    {
        this.updateUiPositions = updateUiPositions;
        //Debug.Log("DDDDDDDDDDDDDDDDDDD:" + (OnPermChangeCallback == null));
        this.OnPermChangeCallback = OnPermChangeCallback;
        gameHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        playerName.text = plr.NickName;
        this.plr = plr;
        setValues(permValues);
    }

    public void setValues(Photon.Realtime.Player plr, bool[] perms)
    {
        if (this.plr.Equals(plr))
        {
            setValues(perms);
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
            uiElements[i].setup(GlobalPermissionsHandler.getPermFromIndex(i).ToString(), perms[i], i, (index, value) =>
            {
                updatePlayerPermsPush(index, value);
            });
        }
    }

    public float getSize()
    {
        float size = backgorund.rectTransform.rect.height;
        if (PermsGameobject.activeSelf)
        {
            size += permsBackgournd.rectTransform.rect.height;
        }
        return size;
    }

    void updatePlayerPermsPush(int index, bool value)
    {
        OnPermChangeCallback.Invoke(plr, index, value);
    }


    public void openClosePermissions()
    {
        if (permsBackgourndGameObject.activeSelf)
        {
            permsBackgourndGameObject.SetActive(false);
        }
        else
        {
            permsBackgourndGameObject.SetActive(true);
        }
        updateUiPositions.Invoke();
    }


}
