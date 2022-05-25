using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class InitiativeTokenUiHandler : MonoBehaviour
{
    /*
     * handles the ui for the token initiative list
     */
    [SerializeField] Image SelectedImage;
    [SerializeField] TMP_InputField InitianiveInput;
    [SerializeField] Button killMeButton;
    [SerializeField] TMP_Text tokenName;
    public TokenHandler_Script referenceToken;
    bool isMyTurn = false;
    public General_UI_DropDown_Handler_Script dropdownHandler;
    private void Start()
    {
        SelectedImage.enabled = false;
        killMeButton.onClick.AddListener(() =>
        {
            KILLME();
        });
    }


    
    System.Action<InitiativeTokenUiHandler> setUiCallback;
    System.Action InitiativeChangedCallback;
    public void setUp(TokenHandler_Script referenceToken, System.Action<InitiativeTokenUiHandler> setUiCallback, System.Action InitiativeChangedCallback)
    {
        this.InitiativeChangedCallback = InitiativeChangedCallback;
        this.setUiCallback = setUiCallback;
        this.referenceToken = referenceToken;
        tokenName.text = referenceToken.tokenName;
        InitianiveInput.text = "0";
    }

    bool lockMe = false;
    public void initiativeValueChanged()
    {
        if (lockMe)
        {
            return;
        }
        if (InitianiveInput.text.Equals(""))
        {
            InitianiveInput.text = "0";
        }
        lockMe = true;
        referenceToken.setInitiativeValue(int.Parse(InitianiveInput.text), false);
        InitiativeChangedCallback.Invoke();
        Debug.Log("offEdit");
        onEditBool = false;
        InitianiveInput.ReleaseSelection();
        lockMe = false;
    }

    private void Update()
    {
        tokenName.text = referenceToken.tokenName;
        if (!onEditBool)
        {
            //Debug.Log("updateInitiative:" + referenceToken.initiativeValue.ToString());
            InitianiveInput.text = referenceToken.initiativeValue.ToString();
        }
    }

    bool onEditBool = false;

    public void onEdit()
    {
        //Debug.Log("onEdit");
        onEditBool = true;
    }

    public void offEdit()
    {
        //Debug.Log("offEdit");
        onEditBool = false;
    }





    public void DeSelect()
    {
        SelectedImage.enabled = false;
    }

    public void Select()
    {
        SelectedImage.enabled = true;
    }

    public void KILLME()
    {
        setUiCallback.Invoke(this);
        Destroy(gameObject);
    }

    public int getInitiativeValue()
    {
        if (InitianiveInput.text.Length == 0)
        {
            return 0;
        }
        return int.Parse(InitianiveInput.text);


    }
}