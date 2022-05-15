using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keepMylocalPosZero : MonoBehaviour
{
    [SerializeField] RectTransform recttr;
    // Start is called before the first frame update
    void Start()
    {
        recttr.localPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
