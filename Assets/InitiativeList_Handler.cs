using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InitiativeList_Handler : MonoBehaviour
{
    [SerializeField] GameObject InitiativeListUiInteractPrefab;

    [SerializeField] GameObject Children;

    [SerializeField] List<GameObject> persistantChildObjects = new List<GameObject>();

    [SerializeField] List<InitiativeTokenUiHandler> Handlers = new List<InitiativeTokenUiHandler>();

    [SerializeField] General_UI_DropDown_Handler_Script dropdownScript;

    [SerializeField] PhotonView localView;

    int selectedIndex = 0;
         
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        reloadObjectPos(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void clearList()
    {

        for (int i = Handlers.Count-1; i >= 0; i-- )
        {
            InitiativeTokenUiHandler scr = Handlers[i];
            removeUiTokenElement(scr);
        }
    }

    

    public void addTokenUiElement(TokenHandler_Script scr)
    {
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
        increaseSelectedIndex(false);
        if (!networkCall)
        {
            localView.RPC("nextSelected", RpcTarget.Others, true);
        }
        selectOnIndex(false);

    }

    [PunRPC]
    public void previousSelected(bool networkCall)
    {
        decreaseSelectedIndex(false);
        if (!networkCall)
        {
            localView.RPC("previousSelected", RpcTarget.Others, true);
        }
        selectOnIndex(false);
    }



    public void deSelectAll()
    {
        foreach (InitiativeTokenUiHandler scr in Handlers)
        {
            scr.DeSelect();
        }
    }

    [PunRPC]
    public void selectOnIndex(bool networkCall)
    {
        if (selectedIndex < 0 || selectedIndex >= Handlers.Count)
        {
            return;
        }
        deSelectAll();
        Handlers[selectedIndex].Select();
        if (!networkCall)
        {
            localView.RPC("selectOnIndex", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void SortInitiativeList(bool networkCall)
    {
        Debug.Log("SortList");
        int lowestNum = 100;
        int lowestNumIndex = 0;
        List<InitiativeTokenUiHandler> HandlersTemp = new List<InitiativeTokenUiHandler>();
        while (Handlers.Count > 0)
        { 
            for (int j = 0; j < Handlers.Count; j++)
            {
                int tempInt = Handlers[j].referenceToken.initiativeValue;
                if (tempInt < lowestNum)
                {
                    lowestNum = tempInt;
                    lowestNumIndex = j;
                }
            }
            HandlersTemp.Add(Handlers[lowestNumIndex]);
            Handlers.RemoveAt(lowestNumIndex);
        }
        HandlersTemp.Reverse();
        Handlers = HandlersTemp;


        reloadObjectPos(false);
        if (!networkCall)
        {
            localView.RPC("SortInitiativeList", RpcTarget.Others, true);
        }
    }
}
