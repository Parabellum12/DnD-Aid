using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keepMylocalPosZero : MonoBehaviour
{
    /*
     * keeps the scroll views x pos 0 b/e for what ever reason it want to leave for infinity all the time
     */
    [SerializeField] RectTransform recttr;
    // Start is called before the first frame update
    void Start()
    {
        recttr.localPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        recttr.localPosition = new Vector2(256, recttr.localPosition.y);
    }
}
