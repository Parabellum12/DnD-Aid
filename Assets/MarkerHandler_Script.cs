using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerHandler_Script : MonoBehaviour
{


    private void Update()
    {
        transform.position = UtilClass.getMouseWorldPosition();
        transform.position = new Vector3 (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), -3);
    }
}
