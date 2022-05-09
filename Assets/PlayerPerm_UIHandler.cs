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
    [SerializeField] GameObject PermsGameobject;
    [SerializeField] TMP_Text playerName;
    [SerializeField] List<changePermsPrefab_Script> uiElements;
    Photon.Realtime.Player plr;
    PhotonView photonView;

    public void setup(PhotonView photonView, Photon.Realtime.Player plr)
    {
        playerName.text = plr.NickName;
        this.photonView = photonView;
        this.plr = plr;
        //requestPermArray();
    }

    void requestPermArray()
    {
        photonView.RPC("HandleRequestPermArray", plr, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void HandleRequestPermArray(Photon.Realtime.Player plr)
    {
        photonView.RPC("ReturnRequestPermArray", plr, GlobalPermissionsHandler.returnPermissions());
    }

    [PunRPC]
    public void ReturnRequestPermArray(bool[] perms)
    {
        setvalues(perms);
    }

    void setvalues(bool[] perms)
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
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
        photonView.RPC("updatePlayerPermsHandle", plr, index, value);
    }

    [PunRPC]
    public void updatePlayerPermsHandle(int index, bool value)
    {
        GlobalPermissionsHandler.setPermAs(GlobalPermissionsHandler.getPermFromIndex(index), value);
    }

    void callPermValueUpdate()
    {
        photonView.RPC("HandlePermValueUpdate", RpcTarget.Others, plr);
    }

    [PunRPC]
    public void HandlePermValueUpdate(Photon.Realtime.Player callingPlayer)
    {
        if (plr.Equals(callingPlayer))
        {
            requestPermArray();
        }
    }


}
