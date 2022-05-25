using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General_UI_DropDown_Handler_Script : MonoBehaviour
{
    /*
     * handles the ui changing for the token info/initiative list
     */
    [SerializeField] Button InteractButton = null;
    [SerializeField] Image mainImage = null;
    [SerializeField] GameObject dropDownBackGroundImage = null;
    [SerializeField] float offsetDist = 0;
    [SerializeField] float globalOffset = 0;
    [SerializeField] List<General_UI_DropDown_Handler_Script> childDropDowns = new List<General_UI_DropDown_Handler_Script>();
    bool engaged = false;
    public System.Action updateUICallBack = null;
    [SerializeField] bool IsChildOrParent = true;
    [SerializeField] bool dropDownImageFollowChilrenSize = true;
    [SerializeField] GameObject childrenHolder = null;
    [SerializeField] RectTransform buttonRectTransform;
    [SerializeField] bool IsHolder = false;


    /*
     * global offset must be set to size of children icons
     * all children icons must be same size
     * 
     */




    // Start is called before the first frame update
    void Start()
    {

        if (IsChildOrParent)
        {
            engaged = false;

            if (dropDownBackGroundImage != null)
            dropDownBackGroundImage?.SetActive(false);

            if (childrenHolder != null)
            {
                childrenHolder?.SetActive(false);
                dropDownBackGroundImage.SetActive(false);
                buttonRectTransform.rotation = Quaternion.Euler(0, 0, 0);
                childrenHolder?.SetActive(false);
            }

            if (InteractButton != null)
                InteractButton?.onClick.AddListener(() => { HandleClick(); });
        }
        foreach (General_UI_DropDown_Handler_Script scr in childDropDowns)
        {
            scr.updateUICallBack = () =>
            {
                setUiPositions();
            };
        }
    }

    public float getSize()
    {
        if (dropDownBackGroundImage != null && dropDownBackGroundImage.activeSelf && !IsHolder)
        {
            //Debug.Log(gameObject.name + ":returnAllsize");
            return dropDownBackGroundImage.GetComponent<RectTransform>().rect.height + mainImage.GetComponent<RectTransform>().rect.height;
        }
        else
        {
            //Debug.Log(gameObject.name + ":returnCLosedsize");
            return mainImage.GetComponent<RectTransform>().rect.height;
        }
    }

    [SerializeField] RectTransform contentRectTransform;

    public void setUiPositions()
    {
        if (IsHolder)
        {
            return;
        }
        offsetDist = globalOffset;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            if (!childDropDowns[i].gameObject.activeSelf)
            {
                continue;
            }
            //Debug.Log(gameObject.name+":settingPosOF: " + childDropDowns [i].gameObject.name+ " to " + offsetDist);
            childDropDowns[i].transform.localPosition = new Vector2(0, -offsetDist);
            offsetDist += childDropDowns[i].getSize();
        }
        setPosHelper();
        updateUICallBack?.Invoke();
    }

    void setPosHelper()
    {
        if (IsHolder)
        {
            return;
        }
        if (IsChildOrParent)
        {
            if (dropDownImageFollowChilrenSize)
            {
                RectTransform temp = dropDownBackGroundImage.GetComponent<RectTransform>();
                //Debug.Log(gameObject.name + ":rectFollowSize");
                //temp.rect.Set(temp.rect.x, temp.rect.y, temp.rect.width, offsetDist - globalOffset);
                temp.sizeDelta = new Vector2(temp.rect.width, offsetDist - globalOffset);
                //Debug.Log(gameObject.name + ":TempHeight5:" + ((temp.rect.size.y / 2f) - globalOffset));
                //temp.localPosition = Vector2.zero;
                temp.localPosition = new Vector2(0, -(temp.rect.size.y/2) - (gameObject.GetComponent<RectTransform>().rect.height/2));
                //Debug.Log(gameObject.name + ":TemplocalPos:" + temp.localPosition);
            }
        }
        else
        {
            contentRectTransform.sizeDelta = new Vector2(512, offsetDist - globalOffset);
        }
    }

    public void setUiPositionsNoCallback()
    {
        //Debug.Log(gameObject.name + " Handling setUiPositionsNoCallback()");
        offsetDist = globalOffset;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            if (!childDropDowns[i].gameObject.activeSelf)
            {
                continue;
            }
            childDropDowns[i].setUiPositionsNoCallback();
            childDropDowns[i].transform.localPosition = new Vector3(0, -offsetDist, 0);
            //Debug.Log(childDropDowns[i].gameObject.name + " Setting to offset " +  offsetDist);
            offsetDist += childDropDowns[i].getSize();
        }
        setPosHelper();
    }

    public void HandleClick()
    {
        //Debug.Log("HandleClick");
        if (!IsChildOrParent)
        {
            return;
        }
        //Debug.Log("HandleClickInteractable");
        if (!engaged)
        {
            dropDownBackGroundImage.SetActive(true);
            buttonRectTransform.rotation = Quaternion.Euler(0,0,180);
            childrenHolder?.SetActive(true);
        }
        else
        {
            dropDownBackGroundImage.SetActive(false);
            buttonRectTransform.rotation = Quaternion.Euler(0, 0, 0);
            childrenHolder?.SetActive(false);
        }
        engaged = !engaged;
        setUiPositions();
    }

    public void addToChildDropDowns(General_UI_DropDown_Handler_Script scr)
    {
        childDropDowns.Add(scr);
    }

    public void RemoveFromChildDropDowns(General_UI_DropDown_Handler_Script scr)
    {
        childDropDowns.Remove(scr);
    }

    public void clearChildDropDowns()
    {
        childDropDowns.Clear();
    }
}
