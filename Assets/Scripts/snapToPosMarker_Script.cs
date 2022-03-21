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
        Vector3 newPos = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), -9);
        transform.position = newPos;
    }
}
