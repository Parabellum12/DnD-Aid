using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General_UI_DropDown_Handler_Script : MonoBehaviour
{
    [SerializeField] Button InteractButton = null;
    [SerializeField] Image mainImage = null;
    [SerializeField] GameObject dropDownBackGroundImage = null;
    [SerializeField] float offsetDist = 0;
    [SerializeField] float globalOffset = 0;
    [SerializeField] List<General_UI_DropDown_Handler_Script> childDropDowns = new List<General_UI_DropDown_Handler_Script>();
    bool engaged = false;
    public System.Action updateUICallBack = null;
    [SerializeField] bool interactable = true;
    [SerializeField] bool dropDownImageFollowChilrenSize = true;
    [SerializeField] GameObject childrenHolder = null;



    /*
     * global offset must be set to size of children icons
     * all children icons must be same size
     * 
     */




    // Start is called before the first frame update
    void Start()
    {

        if (interactable)
        {
            engaged = false;
            dropDownBackGroundImage?.SetActive(false);
            childrenHolder?.SetActive(false);
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
        if (dropDownBackGroundImage != null && dropDownBackGroundImage.activeSelf)
        {
            Debug.Log(gameObject.name + ":returnAllsize");
            return dropDownBackGroundImage.GetComponent<RectTransform>().rect.height + mainImage.GetComponent<RectTransform>().rect.height;
        }
        else
        {
            Debug.Log(gameObject.name + ":returnCLosedsize");
            return mainImage.GetComponent<RectTransform>().rect.height;
        }
    }

    [SerializeField] RectTransform contentRectTransform;

    public void setUiPositions()
    {

        offsetDist = globalOffset;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            Debug.Log(gameObject.name+":settingPosOF: " + childDropDowns [i].gameObject.name+ " to " + offsetDist);
            childDropDowns[i].transform.localPosition = new Vector2(childDropDowns[i].transform.localPosition.x, -offsetDist);
            offsetDist += childDropDowns[i].getSize();
        }
        if (interactable)
        {
            if (dropDownImageFollowChilrenSize)
            {
                RectTransform temp = dropDownBackGroundImage.GetComponent<RectTransform>();
                Debug.Log(gameObject.name + ":rectFollowSize");
                //temp.rect.Set(temp.rect.x, temp.rect.y, temp.rect.width, offsetDist - globalOffset);
                temp.sizeDelta = new Vector2(temp.rect.width, offsetDist - globalOffset);
                Debug.Log(gameObject.name + ":TempHeight5:" + ((temp.rect.size.y / 2f) - globalOffset));
                //temp.localPosition = Vector2.zero;
                temp.localPosition = new Vector2(0, -(temp.rect.size.y / 2f) - (globalOffset/2));
                Debug.Log(gameObject.name + ":TemplocalPos:" + temp.localPosition);
            }
        }
        else
        {
            contentRectTransform.sizeDelta = new Vector2(512, offsetDist-globalOffset);
        }
        updateUICallBack?.Invoke();
    }

    public void setUiPositionsNoCallback()
    {
        Debug.Log(gameObject.name + " Handling setUiPositionsNoCallback()");
        offsetDist = globalOffset;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            childDropDowns[i].setUiPositionsNoCallback();
            childDropDowns[i].transform.localPosition = new Vector3(childDropDowns[i].transform.localPosition.x, -offsetDist, 0);
            Debug.Log(childDropDowns[i].gameObject.name + " Setting to offset " +  offsetDist);
            offsetDist += childDropDowns[i].getSize();
        }
        if (interactable)
        {
            RectTransform temp = dropDownBackGroundImage.GetComponent<RectTransform>();
            if (dropDownImageFollowChilrenSize)
            {
                //Debug.Log("rectFollowSize");
                //temp.rect.Set(temp.rect.x, temp.rect.y, temp.rect.width, offsetDist - globalOffset);
                temp.sizeDelta = new Vector2(temp.rect.width, offsetDist - globalOffset);
                //Debug.Log("TempHeight:" + (float)temp.rect.height / 2f);
                temp.localPosition = Vector2.zero;
                temp.localPosition = new Vector2(0, -(float)temp.rect.size.y / 2f);
            }
        }
        else
        {
            contentRectTransform.sizeDelta = new Vector2(512, offsetDist - globalOffset);
        }
    }

    public void HandleClick()
    {
        Debug.Log("HandleClick");
        if (!interactable)
        {
            return;
        }
        Debug.Log("HandleClickInteractable");
        if (!engaged)
        {
            dropDownBackGroundImage.SetActive(true);
            childrenHolder?.SetActive(true);
        }
        else
        {
            dropDownBackGroundImage.SetActive(false);
            childrenHolder?.SetActive(false);
        }
        engaged = !engaged;
        setUiPositions();
    }
}
