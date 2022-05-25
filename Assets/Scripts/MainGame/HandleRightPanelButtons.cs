using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRightPanelButtons : MonoBehaviour
{
    /*
     * handles changing the ui for the right panel
     */
    [SerializeField] GameObject Info = null;
    [SerializeField] GameObject permissions = null;
    [SerializeField] GameObject MapSelector = null;

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
        disableAll();
        permissions.SetActive(true);
    }

    public void toMapSelector()
    {
        if (!GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.LoadMaps))
        {
            Debug.Log("No Map Perm");
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
