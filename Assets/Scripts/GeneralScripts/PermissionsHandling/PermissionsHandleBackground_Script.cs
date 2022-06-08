using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PermissionsHandleBackground_Script : MonoBehaviour
{
    /*
     * REPLACED
     * handles permission changing ui
     */
    /*
    [SerializeField] GameObject permissionsHandlerPrefab;
    //[SerializeField] RectTransform contentViewPort;
    [SerializeField] PhotonView photonview;
    List<PlayerPerm_UIHandler> playerPermUiList = new List<PlayerPerm_UIHandler>();
    MainGame_Handler_Script mainHandler;

    [SerializeField] General_UI_DropDown_Handler_ScriptV2 contentUiHandler;

    public void Start()
    {
        mainHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        CreateUi();
    }
    public void CreateUi()
    {
        destroyAnyUi();
        Photon.Realtime.Player[] allPlayers = PhotonNetwork.PlayerList;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            GameObject go = Instantiate(permissionsHandlerPrefab, contentUiHandler.transform);
            PlayerPerm_UIHandler scr = go.GetComponent<PlayerPerm_UIHandler>();
            General_UI_DropDown_Handler_ScriptV2 Uiscr = go.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
            playerPermUiList.Add(scr);
            contentUiHandler.addToChildDropDowns(Uiscr);
            //Debug.Log("hello world pain:" + i);
            scr.setup(ref allPlayers[i], mainHandler.returnPlayerPerms(allPlayers[i]), (plr, index, value) =>
            {
                //Debug.Log("CHangePerm: "+ i  + ":" + index + ":" + value);
                if (plr.Equals(PhotonNetwork.LocalPlayer))
                {
                    if (GlobalPermissionsHandler.getPermFromIndex(index).Equals(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms))
                    {
                        Debug.Log("Player Tried To Change Own ChangeOtherPlayerPerms Permission");
                        return;
                    }
                }
                mainHandler.changePlayersPerms(plr, index, value);
            }, () =>
            {
                updateUiPos();
            }, (plrToKick) =>
            {
                mainHandler.kickPlayerPush(plrToKick);
            });
        }
        contentUiHandler.setUIPositions();
    }

    void updateUiPos()
    {
        contentUiHandler.setUIPositions();
    }

    public void updateUi(Photon.Realtime.Player plr, bool[] perms)
    {
        foreach (PlayerPerm_UIHandler scr in playerPermUiList)
        {
            scr.setValues(plr, perms);
        }
    }

    void destroyAnyUi()
    {
        foreach (PlayerPerm_UIHandler scr in playerPermUiList)
        {
            contentUiHandler.removeFromChildDropDowns(scr.gameObject.GetComponent<General_UI_DropDown_Handler_ScriptV2>());
            Destroy(scr.gameObject);
        }

        playerPermUiList.Clear();
    }
    */
}
