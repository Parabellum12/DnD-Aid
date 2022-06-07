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
    /*
     * handles the game code for he main game
     */
    [SerializeField] SaveLoad_Handler_Script SaveLoadHandler;
    [SerializeField] PhotonView localView;
    List<SaveLoad_Handler_Script.saveClass> GlobalCachedMaps = new List<SaveLoad_Handler_Script.saveClass>();
    public List<SaveLoad_Handler_Script.saveClass> SharedMaps = new List<SaveLoad_Handler_Script.saveClass>();
    [SerializeField] Dictionary<Player, List<SaveLoad_Handler_Script.saveClass>> playerToCachedMaps = new Dictionary<Player, List<SaveLoad_Handler_Script.saveClass>>();
    [SerializeField] General_UI_DropDown_Handler_Script generalUiDropdownMainScr;
    [SerializeField] TMP_Text gameCodeText;
    [SerializeField] TMP_Text loadedMapText;
    [SerializeField] string loadedMapTextValue = "Currently Loaded Map:";
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
    int updateIndex = 0;
    public void Update()
    {
        if (firstUpdateLoop)
        {
            firstUpdateLoop = false;
            generalUiDropdownMainScr.setUiPositionsNoCallback();
            generalUiDropdownMainScr.setUiPositions();
        }
        //idk why but this fixes Ui Bug
        if (updateIndex < 0)
        {
            updateIndex++;
        }
        else
        {
            generalUiDropdownMainScr.setUiPositions();
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log("testWhy1");
        callAllPlayerPermUpdate();
        if (PhotonNetwork.IsMasterClient)
        { 
            RequestMapDataSyncPush(newPlayer); 
        }
        //Debug.Log("testWhy2");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("testWhy1");
        callAllPlayerPermUpdate();
        Debug.Log("testWhy2");
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
        PhotonNetwork.AutomaticallySyncScene = true;
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
        localView.RPC("addMapToGLobalCacheHandle", RpcTarget.All, UtilClass.ObjectToByteArray(temp), PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void addMapToGLobalCacheHandle(byte[] mapData, Player sendingPlayer)
    {
        GlobalCachedMaps.Add(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData));
        addToPlayerToCachedMaps(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData), sendingPlayer);
    }

    
    public void removeMapFromGlobalCachePush(string FileName)
    {
        if (!doesGlobalCacheContain(FileName))
        {
            return;
        }
        Debug.Log("remove to cache:" + FileName);
        SaveLoad_Handler_Script.saveClass temp = SaveLoadHandler.getMapData(FileName);
        localView.RPC("removeMapFromGLobalCacheHandle", RpcTarget.All, UtilClass.ObjectToByteArray(temp), PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void removeMapFromGLobalCacheHandle(byte[] mapData, Player sendingPlayer)
    {
        SaveLoad_Handler_Script.saveClass test = UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData);
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapID.Equals(test.MapID))
            {
                GlobalCachedMaps.Remove(sc);
                break;
            }
        }
        removeFromPlayerToCachedMaps(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData), sendingPlayer);
    }



    void addToPlayerToCachedMaps(SaveLoad_Handler_Script.saveClass sc, Player sendingPlayer)
    {
        Debug.Log("AddMapToPlayerToCache:" + sendingPlayer.NickName + " Map:" + sc.MapName);
        if (!playerToCachedMaps.ContainsKey(sendingPlayer))
        {
            List<SaveLoad_Handler_Script.saveClass> temp = new List<SaveLoad_Handler_Script.saveClass>();
            temp.Add(sc);
            playerToCachedMaps.Add(sendingPlayer, temp);
        }
        else
        {
            List<SaveLoad_Handler_Script.saveClass> temp = new List<SaveLoad_Handler_Script.saveClass>();
            playerToCachedMaps.TryGetValue(sendingPlayer, out List<SaveLoad_Handler_Script.saveClass> data);
            playerToCachedMaps.Remove(sendingPlayer);
            playerToCachedMaps.Add(sendingPlayer, temp);
        }
    }



    [SerializeField] MapSelector_Handler mapSelectorHandler_Script;

    [PunRPC]
    public void addToSharedMaps(byte[] sc, bool networkedCall)
    {
        Debug.Log("ShareMap");
        if (!networkedCall)
        {
            localView.RPC("addToSharedMaps", RpcTarget.Others, sc, true);
            return;
        }
        SharedMaps.Add(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(sc));
        mapSelectorHandler_Script.loadSharedFiles();
    }

    [PunRPC]
    public void removeFromSharedMaps(byte[] sc, bool networkedCall)
    {
        Debug.Log("UnShareMap");

        if (!networkedCall)
        {
            localView.RPC("removeFromSharedMaps", RpcTarget.Others, sc, true);
            return;
        }
        SharedMaps.Remove(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(sc));
        foreach (SaveLoad_Handler_Script.saveClass why in SharedMaps)
        {
            if (why.MapID.Equals(UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(sc).MapID))
            {
                SharedMaps.Remove(why);
                break;
            }
        }
        mapSelectorHandler_Script.loadSharedFiles();
    }








    void removeFromPlayerToCachedMaps(SaveLoad_Handler_Script.saveClass sc, Player sendingPlayer)
    {
        if (!playerToCachedMaps.ContainsKey(sendingPlayer))
        {
            return;
        }

        Debug.Log("removeFromPlayerToCachedMaps:" + sendingPlayer.NickName + " Map:" + sc.MapName);
        playerToCachedMaps.TryGetValue(sendingPlayer, out List<SaveLoad_Handler_Script.saveClass> tempData);
        foreach (SaveLoad_Handler_Script.saveClass scMap in tempData)
        {
            if (scMap.Equals(sc))
            {
                tempData.Remove(scMap);
                break;
            }
        }
        playerToCachedMaps.Remove(sendingPlayer);
        playerToCachedMaps.Add(sendingPlayer, tempData);
    }



    public void CallRequestMapDataSyncPush()
    {
        localView.RPC("RequestMapDataSyncPush", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void RequestMapDataSyncPush(Photon.Realtime.Player plr)
    {
        localView.RPC("RequestMapDataSyncHandle", plr, returnGlobalCacheAsByteArray(), UtilClass.ObjectToByteArray(SaveLoadHandler.getMapData(SaveLoadHandler.getCurrentlyLoadedMapID())));
    }

    [PunRPC]
    public void RequestMapDataSyncHandle(byte[] allData, byte[] currentlyLoadedMap)
    {
        Debug.Log("RequestMapDataSyncHandle");
        GlobalCachedMaps.Clear();
        GlobalCachedMaps.AddRange(returnGlobalCacheByte2DArrayToList(allData));
        SharedMaps.Clear();
        SharedMaps.AddRange(returnSharedByte2DArrayToList(allData));
        LoadMapDataHandle(currentlyLoadedMap);
        mapSelectorHandler_Script.loadSharedFiles();
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
        SaveLoad_Handler_Script.saveClass temp = UtilClass.ByteArrayToObject<SaveLoad_Handler_Script.saveClass>(mapData);
        loadedMapText.text = loadedMapTextValue + temp.MapName;
        SaveLoadHandler.loadMap(temp);
    }
    [PunRPC]
    public void LoadMapDataHandle(string MapId)
    {
        Debug.Log("load map from cache");
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapID.Equals(MapId))
            {
                loadedMapText.text = loadedMapTextValue + sc.MapName;
                SaveLoadHandler.loadMap(sc);
                return;
            }
        }
        Debug.Log("No Map Found To Load");
    }

    bool doesGlobalCacheContain(string  MapId)
    {
        foreach (SaveLoad_Handler_Script.saveClass sc in GlobalCachedMaps)
        {
            if (sc.MapID.Equals(MapId))
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
        CachedMapsWrapper wrapper = new CachedMapsWrapper(GlobalCachedMaps.ToArray(), SharedMaps.ToArray());
        return UtilClass.ObjectToByteArray(wrapper);
    }

    List<SaveLoad_Handler_Script.saveClass> returnGlobalCacheByte2DArrayToList(byte[] data)
    {
        CachedMapsWrapper wrapper = UtilClass.ByteArrayToObject<CachedMapsWrapper>(data);
        List<SaveLoad_Handler_Script.saveClass> returner = new List<SaveLoad_Handler_Script.saveClass>();
        returner.AddRange(wrapper.CachedMaps);
        return returner;
    }

    List<SaveLoad_Handler_Script.saveClass> returnSharedByte2DArrayToList(byte[] data)
    {
        CachedMapsWrapper wrapper = UtilClass.ByteArrayToObject<CachedMapsWrapper>(data);
        List<SaveLoad_Handler_Script.saveClass> returner = new List<SaveLoad_Handler_Script.saveClass>();
        returner.AddRange(wrapper.SharedMaps);
        return returner;
    }

    [System.Serializable]
    public class CachedMapsWrapper
    {
        public SaveLoad_Handler_Script.saveClass[] CachedMaps;
        public SaveLoad_Handler_Script.saveClass[] SharedMaps;
        public CachedMapsWrapper(SaveLoad_Handler_Script.saveClass[] dat, SaveLoad_Handler_Script.saveClass[] sharedMaps)
        {
            CachedMaps = dat;
            SharedMaps = sharedMaps;
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
        //Debug.Log("returned player perms");
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
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("kick Player Handle2");
        SceneManager.LoadScene("TitleScreen");
    }





}
