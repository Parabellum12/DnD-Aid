using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRightPanelButtons : MonoBehaviour
{
    [SerializeField] GameObject Info;
    [SerializeField] GameObject permissions;
    [SerializeField] GameObject MapSelector;

    private void Start()
    {
        toInfo();
    }

    public void toInfo()
    {
        disableAll();
        Info.SetActive(true);
    }

    public void toPerms()
    {
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.ChangeOtherPlayerPerms) && !GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.KickPlayers))
        {
            return;
        }
        disableAll();
        permissions.SetActive(true);
    }

    public void toMapSelector()
    {
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.LoadMaps))
        {
            return;
        }
        disableAll();
        MapSelector.SetActive(true);
    }


    void disableAll()
    {
        Info.SetActive(false);
        permissions.SetActive(false);
        MapSelector.SetActive(false);
    }
}
