using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMarker_Handler_Script : MonoBehaviour
{

    /*
     * handles the markers for editing lines
     */
    Main_Handler_Script mainHandleSCR;
    private void Awake()
    {
        mainHandleSCR = GameObject.Find("Handler").GetComponent<Main_Handler_Script>();
    }
    public void KILLME()
    {
        //Debug.Log("KILL ME!!!!");
        if (mainHandleSCR.getSelectedHandle() == this)
        {
            mainHandleSCR.setSelectedHandle(null);
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

    bool mouseOver = false;

    private void OnMouseEnter()
    {
        mainHandleSCR.setSelectedHandle(this);
        mainHandleSCR.lockInCurrentSelectedHandle = true;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        if (mainHandleSCR.getSelectedHandle() == this && !Input.GetMouseButton(0))
        {
            mainHandleSCR.setSelectedHandle(null);
        }
    }

    private void Update()
    {
        if (!mouseOver && mainHandleSCR.getSelectedHandle() == this && !Input.GetMouseButton(0))
        {
            mainHandleSCR.lockInCurrentSelectedHandle = false;
            mainHandleSCR.setSelectedHandle(null);
        }
    }
}
