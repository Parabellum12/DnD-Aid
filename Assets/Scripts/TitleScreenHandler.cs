using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class TitleScreenHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField gameCode;
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }
    public void toMapCreator()
    {
        SceneManager.LoadScene("MapCreator", LoadSceneMode.Single);
    }

    public void toRoomConnect()
    {
        if (PhotonNetwork.NickName.Length == 0)
        {
            PhotonNetwork.NickName = "Unknown Player";
        }
        if (!PhotonNetwork.IsConnected)
        {
            textValue = "Connecting";
            showConnectionText = true;
            showDots = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            createLobby();
        }
    }


    void createLobby()
    {
        textValue = "Connecting";
        showConnectionText = true;
        showDots = true;
        Photon.Realtime.RoomOptions options = new Photon.Realtime.RoomOptions();
        options.MaxPlayers = 0;
        options.IsOpen = false;
        string roomName = createRandomRoomName();
        if (gameCode.text.Length == 6)
        {
            roomName = gameCode.text;
        }
        PhotonNetwork.JoinOrCreateRoom(roomName, options, Photon.Realtime.TypedLobby.Default);
    }

    string createRandomRoomName()
    {
        string roomNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
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
    bool alreadySetPerms = false;
    public override void OnCreatedRoom()
    {
        if (!alreadySetPerms)
        {
            alreadySetPerms = true;
            GlobalPermissionsHandler.setPermsAsHost();
        }
        Debug.Log("RoomName:"+PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("MainGame");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!alreadySetPerms)
        {
            alreadySetPerms = true;
            GlobalPermissionsHandler.setPermsAsClient();
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        textValue = "Failed To Join Room: " + message;
        showDots = false;
        showConnectionText = true;
        Debug.Log("join Fail; Error Code:" + returnCode + "; message:" + message);
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
    bool showDots = true;
    int dotNum = 0;
    float dotTimer = 0;
    [SerializeField] float dotSpeed = 1;
    void handleConnectionText()
    {
        dotTimer += Time.deltaTime;
        string tempText = textValue;
        if (showDots)
        {
            for (int i = 0; i < dotNum + 1; i++)
            {
                tempText += ".";
            }
            if (dotTimer > dotSpeed)
            {
                dotTimer -= dotSpeed;
                dotNum++;
            }
            dotNum %= 3;
        }
        connectionText.text = tempText;
    }
    
}
