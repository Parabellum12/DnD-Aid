using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class TitleScreenHandler : MonoBehaviourPunCallbacks
{
    /*
     * handles the titlescreen code allows connecting, changing to editor and maingameS
     */
    [SerializeField] TMP_InputField gameCode;
    [SerializeField] TMP_InputField userName;
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
    }
    public void toMapCreator()
    {
        SceneManager.LoadScene("MapCreator", LoadSceneMode.Single);
    }

    public void toRoomConnect()
    {
        if (userName.text.Length > 0)
        {
            PhotonNetwork.NickName = userName.text;
        }
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
        options.IsOpen = true;
        string roomName = createRandomRoomName();
        if (gameCode.text.Length == 6)
        {
            roomName = gameCode.text;
        }
        Photon.Realtime.TypedLobby lobbyType = new Photon.Realtime.TypedLobby(roomName, Photon.Realtime.LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(roomName, options, null);

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
        Debug.Log("CreateLobby");
        if (!alreadySetPerms)
        {

            //PhotonNetwork.IsMessageQueueRunning = false;
            alreadySetPerms = true;
            GlobalPermissionsHandler.setPermsAsHost();
            Debug.Log("RoomName:" + PhotonNetwork.CurrentRoom.Name + "|");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("MainGame");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinLobby");
        base.OnJoinedRoom();
        if (!alreadySetPerms)
        {

            //PhotonNetwork.IsMessageQueueRunning = false;
            Debug.Log("JoinedRoom");
            alreadySetPerms = true;
            GlobalPermissionsHandler.setPermsAsClient();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
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
