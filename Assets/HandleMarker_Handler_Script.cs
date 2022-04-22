using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMarker_Handler_Script : MonoBehaviour
{

    private void Start()
    {
        
    }
    public void KILLME()
    {
        Debug.Log("KILL ME!!!!");
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
            callback.Invoke(transform.position, pos);
            yield return null;
        }
    }

   
}
