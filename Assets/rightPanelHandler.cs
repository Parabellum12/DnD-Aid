using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rightPanelHandler : MonoBehaviour
{
    [SerializeField] Button openCloseButton;
    [SerializeField] RectTransform parentTransform;
    [SerializeField] Image BackgorundImage;
    [SerializeField] float moveSpeed;
    bool openOrClosed = true;
    bool moving = false;


    [SerializeField] bool startingOpenOrClosed;

    float openPos;
    float closePos;

    private void Start()
    {
        if (startingOpenOrClosed)
        {
            //starting open
            openPos = parentTransform.localPosition.x;
            closePos = openPos + BackgorundImage.rectTransform.sizeDelta.x;
        }
        else
        {
            //starting closed
            closePos = parentTransform.localPosition.x;
            openPos = closePos - BackgorundImage.rectTransform.sizeDelta.x;
        }
        Debug.Log("OpenPos:" + openPos + " ClosePos:" + closePos);
    }





    public IEnumerator open()
    {
        while (parentTransform.localPosition.x > openPos)
        {
            parentTransform.localPosition = new Vector2(parentTransform.localPosition.x - (moveSpeed * Time.deltaTime), parentTransform.localPosition.y);
            yield return null;
        }
        parentTransform.localPosition = new Vector2(openPos, parentTransform.localPosition.y);
        yield break;
    }

    public IEnumerator close()
    {
        while (parentTransform.localPosition.x < closePos)
        {
            parentTransform.localPosition = new Vector2(parentTransform.localPosition.x + (moveSpeed * Time.deltaTime), parentTransform.localPosition.y);
            yield return null;
        }
        parentTransform.localPosition = new Vector2(closePos, parentTransform.localPosition.y);
        yield break;
    }

    public void handleClick()
    {
        StopAllCoroutines();
        if (openOrClosed)
        {
            Debug.Log("Close");
            openOrClosed = false;
            openCloseButton.transform.rotation = Quaternion.Euler(0, 0, 180);
            StartCoroutine(close());
        }
        else
        {
            Debug.Log("Open");
            openOrClosed = true;
            openCloseButton.transform.rotation = Quaternion.Euler(0, 0, 0);
            StartCoroutine(open());
        }
    }

}