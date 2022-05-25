using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class tokenUiPlayerPerms_Handler : MonoBehaviour
{
    [SerializeField] GameObject tokenPermChangeUIPrefab;

    List<tokenPlayerPermUiInteraction_Handler> interactionHandlers = new List<tokenPlayerPermUiInteraction_Handler>();
    [SerializeField] PhotonView gameView;
    [SerializeField] MainGame_Handler_Script mainHandler;
    [SerializeField] General_UI_DropDown_Handler_Script contentHandler;
    [SerializeField] GameObject ChildGO;

    Dictionary<Photon.Realtime.Player, bool> playToMove = new Dictionary<Photon.Realtime.Player, bool>();


    public void setPlayerPermUIUp(List<Photon.Realtime.Player> moveplrs, System.Action<Photon.Realtime.Player, bool> callback)
    {
        deleteAnyUI();
        foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerList)
        {
            GameObject go = Instantiate(tokenPermChangeUIPrefab, ChildGO.transform);
            tokenPlayerPermUiInteraction_Handler scr = go.GetComponent<tokenPlayerPermUiInteraction_Handler>();
            interactionHandlers.Add(scr);
            contentHandler.addToChildDropDowns(go.GetComponent<General_UI_DropDown_Handler_Script>());
            scr.setup(plr, mainHandler.returnPlayerPerms(plr)[(int)GlobalPermissionsHandler.PermisionNameToValue.GlobalMoveTokens] || moveplrs.Contains(plr), (plr, changedValue) =>
            {
                callback.Invoke(plr, changedValue);
            });
        }
        contentHandler.setUiPositions();
    }


    void deleteAnyUI()
    {
        foreach (tokenPlayerPermUiInteraction_Handler handler in interactionHandlers)
        {
            handler.KILLME();
        }
        interactionHandlers.Clear();
        contentHandler.clearChildDropDowns();
    }
}
