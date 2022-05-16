using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rightPanelHandler : MonoBehaviour
{
    [SerializeField] Button openCloseButton;
    [SerializeField] Transform parentTransform;
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
            openPos = parentTransform.position.x;
            closePos = openPos + BackgorundImage.rectTransform.sizeDelta.x;
        }
        else
        {
            //starting closed
            closePos = parentTransform.position.x;
            openPos = closePos - BackgorundImage.rectTransform.sizeDelta.x;
        }
    }





    public IEnumerator open()
    {
        while (parentTransform.position.x > openPos)
        {
            parentTransform.position.Set(parentTransform.position.x - (moveSpeed * Time.deltaTime), parentTransform.position.y, 0);
            yield return null;
        }
        yield break;
    }

    public IEnumerator close()
    {
        while (parentTransform.position.x < closePos)
        {
            parentTransform.position.Set(parentTransform.position.x + (moveSpeed * Time.deltaTime), parentTransform.position.y, 0);
            yield return null;
        }
        yield break;
    }

    public void handleClick()
    {
        StopAllCoroutines();
        if (openOrClosed)
        {
            openOrClosed = false;
            StartCoroutine(close());
        }
        else
        {
            openOrClosed = true;
            StartCoroutine(open());
        }
    }

}
