using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalPermissionsHandler
{
    /* all global perms
     * 
     * change other player perms
     * load maps
     * kick players
     * edit initiative list
     * edit fog of war mask
     * see through fog of war mask
     * add tokens
     * remove tokens
     * global move tokens
     */

    static bool[] permBools = new bool[9];
    public enum PermisionNameToValue
    {
        ChangeOtherPlayerPerms = 0,
        LoadMaps = 1,
        KickPlayers = 2,
        EditInitiativeList = 3,
        EditFogOfWarMask = 4,
        SeeThroughFogOfWarMask = 5,
        AddToken = 6,
        RemoveTokens = 7,
        GlobalMoveTokens = 8,
    }

    static public void setPermsAsHost()
    {
        setPermAs(PermisionNameToValue.ChangeOtherPlayerPerms, true);
        setPermAs(PermisionNameToValue.LoadMaps, true);
        setPermAs(PermisionNameToValue.KickPlayers, true);
        setPermAs(PermisionNameToValue.EditInitiativeList, true);
        setPermAs(PermisionNameToValue.EditFogOfWarMask, true);
        setPermAs(PermisionNameToValue.SeeThroughFogOfWarMask, true);
        setPermAs(PermisionNameToValue.AddToken, true);
        setPermAs(PermisionNameToValue.RemoveTokens, true);
        setPermAs(PermisionNameToValue.GlobalMoveTokens, true);
    }

    static public void setPermsAsClient()
    {
        setPermAs(PermisionNameToValue.ChangeOtherPlayerPerms, false);
        setPermAs(PermisionNameToValue.LoadMaps, false);
        setPermAs(PermisionNameToValue.KickPlayers, false);
        setPermAs(PermisionNameToValue.EditInitiativeList, false);
        setPermAs(PermisionNameToValue.EditFogOfWarMask, false);
        setPermAs(PermisionNameToValue.SeeThroughFogOfWarMask, false);
        setPermAs(PermisionNameToValue.AddToken, false);
        setPermAs(PermisionNameToValue.RemoveTokens, false);
        setPermAs(PermisionNameToValue.GlobalMoveTokens, false);
    }

    static public bool getPermValue(PermisionNameToValue perm)
    {
        return permBools[(int)perm];
    }

    static public void setPermAs(PermisionNameToValue permToValue, bool value)
    {
        //Debug.Log("Changed " + permToValue.ToString() + ":" + (int)permToValue + " To " + value.ToString());
        permBools[(int)permToValue] = value;
    }
}
