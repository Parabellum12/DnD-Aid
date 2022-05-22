using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeList_Handler : MonoBehaviour
{
    [SerializeField] GameObject InitiativeListUiInteractPrefab;

    [SerializeField] GameObject Children;

    [SerializeField] List<GameObject> persistantChildObjects = new List<GameObject>();

    List<InitiativeTokenUiHandler> Handlers = new List<InitiativeTokenUiHandler>();

    [SerializeField] General_UI_DropDown_Handler_Script dropdownScript;

    int selectedIndex = 0;
         
    // Start is called before the first frame update
    void Start()
    {
        reloadObjectPos();
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
        scr.inInitiativeList = true;
        GameObject go = Instantiate(InitiativeListUiInteractPrefab, Children.transform);
        InitiativeTokenUiHandler scrHandler = go.GetComponent<InitiativeTokenUiHandler>();

        Handlers.Add(scrHandler);

        scrHandler.setUp(scr, (uiHandler) =>
        {
            removeUiTokenElement(uiHandler);
            reloadObjectPos();
        });
        reloadObjectPos();
    }

    public void removeUiTokenElement(TokenHandler_Script scr)
    {
        foreach (InitiativeTokenUiHandler scr2 in Handlers)
        {
            if (scr2.referenceToken.Equals(scr))
            {
                removeUiTokenElement(scr2);
                return;
            }
        }
    }


    public void reloadObjectPos()
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
    }

    void removeUiTokenElement(InitiativeTokenUiHandler handler)
    {
        handler.referenceToken.inInitiativeList = false;
        Handlers.Remove(handler);
        dropdownScript.RemoveFromChildDropDowns(handler.dropdownHandler);
        Destroy(handler.gameObject);
        if (selectedIndex >= Handlers.Count)
        {
            selectedIndex = Handlers.Count;
        }
        reloadObjectPos();
    }

    void increaseSelectedIndex()
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
    }

    public void deSelectAll()
    {
        foreach (InitiativeTokenUiHandler scr in Handlers)
        {
            scr.DeSelect();
        }
    }

    public void selectOnIndex()
    {
        deSelectAll();
        Handlers[selectedIndex].Select();
    }
}
