using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class StraightLine_Handler_ScriptV2 : MonoBehaviour
{
    /*
     * Creates and handles straight lines for maps
     */
    List<Point> allPoints = new List<Point>();
    [SerializeField] GameObject LineHolder;
    GameObject guideHolder;
    // Start is called before the first frame update
    void Start()
    {
        LineHolder = new GameObject("Main Line Holder");
        guideHolder = new GameObject("guideLineHolder");
        guideLineRenderer = guideHolder.AddComponent<LineRenderer>();
    }

    public float2[][] getLinePoints()
    {
        float2[][] returner = new float2[allPoints.Count][];
        for (int i = 0; i < allPoints.Count; i++)
        {
            returner[i] = allPoints[i].getAsSaveData();
        }
        return returner;
    }

    public void setDataFromSaveData(float2[][] data)
    {
        deleteAnyHandles();
        clearPoints();
        foreach (float2[] arr in data)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                AddLine(new Vector2(arr[0].x, arr[0].y), new Vector2(arr[i].x, arr[i].y));
            }
        }
    }


    bool needToUpdateLines = false;
    // Update is called once per frame
    void Update()
    {
        if (needToUpdateLines)
        {
            drawLines();
        }
    }

    GameObject[] lineHolders = new GameObject[0];
    LineRenderer[] lineRenderers = new LineRenderer[0];

    public void drawLines()
    {
        needToUpdateLines = false;
        List<Vector4> lines = getLines();
       // Debug.Log("reDraw Lines:" + lines.Count);
        foreach (GameObject g in lineHolders)
        {
            Destroy(g);
        }
        lineHolders = new GameObject[lines.Count];
        lineRenderers = new LineRenderer[lines.Count];
        for (int i = 0; i < lines.Count; i++)
        {
            Vector4 vec4 = lines[i];
            Vector3 start = new Vector3(vec4.x, vec4.y, -1);
            Vector3 end = new Vector3(vec4.z, vec4.w, -1);
            lineHolders[i] = new GameObject("LineHolder:" + i);
            lineHolders[i].transform.parent = LineHolder.transform;
            lineHolders[i].AddComponent<LineRenderer>();
            lineRenderers[i] = lineHolders[i].GetComponent<LineRenderer>();

            lineRenderers[i].positionCount = 2;
            Vector3[] temp = { start, end };
            lineRenderers[i].SetPositions(temp);
            lineRenderers[i].startWidth = 1;
            lineRenderers[i].endWidth = 1;
            lineRenderers[i].useWorldSpace = true;


        }


    }

    [SerializeField] GameObject HandlerPrefab;

    Point pointIn = null;
    Point pointOut = null;

    public bool deleteLine()
    {
        if (pointIn != null)
        {
            deleteAnyHandles();
            pointIn.NextPoints.Remove(pointOut);
            drawLines();
            pointIn = null;
            pointOut = null;
            return true;
        }
        return false;
    }

    public bool HandleIfSelected(Vector2 mousePos)
    {
        Point point1 = null;
        Point point2 = null;

        foreach (Point point in allPoints)
        {
            point.destroyHandle();
            if (point.getIfLineSelected(mousePos, out Point p, out Point p2))
            {
                point1 = p;
                point2 = p2;
            }
        }
        if (point1 != null)
        {
            pointIn = point1;
            pointOut = point2;
            point1.createHandle(HandlerPrefab, () =>
            {
                drawLines();
            });
            point2.createHandle(HandlerPrefab, () =>
            {
                drawLines();
            });
            return true;
        }
        pointIn = null;
        pointOut = null;
        return false;
    }

    public void deleteAnyHandles()
    {
        foreach (Point point in allPoints)
        {
            point.destroyHandle();
        }
    }

    private List<Vector4> getLines()
    {
        List<Vector4> returner = new List<Vector4>();
        //Debug.Log("Lines: " + allPoints.Count);
        foreach (Point p in allPoints)
        {
            p.AlreadyDrawnLines = false;
        }

        foreach (Point p in allPoints)
        {
            if (!p.AlreadyDrawnLines)
            {
                returner.AddRange(p.getLines());
            }
        }
        return returner;
    }

    [SerializeField] LineRenderer guideLineRenderer;
    public void drawGuideLine(Vector3 PointFrom, Vector3 PointTo)
    {
        guideLineRenderer.useWorldSpace = true;
        guideLineRenderer.positionCount = 2;
        guideLineRenderer.startWidth = 1;
        guideLineRenderer.endWidth = 1;
        Vector3[] temp = { PointFrom, PointTo };
        guideLineRenderer.SetPositions(temp);
    }

    public void endGuideLine()
    {
        guideLineRenderer.positionCount = 0;
    }


    public void AddLine(Vector2 start, Vector2 end)
    {
        //Debug.Log("Add Line:" + start.ToString() + "," + end.ToString());
        Point startingPoint = getPointValue(start);
        Point endingPoint = getPointValue(end);
        if (!startingPoint.NextPoints.Contains(endingPoint))
        {
            startingPoint.NextPoints.Add(endingPoint);
        }

        if (!allPoints.Contains(startingPoint))
        {
            allPoints.Add(startingPoint);
        }
        if (!allPoints.Contains(endingPoint))
        {
            allPoints.Add(endingPoint);
        }

        needToUpdateLines = true;
    }

    private Point getPointValue(Vector2 pos)
    {
        foreach (Point p in allPoints)
        {
            if (p.x == pos.x && p.y == pos.y)
            {
                return p;
            }
        }
        return new Point(pos);
    }
    private Point getPointValueOrNull(Vector2 pos)
    {
        foreach (Point p in allPoints)
        {
            if (p.x == pos.x && p.y == pos.y)
            {
                return p;
            }
        }
        return null;
    }

    private Point getPointValueOrNull(Vector2 pos, Point pointToIgnore)
    {
        foreach (Point p in allPoints)
        {
            if (p.x == pos.x && p.y == pos.y && p != pointToIgnore )
            {
                return p;
            }
        }
        return null;
    }



    //handle moving
    Point pointToMove = null;
    bool moveStarted = false;
    public void handleMoveStart(Vector2 pos)
    {
        pointToMove = getPointValueOrNull(pos);
        if (pointToMove != null)
        {
            moveStarted = true;
        }
    }

    public void handleMoveEnd()
    {
        if (!moveStarted)
        {
            return;
        }
        Point EndPointTest = getPointValueOrNull(new Vector2(pointToMove.x, pointToMove.y), pointToMove);
        if (EndPointTest != null)
        {
            //moved over another point
            List<Point> pointedToMovedPoint = getPointsPointingTo(pointToMove);
            foreach (Point p in pointedToMovedPoint)
            {
                p.NextPoints.Remove(pointToMove);
                p.NextPoints.Add(EndPointTest);
            }
            foreach (Point p in pointToMove.NextPoints)
            {
                if (!EndPointTest.NextPoints.Contains(p))
                {
                    EndPointTest.NextPoints.Add(p);
                }
            }
            allPoints.Remove(pointToMove);
        }
        pointToMove = null;
        moveStarted = false;
        needToUpdateLines = true;
    }

    private List<Point> getPointsPointingTo(Point point)
    {
        List<Point> returner = new List<Point>();
        foreach (Point p in allPoints)
        {
            if (p.NextPoints.Contains(point) && p != point)
            {
                returner.Add(p);
            }
        }
        return returner;
    }

    public void handleMove(Vector2 pos)
    {
        if (moveStarted)
        {
            pointToMove.x = pos.x;
            pointToMove.y = pos.y;
            needToUpdateLines=true;
        }
    }




    

    //save load

    public string ReturnSavePointsAsString()
    {
       // getLines();
        List<string> returnerData = new List<string>();
        //Debug.Log("allPoints:" + allPoints.Count);
        foreach (Point p in allPoints)
        {
            returnerData.AddRange(p.getLinesAsSaveList());
            //Debug.Log(p.getLinesAsSaveList());
        }
        //SLTD = straight line total data
        string returner = "SLTD[";
        if (returnerData.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < returnerData.Count; i++)
        {
            //Debug.Log(returnerData[i]);
            returner += returnerData[i];
            if (i != returnerData.Count - 1)
            {
                returner += "|";
            }
        }
        return returner;
    }

    public void LoadFromSavePointsAsString(string s)
    {
        string[] tag = s.Split('[');
        if (tag[0].Equals("SLTD"))
        {
            //correct tag
            //Debug.Log("StraightLine SaveTag Correct:" + s);
            allPoints.Clear();
            string[] data = tag[1].Split('|');
            List<string> lines = new List<string>();
            foreach (string str in data)
            {
                lines.Add(str);
            }
            LoadFromSavePointsAsStringList(lines);
        }

    }

    public void LoadFromSavePointsAsStringList(List<string> lines)
    {
        foreach (string s in lines)
        {
            string[] data = s.Split(':');
            if (data[0].Equals("SLCD"))
            {
                string[] dataValues = data[1].Split(',');
                if (dataValues.Length != 4)
                {
                    Debug.Log("Error: Incorrect Line Data Points:" + data[0]);
                }
                else
                {
                    Vector2 start = new Vector2(int.Parse(dataValues[0]), int.Parse(dataValues[1]));
                    Vector2 end = new Vector2(int.Parse(dataValues[2]), int.Parse(dataValues[3]));
                    AddLine(start, end);
                }
            }
            else
            {
                Debug.Log("Error: Incorrect Line Load Tag:" + s);
            }
        }

    }

    //testing stuff

    public void clearPoints()
    {
        foreach (GameObject g in lineHolders)
        {
            Destroy(g);
        }
        lineHolders = new GameObject[0];
        lineRenderers = new LineRenderer[0];
        allPoints.Clear();
    }



    public class Point
    {
        public float x;
        public float y;
        public List<Point> NextPoints;
        public bool AlreadyDrawnLines;
        public Point(Vector2 pos)
        {
            x = pos.x;
            y = pos.y;
            NextPoints = new List<Point>();
            AlreadyDrawnLines = false;
        }

        public float2 getXYAsFloat2()
        {
            return new float2(x, y);
        }

        public float2[] getAsSaveData()
        {
            float2[] returner = new float2[NextPoints.Count+1];
            returner[0] = getXYAsFloat2();
            for (int i = 0; i < NextPoints.Count; i++)
            {
                returner[i + 1] = NextPoints[i].getXYAsFloat2();
            }
            return returner;
        }



        public List<Vector4> getLines()
        {
            List<Vector4> temp = new List<Vector4>();
            foreach (Point p in NextPoints)
            {
                temp.Add(new Vector4(x, y, p.x, p.y));
            }
            return temp;
        }

        public Vector2 getPos()
        {
            return new Vector2(x, y);
        }

        public bool getIfLineSelected(Vector2 mousePos, out Point p, out Point p2)
        {
            Point nextPoint = null;
            foreach (Point poi in NextPoints)
            {
                for (float i = 0; i < 1; i+=0.001f)
                {
                    if (Vector2.Distance(Vector2.Lerp(getPos(), poi.getPos(), i), mousePos) < 1f)
                    {
                        nextPoint = poi;
                    }
                }
            }
            if (nextPoint != null)
            {
                p = this;
                p2 = nextPoint;
                return true;
            }
            p = null;
            p2 = null;
            return false;
        }

        HandleMarker_Handler_Script handle = null;
        public void createHandle(GameObject handlePrefab, System.Action drawLinesCallBack)
        {
            GameObject temp = Instantiate(handlePrefab);
            HandleMarker_Handler_Script scr = temp.GetComponent<HandleMarker_Handler_Script>();
            Vector3 handlePos = getPos();
            handlePos.z = -2;
            scr.setPos(handlePos);
            handle = scr;
            scr.StartCoroutine(scr.returnMyPos(0, (vec, index) =>
            {
                x = vec.x;
                y = vec.y;
                drawLinesCallBack.Invoke();
            }));
        }

        public void destroyHandle()
        {
            if (handle != null)
            {
                handle.KILLME();
            }
        }



        public List<string> getLinesAsSaveList()
        {
            List<string> returner = new List<string>();
            foreach (Point pEnd in NextPoints)
            {
                returner.Add("SLCD:" + x + "," + y + "," + pEnd.x + "," + pEnd.y);
            }
            return returner;
        }

        

    }
}
