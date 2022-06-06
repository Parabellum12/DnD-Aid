using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundLine_Script : MonoBehaviour
{
    /*
     * creates the background lines
     */
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        updateBackgroundGrid();
    }

    [SerializeField] LineRenderer BackGroundLR;
    [SerializeField] int modValue = 8;
    [SerializeField] Transform mainCam;
    [SerializeField] Camera cam;
    List<Vector3> pointsList = new List<Vector3>();
    void updateBackgroundGrid()
    {
        pointsList = new List<Vector3>();
        Vector2Int camPos = new Vector2Int(Mathf.FloorToInt(mainCam.position.x), Mathf.FloorToInt(mainCam.position.y));
        float size = cam.orthographicSize;
        int gridSize = Mathf.FloorToInt(180 * (size / 50f));
        for (int x = camPos.x - gridSize; x <= camPos.x + gridSize; x++)
        {
            if (x % modValue != 0)
            {
                continue;
            }
            pointsList.Add(new Vector3(x, camPos.y - gridSize, 0));
            pointsList.Add(new Vector3(x, camPos.y + gridSize, 0));
            pointsList.Add(new Vector3(x, camPos.y - gridSize, 0));
        }
        for (int y = camPos.y - gridSize; y < camPos.y + gridSize; y++)
        {
            if (y % modValue != 0)
            {
                continue;
            }
            pointsList.Add(new Vector3(camPos.x - gridSize, y, 0));
            pointsList.Add(new Vector3(camPos.x + gridSize, y, 0));
            pointsList.Add(new Vector3(camPos.x - gridSize, y, 0));
        }




        BackGroundLR.positionCount = pointsList.Count;
        //Debug.Log("done");
        BackGroundLR.SetPositions(pointsList.ToArray());
    }
}
