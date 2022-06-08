using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class permissionsUiHandler : MonoBehaviour
{
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 contentDropdownHandler;
    [SerializeField] GameObject PlayerPermUiPrefab;
    [SerializeField] MainGame_Handler_Script mainHandler;
    List<PlayerPerm_UIHandler> uiList = new List<PlayerPerm_UIHandler>();

    private void Start()
    {
        createUI();
    }


    public void createUI()
    {
        deleteAnyUI();
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            GameObject go = Instantiate(PlayerPermUiPrefab, contentDropdownHandler.gameObject.transform);
            General_UI_DropDown_Handler_ScriptV2 dropdownScr = go.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
            contentDropdownHandler.addToChildDropDowns(dropdownScr);
            PlayerPerm_UIHandler scr = go.GetComponent<PlayerPerm_UIHandler>();
            uiList.Add(scr);
            Photon.Realtime.Player refPlayer = players[i];
            scr.setup(ref refPlayer, mainHandler.returnPlayerPerms(players[i]), (plr, index, value) =>
            {
                mainHandler.changePlayersPerms(plr, index, value);
            }, (plrToKick) =>
            {
                mainHandler.kickPlayerPush(plrToKick);
            });
        }
        contentDropdownHandler.setUIPositions();
    }

    public void updateUi(Photon.Realtime.Player plr, bool[] values)
    {
        foreach (PlayerPerm_UIHandler scr in uiList)
        {
            scr.setValues(plr, values);
        }
    }


    void deleteAnyUI()
    {
        foreach (PlayerPerm_UIHandler scr in uiList)
        {
            Destroy(scr.gameObject);
        }
        uiList.Clear();
        contentDropdownHandler.clearChildDropDowns();
    }
}
