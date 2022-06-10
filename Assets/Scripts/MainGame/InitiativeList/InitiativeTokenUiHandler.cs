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
    [SerializeField] Button SelectMeButton;
    public TokenHandler_Script referenceToken;
    //bool isMyTurn = false;
    public General_UI_DropDown_Handler_Script dropdownHandler;
    TokenInfoHandler infoHandler;
    private void Start()
    {
        SelectedImage.enabled = false;
        killMeButton.onClick.AddListener(() =>
        {
            KILLME();
        });
        SelectMeButton.onClick.AddListener(() =>
        {
            handleSelectMe();
        });
    }


    
    System.Action<InitiativeTokenUiHandler> setUiCallback;
    System.Action InitiativeChangedCallback;
    public void setUp(TokenHandler_Script referenceToken, TokenInfoHandler tokenInfoHande, System.Action<InitiativeTokenUiHandler> setUiCallback, System.Action InitiativeChangedCallback)
    {
        this.infoHandler = tokenInfoHande;
        this.InitiativeChangedCallback = InitiativeChangedCallback;
        this.setUiCallback = setUiCallback;
        this.referenceToken = referenceToken;
        onEditBool = false;
        tokenName.text = referenceToken.tokenName;
        lockMe = true;
        InitianiveInput.text = referenceToken.initiativeValue.ToString();
        lockMe = false;
    }

    [SerializeField] float maxTimeBetweenClicks = .5f;
    float doubleClickTimer = 0;
    bool doubleClickCheck = false;

    public void handleSelectMe()
    {
        infoHandler.setActiveSelected(referenceToken);
        if (infoHandler.ActiveSelectedToken != null && infoHandler.ActiveSelectedToken.Equals(referenceToken))
        {
            if (!doubleClickCheck)
            {
                doubleClickTimer = 0;
                doubleClickCheck = true;
            }
            else if (doubleClickTimer <= maxTimeBetweenClicks && doubleClickCheck)
            {
                Camera.main.transform.position = new Vector3(referenceToken.gameObject.transform.position.x, referenceToken.gameObject.transform.position.y, Camera.main.transform.position.z);

                doubleClickCheck = false;
            }
            
        }
        else
        {
            doubleClickCheck = false;
        }
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
        if (doubleClickCheck)
        {
            doubleClickTimer += Time.deltaTime;
            if (doubleClickTimer > maxTimeBetweenClicks)
            {
                doubleClickCheck = false;
            }
        }
        tokenName.text = referenceToken.tokenName;
        if (!onEditBool && !InitianiveInput.text.Equals(referenceToken.initiativeValue.ToString()))
        {
            //Debug.Log("updateInitiative:" + referenceToken.initiativeValue.ToString());
            InitianiveInput.text = referenceToken.initiativeValue.ToString();
        }
        if (referenceToken == null)
        {
            KILLME();
        }
        InitianiveInput.interactable = GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.EditInitiativeList);
        killMeButton.interactable = GlobalPermissionsHandler.getPermValue(GlobalPermissionsHandler.PermisionNameToValue.EditInitiativeList);
        handleFlash();
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
        if (!lockMe)
        {
            onEditBool = false;
        }
    }


    public bool isSelected()
    {
        return SelectedImage.enabled;
    }

    bool selected = false;
    public void DeSelect()
    {
        selected = false;
        SelectedImage.enabled = false;
    }

    public void Select()
    {
        selected = true;
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


    bool orangeToWhite = true;
    float flashTime = 0;

    void handleFlash()
    {
        if (selected || infoHandler.ActiveSelectedToken != null && infoHandler.ActiveSelectedToken.Equals(referenceToken))
        {
            if (selected)
            {
                SelectedImage.enabled = true;


                if (infoHandler.ActiveSelectedToken != null && infoHandler.ActiveSelectedToken.Equals(referenceToken))
                {
                    flashTime += Time.deltaTime;
                    if (flashTime > 1)
                    {
                        flashTime -= 1;
                        orangeToWhite = !orangeToWhite;
                    }

                    if (orangeToWhite)
                    {
                        SelectedImage.color = Color.Lerp(Color.green, Color.white, flashTime);
                    }
                    else
                    {
                        SelectedImage.color = Color.Lerp(Color.white, Color.green, flashTime);
                    }
                }
                else
                {
                    SelectedImage.color = Color.green;
                    orangeToWhite = true;
                    flashTime = 0;
                }
            }
            else
            {
                SelectedImage.enabled = true;
                if (infoHandler.ActiveSelectedToken != null && infoHandler.ActiveSelectedToken.Equals(referenceToken))
                {
                    flashTime += Time.deltaTime;
                    if (flashTime > 1)
                    {
                        flashTime -= 1;
                        orangeToWhite = !orangeToWhite;
                    }

                    if (orangeToWhite)
                    {
                        SelectedImage.color = Color.Lerp(Color.blue, Color.white, flashTime);
                    }
                    else
                    {
                        SelectedImage.color = Color.Lerp(Color.white, Color.blue, flashTime);
                    }
                }
                else
                {
                    SelectedImage.color = Color.green;
                    orangeToWhite = true;
                    flashTime = 0;
                }
            }
        }
        else if (!selected)
        {
            SelectedImage.enabled = false;
        }
    }

}
