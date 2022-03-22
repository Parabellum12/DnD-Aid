using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMarker_Handler_Script : MonoBehaviour
{
    

    public void KILLME()
    {
        Debug.Log("KILL ME!!!!");
        Destroy(gameObject);
    }

    public void setPos(Vector3 pos)
    {
        transform.position = pos;
    }
}
