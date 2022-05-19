using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TokenHandler_Script : MonoBehaviour
{
    [SerializeField] Image TokenPfp;
    public TokenInfoHandler TokenInfoHandler_Script;
    [SerializeField] PhotonView localView;
    public void setTokenPFP(Texture2D tex)
    {
        TokenPfp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2)); ;
    }

    public void setInfoToThis()
    {
        TokenInfoHandler_Script.setActiveSelected(this);
    }

    public void KILLME()
    {
        localView.RPC("UpdateUiIfSelectedKilled", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateUiIfSelectedKilled()
    {
        if (TokenInfoHandler_Script != null && this != null && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            TokenInfoHandler_Script.ActiveSelectedToken = null;
            TokenInfoHandler_Script.SetUIToInactive();
        }
        if (this.gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject.GetPhotonView());
        }
    }

    bool mouseOver = false;
    bool moving = false;
    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (moving)
        {
            return;
        }
        mouseOver = false;
    }

    private void Update()
    {
        if (UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
        {
            return;
        }
        if (Input.GetMouseButton(0) && mouseOver && TokenInfoHandler_Script.ActiveSelectedToken != this)
        {
            TokenInfoHandler_Script.setActiveSelected(this);
        }
        else if (Input.GetMouseButton(0) && !mouseOver && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            TokenInfoHandler_Script.setActiveSelected(null);
        }
        if (Input.GetMouseButton(0) && mouseOver && TokenInfoHandler_Script.ActiveSelectedToken == this)
        {
            moving = true;
            Vector3 temp = UtilClass.getMouseWorldPosition();
            temp.z = -5;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position = new Vector3(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y), temp.z);
            }
            else
            {
                transform.position = temp;
            }
        }
        else
        {
            moving = false;
        }
    }


}
