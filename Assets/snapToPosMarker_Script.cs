using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapToPosMarker_Script : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        setPosLockOn();
    }

    void setPosLockOn()
    {
        Vector2 pos = UtilClass.getMouseWorldPosition();
        Vector2 newPos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        transform.position = newPos;
    }
}
