using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class InitiativeList_Handler : MonoBehaviourPunCallbacks
{
    /*
     * handles the overall initiative list code
     */
    [SerializeField] GameObject InitiativeListUiInteractPrefab;

    [SerializeField] GameObject Children;

    [SerializeField] List<GameObject> persistantChildObjects = new List<GameObject>();

    [SerializeField] List<InitiativeTokenUiHandler> Handlers = new List<InitiativeTokenUiHandler>();

    [SerializeField] General_UI_DropDown_Handler_Script dropdownScript;

    [SerializeField] PhotonView localView;

    [SerializeField] List<Button> ListInteractButtons = new List<Button>();

    int selectedIndex = 0;
         
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            localView.RPC("syncList", RpcTarget.MasterClient);
            return;
        }
        reloadObjectPos(false);
        selectOnIndex(false, selectedIndex);
        
    }


    private void Update()
    {
        if (GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.EditInitiativeList))
        {
            foreach (Button bu in ListInteractButtons)
            {
                bu.interactable = true;
            }
        }
        else
        {
            foreach (Button bu in ListInteractButtons)
            {
                bu.interactable = false;
            }
        }
    }

    

    [PunRPC]
    public void syncList()
    {
        foreach (InitiativeTokenUiHandler scr in Handlers)
        {
            scr.referenceToken.addMeToInitiativeList(false);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
    }


    public void clearList()
    {

        for (int i = Handlers.Count-1; i >= 0; i-- )
        {
            InitiativeTokenUiHandler scr = Handlers[i];
            scr.referenceToken.removeMeFromInitiativeList(false);
        }
    }

    

    public void addTokenUiElement(TokenHandler_Script scr)
    {
        foreach (InitiativeTokenUiHandler scr2 in Handlers)
        {
            if (scr2.referenceToken.Equals(scr))
            {
                return;
            }
        }

        GameObject go = Instantiate(InitiativeListUiInteractPrefab, Children.transform);
        InitiativeTokenUiHandler scrHandler = go.GetComponent<InitiativeTokenUiHandler>();

        Handlers.Add(scrHandler);
        scr.setInitiativeValue(0, false);
        scrHandler.setUp(scr, (uiHandler) =>
        {
            scr.removeMeFromInitiativeList(false);
            reloadObjectPos(false);
        }, () =>
        {
            //organize list
            SortInitiativeList(false);
        });
        reloadObjectPos(false);
    }

    public void removeUiTokenElement(TokenHandler_Script scr)
    {
        foreach (InitiativeTokenUiHandler scr2 in Handlers)
        {
            if (scr2.referenceToken.Equals(scr))
            {
                Handlers.Remove(scr2);
                return;
            }
        }
    }
    void removeUiTokenElement(InitiativeTokenUiHandler handler)
    {
        Handlers.Remove(handler);
        dropdownScript.RemoveFromChildDropDowns(handler.dropdownHandler);
        Destroy(handler.gameObject);
        if (selectedIndex >= Handlers.Count)
        {
            selectedIndex = Handlers.Count;
        }
        reloadObjectPos(false);
    }

    public void removeUiTokenElementCallFromToken(TokenHandler_Script scr)
    {
        InitiativeTokenUiHandler handler = null;
        foreach (InitiativeTokenUiHandler scr2 in Handlers)
        {
            if (scr2.referenceToken.Equals(scr))
            {
                handler = scr2;
                break;
            }
        }
        Handlers.Remove(handler);
        removeUiTokenElement(scr);
        dropdownScript.RemoveFromChildDropDowns(handler.dropdownHandler);
        Destroy(handler.gameObject);
        if (selectedIndex >= Handlers.Count)
        {
            selectedIndex = Handlers.Count;
        }
        reloadObjectPos(false);
    }

    [PunRPC]
    public void reloadObjectPos(bool networkCAll)
    {
        dropdownScript.clearChildDropDowns();
        foreach (GameObject go in persistantChildObjects)
        {
            dropdownScript.addToChildDropDowns(go.GetComponent<General_UI_DropDown_Handler_Script>());
        }
        foreach (InitiativeTokenUiHandler scr in Handlers)
        {
            dropdownScript.addToChildDropDowns(scr.gameObject.GetComponent<General_UI_DropDown_Handler_Script>());
        }
        dropdownScript.setUiPositions();
        if (!networkCAll)
        {
            localView.RPC("reloadObjectPos", RpcTarget.Others, true);
        }
    }


    [PunRPC]
    void increaseSelectedIndex(bool networkCall)
    {
        selectedIndex++;
        if (Handlers.Count == 0)
        {
            selectedIndex = 0;
        }
        else
        {
            selectedIndex = selectedIndex % Handlers.Count;
        }
        if (!networkCall)
        {
            localView.RPC("increaseSelectedIndex", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    void decreaseSelectedIndex(bool networkCall)
    {
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = Handlers.Count - 1;
        }
        if (!networkCall)
        {
            localView.RPC("decreaseSelectedIndex", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void nextSelected(bool networkCall)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            localView.RPC("nextSelected", RpcTarget.MasterClient, false);
            return;
        }
        increaseSelectedIndex(false);
        selectOnIndex(false, selectedIndex);
    }

    [PunRPC]
    public void previousSelected(bool networkCall)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            localView.RPC("previousSelected", RpcTarget.MasterClient, false);
            return;
        }
        decreaseSelectedIndex(false);
        selectOnIndex(false, selectedIndex);
    }



    public void deSelectAll()
    {
        foreach (InitiativeTokenUiHandler scr in Handlers)
        {
            scr.DeSelect();
        }
    }

    [PunRPC]
    public void selectOnIndex(bool networkCall, int index)
    {
        if (!networkCall)
        {
            if (selectedIndex < 0 || selectedIndex >= Handlers.Count)
            {
                return;
            }
            deSelectAll();
            Handlers[selectedIndex].Select();
        }
        else
        {
            if (index < 0 || index >= Handlers.Count)
            {
                return;
            }
            deSelectAll();
            Handlers[index].Select();
        }



        if (!networkCall)
        {
            localView.RPC("selectOnIndex", RpcTarget.Others, true, selectedIndex);
        }
    }

    [PunRPC]
    public void SortInitiativeList(bool networkCall)
    {
        if (!networkCall)
        {
            localView.RPC("SortInitiativeList", RpcTarget.Others, true);
        }
        //Debug.Log("SortList");
        int highestNum = 100;
        int highestNumIndex = 0;
        List<InitiativeTokenUiHandler> HandlersTemp = new List<InitiativeTokenUiHandler>();
        while (Handlers.Count > 0)
        {
            highestNum = 100;
            for (int j = Handlers.Count-1; j >= 0; j--)
            {
                int tempInt = Handlers[j].referenceToken.initiativeValue;
                if (tempInt < highestNum)
                {
                    highestNum = tempInt;
                    highestNumIndex = j;
                }
            }
            HandlersTemp.Add(Handlers[highestNumIndex]);
            Handlers.RemoveAt(highestNumIndex);
        }
        HandlersTemp.Reverse();
        Handlers.Clear();
        Handlers.AddRange(HandlersTemp);


        reloadObjectPos(true);
    }
}
