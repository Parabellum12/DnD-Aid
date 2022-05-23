using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeTokenUiHandler : MonoBehaviour
{
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

    public void initiativeValueChanged()
    {
        InitiativeChangedCallback.Invoke();
    }

    private void Update()
    {
        tokenName.text = referenceToken.tokenName;
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
}
