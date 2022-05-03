using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class TitleScreenHandler : MonoBehaviourPunCallbacks
{

    public void toMapCreator()
    {
        SceneManager.LoadScene("MapCreator", LoadSceneMode.Single);
    }

    public void toRoomConnect()
    {
        textValue = "Connecting";
        showConnectionText = true;
        PhotonNetwork.ConnectUsingSettings();
    }


    void createLobby()
    {
        Photon.Realtime.RoomOptions options = new Photon.Realtime.RoomOptions();
        options.MaxPlayers = 0;
        options.IsOpen = false;
        PhotonNetwork.JoinOrCreateRoom(createRandomRoomName(), options, Photon.Realtime.TypedLobby.Default);
    }

    string createRandomRoomName()
    {
        string roomNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        string roomName = "";

        for (int i = 0; i < 6; i++)
        {
            int charPos = Random.Range(0, roomNameCharacters.Length);
            roomName += roomNameCharacters.ToCharArray()[charPos];
        }
        return roomName;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        createLobby();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        GlobalPermissionsHandler.setPermsAsHost();
        Debug.Log("RoomName:"+PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("MainGame");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GlobalPermissionsHandler.setPermsAsClient();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        showConnectionText = false;
        base.OnConnectedToMaster();

        createLobby();
    }


    private void Update()
    {
        if (showConnectionText)
        {
            connectionText.gameObject.SetActive(true);
            handleConnectionText();
        }
        else
        {
            connectionText.gameObject.SetActive(false);
        }
    }

    bool dots = false;
    string textValue = "";
    [SerializeField] TMP_Text connectionText;
    bool showConnectionText = false;
    int dotNum = 0;
    float dotTimer = 0;
    [SerializeField] float dotSpeed = 1;
    void handleConnectionText()
    {
        dotTimer += Time.deltaTime;
        string tempText = textValue;
        for (int i = 0; i < dotNum+1; i++)
        {
            tempText += ".";
        }
        if (dotTimer > dotSpeed)
        {
            dotTimer -= dotSpeed;
            dotNum++;
        }
        dotNum %= 3;
        connectionText.text = tempText;
    }
    
}
