using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Handler_Script : MonoBehaviour
{
    [SerializeField] LineRenderer BackGroundLR;
    [SerializeField] int modValue = 4;
    [SerializeField] Transform mainCam;
    [SerializeField] StraightLine_Handler_ScriptV2 straightLine_Handler;
    [SerializeField] Transform GridSnapMarker;

    // Start is called before the first frame update
    void Start()
    {

    }

    public enum tools
    {
        select,
        DrawStraightLine,
        move
    }

    public tools ActiveTool = tools.DrawStraightLine;

    // Update is called once per frame
    void Update()
    {
        //change selected tools
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveTool = tools.DrawStraightLine;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveTool = tools.move;
        }

        updateBackgroundGrid();

        if (Input.GetMouseButtonDown(0))
        {
            //left click
            if (ActiveTool == tools.DrawStraightLine)
            {
                handleCreateLine();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (ActiveTool == tools.DrawStraightLine)
            {
                needToRemove = false;
                straightLine_Handler.endGuideLine();
                startNew = true;
                pos1 = new Vector2();
            }
        }

        if (!startNew)
        {
            straightLine_Handler.drawGuideLine(new Vector3(pos1.x, pos1.y, -1), new Vector3(GridSnapMarker.transform.position.x, GridSnapMarker.transform.position.y, -1));
            needToRemove = true;
        }
        else if (needToRemove)
        {
            needToRemove = false;
            straightLine_Handler.endGuideLine();
        }
    }

    Vector2 pos1 = new Vector2();
    bool startNew = true;
    bool needToRemove = false;

    private void handleCreateLine()
    {
        if (startNew)
        {
            //new line
            startNew = false;
            pos1 = new Vector2(GridSnapMarker.position.x, GridSnapMarker.position.y);
        }
        else
        {
            //continue line
            Vector2 pos2 = new Vector2(GridSnapMarker.position.x, GridSnapMarker.position.y);
            straightLine_Handler.AddLine(pos1, pos2);
            pos1 = pos2;
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
