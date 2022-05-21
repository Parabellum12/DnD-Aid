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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addTokenUiElement(TokenHandler_Script scr)
    {
        GameObject go = Instantiate(InitiativeListUiInteractPrefab, Children.transform);
        InitiativeTokenUiHandler scrHandler = go.GetComponent<InitiativeTokenUiHandler>();
        scrHandler.setUp(scr, (uiHandler) =>
        {
            removeUiTokenElement(uiHandler);
            reloadObjectPos();
        });
        Handlers.Add(scrHandler);
        reloadObjectPos();
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
        Handlers.Remove(handler);
        dropdownScript.RemoveFromChildDropDowns(handler.dropdownHandler);
        dropdownScript.setUiPositions();
        selectedIndex = selectedIndex % Handlers.Count;
    }

    void increaseSelectedIndex()
    {
        selectedIndex++;
        selectedIndex = selectedIndex % Handlers.Count;
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
