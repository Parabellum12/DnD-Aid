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

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        showConnectionText = false;
        base.OnConnectedToMaster();
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
