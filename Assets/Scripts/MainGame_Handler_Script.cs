using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;

public class MainGame_Handler_Script : MonoBehaviourPunCallbacks
{
    [SerializeField] SaveLoad_Handler_Script SaveLoadHandler;
    [SerializeField] PhotonView localView;
    List<SaveLoad_Handler_Script.saveClass> GlobalCachedMaps = new List<SaveLoad_Handler_Script.saveClass>();
    [SerializeField] General_UI_DropDown_Handler_Script generalUiDropdownMainScr;
    [SerializeField] TMP_Text gameCodeText;

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        gameCodeText.text = "Game Code:"+PhotonNetwork.CurrentRoom.Name;
        if (!PhotonNetwork.IsMasterClient)
        {
            //client
            Debug.Log("CallRequestMapDataSyncPush");
            CallRequestMapDataSyncPush();
        }
        else
        {
            //master client
            callForPlayerPermsUpdate();
        }
    }







    bool firstUpdateLoop = true;
    public void Update()
    {
        if (firstUpdateLoop)
        {
            firstUpdateLoop = false;
            generalUiDropdownMainScr.setUiPositionsNoCallback();
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log("testWhy1");
        callAllPlayerPermUpdate();
        //Debug.Log("testWhy2");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log("testWhy1");
        callAllPlayerPermUpdate();
        //Debug.Log("testWhy2");
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("TitleScreen");
    }



    //map loading and cache sharing
    public void addMapToGlobalCachePush(string FileName)
    {
        if (doesGlobalCacheContain(FileName))
        {
            return;
        }
        Debug.Log("add to cache:" + FileName);
        SaveLoad_Handler_Script.saveClass temp = SaveLoadHandler.getMapData(FileName);
        localView.RPC("addMapToGLobalCacheHandle", RpcTarget.All, UtilClass.ObjectToByteArray(temp));
    }

    [PunRPC]
    public void addMapToGLobalCacheHandle(byte[] mapData)
    {
        GlobalCachedMaps.Add(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData));
    }

    
    public void removeMapFromGlobalCachePush(string FileName)
    {
        if (!doesGlobalCacheContain(FileName))
        {
            return;
        }
        Debug.Log("remove to cache:" + FileName);
        SaveLoad_Handler_Script.saveClass temp = SaveLoadHandler.getMapData(FileName);
        localView.RPC("removeMapFromGLobalCacheHandle", RpcTarget.All, UtilClass.ObjectToByteArray(temp));
    }

    [PunRPC]
    public void removeMapFromGLobalCacheHandle(byte[] mapData)
    {
        SaveLoad_Handler_Script.saveClass test = UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData);
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapName.Equals(test.MapName))
            {
                GlobalCachedMaps.Remove(sc);
                break;
            }
        }
    }

    public void CallRequestMapDataSyncPush()
    {
        localView.RPC("RequestMapDataSyncPush", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void RequestMapDataSyncPush(Photon.Realtime.Player plr)
    {
        localView.RPC("RequestMapDataSyncHandle", plr, returnGlobalCacheAsByteArray(), UtilClass.ObjectToByteArray(SaveLoadHandler.getMapData(SaveLoadHandler.getCurrentlyLoadedMapName())));
    }

    [PunRPC]
    public void RequestMapDataSyncHandle(byte[] allData, byte[] currentlyLoadedMap)
    {
        Debug.Log("RequestMapDataSyncHandle");
        GlobalCachedMaps.Clear();
        GlobalCachedMaps.AddRange(returnGlobalCacheByte2DArrayToList(allData));
        SaveLoadHandler.loadMap(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(currentlyLoadedMap));
    }


    public void LoadMapDataPush(string map)
    {
        if (doesGlobalCacheContain(map))
        {
            localView.RPC("LoadMapDataHandle", RpcTarget.All, map);
        }
        else 
        {
            addMapToGlobalCachePush(map);
            localView.RPC("LoadMapDataHandle", RpcTarget.All, UtilClass.ObjectToByteArray(SaveLoadHandler.getMapData(map)));
        }
    }

    [PunRPC]
    public void LoadMapDataHandle(byte[] mapData)
    {
        Debug.Log("load Map from data");
        SaveLoadHandler.loadMap(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData));
    }
    [PunRPC]
    public void LoadMapDataHandle(string mapName)
    {
        Debug.Log("load map from cache");
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapName.Equals(mapName))
            {
                SaveLoadHandler.loadMap(sc);
            }
        }
    }

    bool doesGlobalCacheContain(string  mapName)
    {
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapName.Equals(mapName))
            {
                return true;
            }
        }
        return false;
    }





    //network room handling
    public void ToggleRoomJoinable(bool joinable)
    {
        PhotonNetwork.CurrentRoom.IsOpen = joinable;
    }

    byte[] returnGlobalCacheAsByteArray()
    {
        CachedMapsWrapper wrapper = new CachedMapsWrapper(GlobalCachedMaps.ToArray());
        return UtilClass.ObjectToByteArray(wrapper);
    }

    List<SaveLoad_Handler_Script.saveClass> returnGlobalCacheByte2DArrayToList(byte[] data)
    {
        CachedMapsWrapper wrapper = UtilClass.ByteArrayToObject<CachedMapsWrapper>(data);
        List<SaveLoad_Handler_Script.saveClass> returner = new List<SaveLoad_Handler_Script.saveClass>();
        returner.AddRange(wrapper.CachedMaps);
        return returner;
    }

    [System.Serializable]
    public class CachedMapsWrapper
    {
        public SaveLoad_Handler_Script.saveClass[] CachedMaps;

        public CachedMapsWrapper(SaveLoad_Handler_Script.saveClass[] dat)
        {
            CachedMaps = dat;
        }
    }



    [SerializeField]PermissionsHandleBackground_Script permUIHandler;
    Dictionary<Photon.Realtime.Player, bool[]> playerToPerms = new Dictionary<Player, bool[]>();

    public bool[] returnPlayerPerms(Photon.Realtime.Player plr)
    {
        return playerToPerms.GetValueOrDefault(plr);
    }

    public void callAllPlayerPermUpdate()
    {
        localView.RPC("callForPlayerPermsUpdate", RpcTarget.All);
    }

    [PunRPC]
    public void callForPlayerPermsUpdate()
    {
        playerToPerms.Clear();
        //Debug.Log("reset player perms");
        //playerToPerms.Add(PhotonNetwork.LocalPlayer, GlobalPermissionsHandler.returnPermissions());
        //permUIHandler.CreateUi();
        localView.RPC("RequestPlayerPermsPush", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void RequestPlayerPermsPush(Player plrToReturnDataTo)
    {
        localView.RPC("RequestPlayerPermsHandle", plrToReturnDataTo, PhotonNetwork.LocalPlayer, GlobalPermissionsHandler.returnPermissions());
    }

    [PunRPC]
    public void RequestPlayerPermsHandle(Player plr, bool[] perms)
    {
        Debug.Log("returned player perms");
        playerToPerms.Add(plr, perms);
        permUIHandler.CreateUi();
    }

    public void changePlayersPerms(Player plrToChangePermsOn, int index, bool value)
    {
        localView.RPC("changeThisPlayersPerms", plrToChangePermsOn, index, value);
    }

    [PunRPC]
    public void changeThisPlayersPerms(int index, bool value)
    {
        //Debug.Log("Change Perm:" + GlobalPermissionsHandler.getPermFromIndex(index) + " To: " + value);
        GlobalPermissionsHandler.setPermAs(GlobalPermissionsHandler.getPermFromIndex(index), value);
        localView.RPC("updatePerms", RpcTarget.All, PhotonNetwork.LocalPlayer, GlobalPermissionsHandler.returnPermissions());
    }

    [PunRPC]

    public void updatePerms(Player plrPermsChanged, bool[] perms)
    {
        Debug.Log("updatePerms:" + plrPermsChanged.ToString());
        playerToPerms.Remove(plrPermsChanged);
        playerToPerms.Add(plrPermsChanged, perms);
        permUIHandler.updateUi(plrPermsChanged, perms);
    }




    
    public void kickPlayerPush(Photon.Realtime.Player plrToKick)
    {
        Debug.Log("kick Player Push");
        localView.RPC("kickPlayerHandle", plrToKick);
    }

    [PunRPC]
    public void kickPlayerHandle()
    {
        Debug.Log("kick Player Handle");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.AutomaticallySyncScene = false;
        SceneManager.LoadScene("TitleScreen");
    }





}
