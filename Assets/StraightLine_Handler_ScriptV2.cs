using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLine_Handler_ScriptV2 : MonoBehaviour
{
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
        Debug.Log("reDraw Lines:" + lines.Count);
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
            lineHolders[i] = new GameObject("LineHolder:+i");
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

    private List<Vector4> getLines()
    {
        List<Vector4> returner = new List<Vector4>();
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
        Debug.Log("Add Line:" + start.ToString() + "," + end.ToString());
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
            allPoints.Add((endingPoint));
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

        public List<Vector4> getLines()
        {
            List<Vector4> temp = new List<Vector4>();
            foreach (Point p in NextPoints)
            {
                temp.Add(new Vector4(x, y, p.x, p.y));
            }
            return temp;
        }
    }
}
