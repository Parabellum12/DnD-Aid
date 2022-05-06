using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainGame_Handler_Script : MonoBehaviourPunCallbacks
{
    [SerializeField] SaveLoad_Handler_Script SaveLoadHandler;
    public PhotonView localView;
    List<SaveLoad_Handler_Script.saveClass> GlobalCachedMaps = new List<SaveLoad_Handler_Script.saveClass>();


    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            //client
            Debug.Log("CallRequestMapDataSyncPush");
            CallRequestMapDataSyncPush();
        }
        else
        {
            //master client
        }
    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
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
        localView.RPC("addMapToGLobalCacheHandle", RpcTarget.All, SaveLoadHandler.ObjectToByteArray(temp));
    }

    [PunRPC]
    public void addMapToGLobalCacheHandle(byte[] mapData)
    {
        GlobalCachedMaps.Add(SaveLoadHandler.ByteArrayToObject(mapData));
    }

    
    public void removeMapFromGlobalCachePush(string FileName)
    {
        if (!doesGlobalCacheContain(FileName))
        {
            return;
        }
        Debug.Log("remove to cache:" + FileName);
        SaveLoad_Handler_Script.saveClass temp = SaveLoadHandler.getMapData(FileName);
        localView.RPC("removeMapFromGLobalCacheHandle", RpcTarget.All, SaveLoadHandler.ObjectToByteArray(temp));
    }

    [PunRPC]
    public void removeMapFromGLobalCacheHandle(byte[] mapData)
    {
        SaveLoad_Handler_Script.saveClass test = SaveLoadHandler.ByteArrayToObject(mapData);
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
        localView.RPC("RequestMapDataSyncHandle", plr, returnGlobalCacheAsByteArray(), SaveLoadHandler.ObjectToByteArray(SaveLoadHandler.getMapData(SaveLoadHandler.getCurrentlyLoadedMapName())));
    }

    [PunRPC]
    public void RequestMapDataSyncHandle(byte[][] allData, byte[] currentlyLoadedMap)
    {
        Debug.Log("RequestMapDataSyncHandle");
        GlobalCachedMaps.Clear();
        GlobalCachedMaps.AddRange(returnGlobalCacheByte2DArrayToList(allData));
        SaveLoadHandler.loadMap(SaveLoadHandler.ByteArrayToObject(currentlyLoadedMap));
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
            localView.RPC("LoadMapDataHandle", RpcTarget.All, SaveLoadHandler.ObjectToByteArray(SaveLoadHandler.getMapData(map)));
        }
    }

    [PunRPC]
    public void LoadMapDataHandle(byte[] mapData)
    {
        Debug.Log("load Map from data");
        SaveLoadHandler.loadMap(SaveLoadHandler.ByteArrayToObject(mapData));
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

    byte[][] returnGlobalCacheAsByteArray()
    {
        byte[][] returner = new byte[GlobalCachedMaps.Count][];
        for (int i = 0; i < GlobalCachedMaps.Count; i++)
        {
            returner[i] = SaveLoadHandler.ObjectToByteArray(GlobalCachedMaps[i]);
        }
        return returner;
    }

    List<SaveLoad_Handler_Script.saveClass> returnGlobalCacheByte2DArrayToList(byte[][] data)
    {
        List<SaveLoad_Handler_Script.saveClass> returner = new List<SaveLoad_Handler_Script.saveClass>();
        foreach (byte[] dat in data)
        {
            returner.Add(SaveLoadHandler.ByteArrayToObject(dat));
        }
        return returner;
    }

}
