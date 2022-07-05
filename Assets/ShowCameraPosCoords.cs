using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowCameraPosCoords : MonoBehaviour
{
    [SerializeField] TMP_Text text;
     
    // Update is called once per frame
    void Update()
    {
        text.text = "(" + convertfloatToPosCoords(Camera.main.transform.position.x) + "," + convertfloatToPosCoords(Camera.main.transform.position.y) + ")";
    }

    string convertfloatToPosCoords(float input)
    {
        int temp = Mathf.FloorToInt(input * 10f);
        float temp2 = temp / 10;
        return temp2.ToString();
    }
}
