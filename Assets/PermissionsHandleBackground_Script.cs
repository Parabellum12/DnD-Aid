using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PermissionsHandleBackground_Script : MonoBehaviour
{
    [SerializeField] GameObject permissionsHandlerPrefab;
    [SerializeField] RectTransform contentViewPort;
    [SerializeField] PhotonView photonview;
    List<PlayerPerm_UIHandler> playerPermUiList = new List<PlayerPerm_UIHandler>();
    MainGame_Handler_Script mainHandler;

    public void Start()
    {
        mainHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        CreateUi();
    }
    public void CreateUi()
    {
        destroyAnyUi();
        Photon.Realtime.Player[] allPlayers =  PhotonNetwork.PlayerList;
        float VerticalOffset = 0;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            GameObject go = Instantiate(permissionsHandlerPrefab, contentViewPort);
            go.transform.localPosition = new Vector2(0, -VerticalOffset - 75);
            PlayerPerm_UIHandler scr = go.GetComponent<PlayerPerm_UIHandler>();
            scr.setup(ref allPlayers[i], mainHandler.returnPlayerPerms(allPlayers[i]), (index, value) =>
            {
                mainHandler.changePlayersPerms(allPlayers[i], index, value);
            });
            VerticalOffset += scr.getSize();
        }
    }

    public void updateUi(Photon.Realtime.Player plr, bool[] perms)
    {
        foreach(PlayerPerm_UIHandler scr in playerPermUiList)
        {
            scr.setValues(plr, perms);
        }
    }

    void destroyAnyUi()
    {
        foreach (PlayerPerm_UIHandler scr in playerPermUiList)
        {
            Destroy(scr.gameObject);
        }
    }
}
