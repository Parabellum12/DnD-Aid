using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PermissionsSelector_Handler : MonoBehaviour
{
    [SerializeField] RectTransform contentTransform;
    [SerializeField] PhotonView photonView;
    [SerializeField] GameObject permPrefab;

    public void loadPlayerPerms()
    {

    }

    public void networkMyPermsPush()
    {
        photonView.RPC("networkMyPermsHandle", RpcTarget.All, PhotonNetwork.LocalPlayer, GlobalPermissionsHandler.returnPermissions());
    }


    [PunRPC]
    public void networkMyPermsHandle(Photon.Realtime.Player plr, bool[] perms)
    {

    }

}
