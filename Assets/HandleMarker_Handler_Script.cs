using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMarker_Handler_Script : MonoBehaviour
{

    Main_Handler_Script mainHandleSCR;
    private void Start()
    {
        mainHandleSCR = GameObject.Find("Handler").GetComponent<Main_Handler_Script>();
    }
    public void KILLME()
    {
        //Debug.Log("KILL ME!!!!");
        if (mainHandleSCR.selectedHandle == this)
        {
            mainHandleSCR.selectedHandle = null;
        }
        Destroy(gameObject);
    }

    public void setPos(Vector3 pos)
    {
        transform.position = pos;
    }



    public IEnumerator returnMyPos(int pos, System.Action<Vector3, int> callback)
    {
        while (true)
        {
            Vector3 newPos = transform.position;
            newPos.z = -2;
            callback.Invoke(newPos, pos);
            yield return null;
        }
    }


    private void OnMouseEnter()
    {
        mainHandleSCR.selectedHandle = this;
    }

    private void OnMouseExit()
    {
        if (mainHandleSCR.selectedHandle == this && !Input.GetMouseButton(0))
        {
            mainHandleSCR.selectedHandle = null;
        }
    }
}
