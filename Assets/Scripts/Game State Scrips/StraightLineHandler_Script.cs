using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLineHandler_Script : MonoBehaviour
{
    List<Point> allPoints = new List<Point>();
    GameObject lineRendererHolder;
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    LineRenderer guideLineRendederer;

    Vector2 OriginPos = Vector2.zero;
    [SerializeField] float zPos = -2;

    [SerializeField] Material straightLineMaterial;


    private void Start()
    {
        lineRendererHolder = new GameObject("Straight Line LineRender Holder");
        lineRendererHolder.transform.position = new Vector3(0,0, -2);
        guideLineRendederer = new GameObject("GuideLineHolder").AddComponent<LineRenderer>();
        guideLineRendederer.gameObject.transform.parent = lineRendererHolder.transform;
        currentState = State.NewLine;
    }

    public enum State
    {
        NewLine,
        ContinueLine
    }
    State currentState = State.NewLine;

    public State ReturnState()
    {
        return currentState;
    }

    Point lastPointAdded = null;
    int currentLineCount = 0;
    public void AddPoint(Vector2 pos, bool alsoDrawLines)
    {
        Point newPoint = new Point(pos, OriginPos);
        if (currentState == State.ContinueLine)
        {
            currentLineCount++;
            lastPointAdded.AddPoint(newPoint);
        }
        else
        {
            currentState = State.ContinueLine;
        }
        lastPointAdded = newPoint;
        allPoints.Add(newPoint);
        if (alsoDrawLines)
        {
            DrawLines();
        }
        if (currentLineCount > 0)
        {
            PrunePoints();
        }
    }

    public void setPoints(List<Point> newPoints)
    {
        allPoints.Clear();
        allPoints.AddRange(newPoints);
        DrawLines();
    }

    public List<Point> GetAllPoints()
    {
        return allPoints;
    }

    public void EndCurrentLine()
    {
        currentState = State.NewLine;
        currentLineCount = 0;
        lastPointAdded = null;
    }

    public void HandleGuideLine(Vector2 Pos)
    {
        if (currentState == State.ContinueLine)
        {
            guideLineRendederer.positionCount = 2;
            guideLineRendederer.SetPosition(0, new Vector3(lastPointAdded.GetPos().x, lastPointAdded.GetPos().y, zPos));
            guideLineRendederer.SetPosition(1, new Vector3(Pos.x, Pos.y, zPos));
        }
        else if (currentState == State.NewLine)
        {
            guideLineRendederer.positionCount = 0;
        }
    }

    public List<Point> GetSaveData()
    {
        return allPoints;
    }

    public void LoadSaveData(List<Point> points)
    {
        allPoints = points;
        DrawLines();
    }





    void RemovePoint(Point p)
    {
        foreach (Point p2 in allPoints)
        {
            p2.RemovePoint(p);
        }
    }


    public void DrawLines()
    {
        foreach (LineRenderer lr in allLineRenderers)
        {
            Destroy(lr.gameObject);
        }
        allLineRenderers.Clear();
        foreach (Point p in allPoints)
        {
            Vector4[] lines = p.GetAllLines();
            for (int i = 0; i < lines.Length; i++)
            {
                DrawLine(lines[i]);
            }
        }
    }

    void DrawLine(Vector4 line)
    {
        GameObject go = new GameObject("LineHolder");
        go.transform.parent = lineRendererHolder.transform;
        go.transform.position = Vector3.zero;
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        Vector3 start = new Vector3(line.x, line.y, zPos);
        Vector3 end = new Vector3(line.z, line.w, zPos);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        allLineRenderers.Add(lr);
        lr.material = straightLineMaterial;
    }

    void PrunePoints()
    {
        //Debug.Log("hi");
        //collapse points in same pos into 1, remove unneeded points
        Vector2[] points = new Vector2[allPoints.Count];
        List<Vector2Int> samePoints = new List<Vector2Int>();
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = allPoints[i].GetPos();
            for (int j = 0; j < i; j++)
            {
                if (points[i].Equals(points[j]) && j != i)
                {
                    samePoints.Add(new Vector2Int(i, j));
                }
            }
        }
        for (int i = 0; i < samePoints.Count; i++)
        {
            for (int j = 0; j < allPoints.Count; j++)
            {
                if (j != samePoints[i].x && j != samePoints[i].y)
                {
                    allPoints[j].ReplacePointWithNewPoint(allPoints[samePoints[i].x], allPoints[samePoints[i].y]);
                }
            }

            allPoints[samePoints[i].y].AddPoint(allPoints[samePoints[i].x].ReturnPointsIPointTo().ToArray());
            allPoints.Remove(allPoints[samePoints[i].x]);
        }

    }

    int maxLineTestResolution = 1000;

    public bool HandleSelect(Vector2 pos)
    {
        float startTime = Time.realtimeSinceStartup;
        foreach (Point p in allPoints)
        {
            foreach (Vector4 lines in p.GetAllLines())
            {
                Vector2 a = new Vector2(lines.x, lines.y);
                Vector2 b = new Vector2(lines.z, lines.w);
                //Debug.Log("CurvedLine HandleSelect:" + a + "->" + b + ":" + pos);
                if (UtilClass.isPointWithinDistanceToLine(a, b, pos, 1.25f))
                {
                    //generateHandles;
                    //Debug.Log("Straight HandleSelect");
                    updateLineTestResolution((Time.realtimeSinceStartup - startTime) > .1f, Time.realtimeSinceStartup - startTime);
                    return true;
                }

            }
        }

        updateLineTestResolution((Time.realtimeSinceStartup - startTime) > .1f, Time.realtimeSinceStartup - startTime);
        return false;
    }
    private void updateLineTestResolution(bool WasTooLong, float magnitude)
    {
        float mutliBy = 1;
        if (WasTooLong)
        {
            mutliBy = -1f;
        }

        int subBy = 0;
        if (magnitude < .2f)
        {
            subBy = 50;
        }
        else if (magnitude < .3f)
        {
            subBy = 100;
        }
        else if (magnitude < .4f)
        {
            subBy = 200;
        }
        else if (magnitude < .5f)
        {
            subBy = 300;
        }
        else
        {
            subBy = 500;
        }


        maxLineTestResolution += Mathf.RoundToInt(subBy * mutliBy);

        maxLineTestResolution = Mathf.Clamp(maxLineTestResolution, 1, 1000);
    }


    public class Point
    {
        Vector2 pos;
        List<Point> PointsIPointTo = new List<Point>();
        Vector2 OriginPos = Vector2.zero;
        public Point(Vector2 pos, Vector2 originPos)
        {
            this.pos = pos;
            OriginPos = originPos;
        }

        public void AddPoint(Point nextPoint)
        {
            PointsIPointTo.Add(nextPoint);
        }

        public void AddPoint(Point[] nextPoints)
        {
            PointsIPointTo.AddRange(nextPoints);
        }

        public void RemovePoint(Point nextPoint)
        {
            PointsIPointTo.Remove(nextPoint);
        }

        public Vector4[] GetAllLines()
        {
            Vector4[] returner = new Vector4[PointsIPointTo.Count];
            for (int i = 0; i < PointsIPointTo.Count; i++)
            {
                Vector2 pPos = PointsIPointTo[i].GetPos();
                returner[i] = new Vector4(pos.x, pos.y, pPos.x, pPos.y);
            }
            return returner;
        }

        public Vector2 GetPos()
        {
            return pos + OriginPos;
        }

        public List<Point> ReturnPointsIPointTo()
        {
            return PointsIPointTo;
        }

        public void ReplacePointWithNewPoint(Point oldPoint, Point newPoint)
        {
            bool addNew = false;
            foreach (Point point in PointsIPointTo)
            {
                if (point.Equals(oldPoint))
                {
                    PointsIPointTo.Remove(oldPoint);
                    addNew = true;
                    break;
                }
            }
            if (addNew)
            {
                PointsIPointTo.Add(newPoint);
            }
        }
    }
}
