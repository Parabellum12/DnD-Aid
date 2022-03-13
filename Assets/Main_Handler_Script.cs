using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Handler_Script : MonoBehaviour
{
    [SerializeField] LineRenderer BackGroundLR;
    [SerializeField] int modValue = 4;
    [SerializeField] Transform mainCam;
    [SerializeField] StraightLine_Handler_Script straightLine_Handler;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateBackgroundGrid();

        if (Input.GetMouseButtonDown(0))
        {
            straightLine_Handler.handleLeftClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            straightLine_Handler.handleRightClick();
        }

    }

    List<Vector3> pointsList = new List<Vector3>();
    void updateBackgroundGrid()
    {
        pointsList = new List<Vector3>();
        Vector2Int camPos = new Vector2Int(Mathf.FloorToInt(mainCam.position.x), Mathf.FloorToInt(mainCam.position.y));

        for (int x = camPos.x - 180; x <= camPos.x + 180; x++)
        {
            if (x % modValue != 0)
            {
                continue;
            }
            pointsList.Add(new Vector3(x, camPos.y - 100, 0));
            pointsList.Add(new Vector3(x, camPos.y + 100, 0));
            pointsList.Add(new Vector3(x, camPos.y - 100, 0));
        }
        for (int y = camPos.y - 100; y < camPos.y+100; y++)
        {
            if (y % modValue != 0)
            {
                continue;
            }
            pointsList.Add(new Vector3(camPos.x - 180, y, 0));
            pointsList.Add(new Vector3(camPos.x + 180, y, 0));
            pointsList.Add(new Vector3(camPos.x - 180, y, 0));
        }




        BackGroundLR.positionCount = pointsList.Count;
        //Debug.Log("done");
        BackGroundLR.SetPositions(pointsList.ToArray());
    }
}
