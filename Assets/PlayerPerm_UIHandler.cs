using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerPerm_UIHandler : MonoBehaviour
{
    [SerializeField] Image backgorund;
    [SerializeField] Image permsBackgournd;
    [SerializeField] GameObject PermsGameobject;
    [SerializeField] TMP_Text playerName;
    [SerializeField] List<changePermsPrefab_Script> uiElements;
    Photon.Realtime.Player plr;
    MainGame_Handler_Script gameHandler;

    System.Action<int, bool> OnPermChangeCallback;
    public void setup(ref Photon.Realtime.Player plr, bool[] permValues, System.Action<int, bool> OnPermChangeCallback)
    {
        Debug.Log("DDDDDDDDDDDDDDDDDDD:" + (OnPermChangeCallback == null));
        this.OnPermChangeCallback = OnPermChangeCallback;
        gameHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Handler_Script>();
        playerName.text = plr.NickName;
        gameHandler.localView = gameObject.AddComponent<PhotonView>();
        this.plr = plr;
        setValues(permValues);
    }

    public void setValues(Photon.Realtime.Player plr, bool[] perms)
    {
        if (this.plr.Equals(plr))
        {
            setValues(perms);
        }
    }

    void setValues(bool[] perms)
    {
        if (perms == null)
        {
            Debug.Log("setValues(bool[]) perms is null");
            return;
        }
        for (int i = 0; i < uiElements.Count; i++)
        {
            Debug.Log("whatHappened:" + i);
            Debug.Log(uiElements.Count);
            Debug.Log(perms.Length);
            uiElements[i].setup(GlobalPermissionsHandler.getPermFromIndex(i).ToString(), perms[i], i, (index, value) =>
            {
                updatePlayerPermsPush(index, value);
            });
        }
    }

    public float getSize()
    {
        float size = backgorund.rectTransform.rect.height;
        if (PermsGameobject.activeSelf)
        {
            size += permsBackgournd.rectTransform.rect.height;
        }
        return size;
    }

    void updatePlayerPermsPush(int index, bool value)
    {
        OnPermChangeCallback.Invoke(index, value);
    }


}
