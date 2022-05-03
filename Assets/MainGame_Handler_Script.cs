using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MainGame_Handler_Script : MonoBehaviour
{
    [SerializeField] SaveLoad_Handler_Script SaveLoadHandler;
    public PhotonView localView;
    List<SaveLoad_Handler_Script.saveClass> GlobalCachedMaps = new List<SaveLoad_Handler_Script.saveClass>();
    // Start is called before the first frame update
    void Start()
    {

    }

    
    public void addMapToGlobalCachePush(string FileName)
    {
        localView.RPC("addMapToGLobalCacheHandle", RpcTarget.All, SaveLoadHandler.ObjectToByteArray(SaveLoadHandler.getMapData(FileName)));
    }

    [PunRPC]
    public void addMapToGLobalCacheHandle(byte[] mapData)
    {
        GlobalCachedMaps.Add(SaveLoadHandler.ByteArrayToObject(mapData));
    }

    
    public void removeMapFromGlobalCachePush(string FileName)
    {
        localView.RPC("removeMapFromGLobalCacheHandle", RpcTarget.All, SaveLoadHandler.ObjectToByteArray(SaveLoadHandler.getMapData(FileName)));
    }

    [PunRPC]
    public void removeMapFromGLobalCacheHandle(byte[] mapData)
    {
        GlobalCachedMaps.Remove(SaveLoadHandler.ByteArrayToObject(mapData));
    }

    public void CallRequestMapDataSyncPush()
    {
        localView.RPC("RequestMapDataSyncPush", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void RequestMapDataSyncPush(Photon.Realtime.Player plr)
    {
        localView.RPC("RequestMapDataSyncHandle", plr, GlobalCachedMaps, SaveLoadHandler.getMapData(SaveLoadHandler.getCurrentlyLoadedMapName()));
    }

    [PunRPC]
    public void RequestMapDataSyncHandle(List<SaveLoad_Handler_Script.saveClass> allData, SaveLoad_Handler_Script.saveClass currentlyLoadedMap)
    {
        GlobalCachedMaps.Clear();
        GlobalCachedMaps.AddRange(allData);
        SaveLoadHandler.loadMap(currentlyLoadedMap);
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
}