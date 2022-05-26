using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General_UI_DropDown_Handler_ScriptV2 : MonoBehaviour
{
    /*
     * 
     * 
     * turns out i did have the time and will to create a v2, who would've thought a few hrs would do this to me
     * 
     */
    [SerializeField] bool isHolder = false;
    [SerializeField] bool isCanvasOrUiItem = false;
    [SerializeField] bool dropDownImageFollowChildSize = true;

    [SerializeField] bool engaged = false;

    [SerializeField] Image mainImage = null;
    [SerializeField] GameObject dropDownBackgroundImage = null;
    [SerializeField] Button InteractionButton = null;
    [SerializeField] RectTransform contentRectTransform;

    [SerializeField] List<General_UI_DropDown_Handler_ScriptV2> childDropDowns = new List<General_UI_DropDown_Handler_ScriptV2>();

    public System.Action updateUICallback = null;

    [SerializeField] RectTransform buttonRectTransform;
    [SerializeField] GameObject ChildrenObjectHolder;

    [SerializeField] float globalOffsetDist = 0;

    [SerializeField] float offsetDist = 0;





    private void Start()
    {
        if (isCanvasOrUiItem)
        {
            engaged = false;

            if (dropDownBackgroundImage != null)
            {
                dropDownBackgroundImage.SetActive(false);
            }

            if (ChildrenObjectHolder != null)
            {
                ChildrenObjectHolder.SetActive(false);
                dropDownBackgroundImage.SetActive(false);
                buttonRectTransform.transform.rotation = Quaternion.identity;
            }

            if (InteractionButton != null)
            {
                InteractionButton.onClick.AddListener(() =>
                {
                    handleClick();
                });
            }
        }
        
        foreach (General_UI_DropDown_Handler_ScriptV2 scr in childDropDowns)
        {
            scr.updateUICallback = () =>
            {
                setUIPositions();
            };
        }
    }

    public float getSize()
    {
        if (dropDownBackgroundImage != null && dropDownBackgroundImage.activeSelf && !isHolder)
        {
            return dropDownBackgroundImage.GetComponent<RectTransform>().rect.height + mainImage.rectTransform.rect.height;
        }
        else
        {
            return mainImage.rectTransform.rect.height;
        }
    }

    //called after setup and child or self changed
    public void setUIPositions()
    {
        offsetDist = globalOffsetDist;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            if (!childDropDowns[i].gameObject.activeSelf)
            {
                continue;
            }
            childDropDowns[i].transform.localPosition = new Vector3(0, -offsetDist - (childDropDowns[i].getSize() / 2), 0);
            offsetDist += childDropDowns[i].getSize();
        }
        setPosHelper();
        updateUICallback?.Invoke();
    }

    //call to setup
    public void setUIPositionsNoCallback()
    {
        offsetDist = globalOffsetDist;
        for (int i = 0; i < childDropDowns.Count; i++)
        {
            if (!childDropDowns[i].gameObject.activeSelf)
            {
                continue;
            }
            childDropDowns[i].setUIPositionsNoCallback();
            childDropDowns[i].transform.localPosition = new Vector3(0, -offsetDist - (childDropDowns[i].getSize()/2), 0);
            offsetDist += childDropDowns[i].getSize();
        }
        setPosHelper();
    }

    void setPosHelper()
    {
        if (isHolder)
        {
            return;
        }
        if (isCanvasOrUiItem)
        {
            if (dropDownImageFollowChildSize)
            {
                RectTransform temp = dropDownBackgroundImage.GetComponent<RectTransform>();
                //Debug.Log(gameObject.name + ":rectFollowSize");
                //temp.rect.Set(temp.rect.x, temp.rect.y, temp.rect.width, offsetDist - globalOffset);
                temp.sizeDelta = new Vector2(temp.rect.width, offsetDist - globalOffsetDist);
                //Debug.Log(gameObject.name + ":TempHeight5:" + ((temp.rect.size.y / 2f) - globalOffset));
                //temp.localPosition = Vector2.zero;
                temp.localPosition = new Vector2(0, -(temp.rect.size.y / 2) - (gameObject.GetComponent<RectTransform>().rect.height / 2));
                //Debug.Log(gameObject.name + ":TemplocalPos:" + temp.localPosition);
            }
        }
        else
        {
            contentRectTransform.sizeDelta = new Vector2(512, offsetDist - globalOffsetDist);
        }
    }

    public void handleClick()
    {
        if (isCanvasOrUiItem)
        {
            return;
        }

        if (!engaged)
        {
            dropDownBackgroundImage.SetActive(true);
            buttonRectTransform.rotation = Quaternion.Euler(0,0,180);
            ChildrenObjectHolder.SetActive(true);
        }
        else
        {
            dropDownBackgroundImage.SetActive(false);
            buttonRectTransform.rotation = Quaternion.Euler(0, 0, 0);
            ChildrenObjectHolder.SetActive(false);
        }

        engaged = !engaged;
        setUIPositions();
    }

    public void addToChildDropDowns(General_UI_DropDown_Handler_ScriptV2 childToAdd)
    {
        childDropDowns.Add(childToAdd);
    }

    public void removeFromChildDropDowns(General_UI_DropDown_Handler_ScriptV2 childToRemove)
    {
        childDropDowns.Remove(childToRemove);
    }

    public void clearChildDropDowns()
    {
        childDropDowns.Clear();
    }


}
