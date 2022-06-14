using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLineHandler_Script : MonoBehaviour
{
    List<Point> allPoints = new List<Point>();
    GameObject lineRendererHolder;
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    LineRenderer guideLineRendederer;

    private void Start()
    {
        lineRendererHolder = new GameObject("Straight Line LineRender Holder");
        lineRendererHolder.transform.position = new Vector3(0,0, -2);
        guideLineRendederer = new GameObject("GuideLineHolder").AddComponent<LineRenderer>();
        guideLineRendederer.gameObject.transform.parent = lineRendererHolder.transform;
        currentState = State.NewLine;
    }

    enum State
    {
        NewLine,
        ContinueLine
    }
    State currentState = State.NewLine;

    Point lastPointAdded = null;

    public void AddPoint(Vector2 pos, bool alsoDrawLines)
    {
        Point newPoint = new Point(pos);
        if (currentState == State.ContinueLine)
        {
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
    }

    public void EndCurrentLine()
    {
        currentState = State.NewLine;
        lastPointAdded = null;
    }

    public void RemovePoint(Vector2 pos, bool alsoDrawLines)
    {
        Point pToRemove = null;
        foreach (Point p in allPoints)
        {
            if (p.GetPos().Equals(pos))
            {
                RemovePoint(p);
                pToRemove = p;
            }
        }
        if (pToRemove != null)
        {
            allPoints.Remove(pToRemove);
        }
        if (alsoDrawLines)
        {
            DrawLines();
        }
    }

    public void HandleGuideLine(Vector2 Pos)
    {
        if (currentState == State.ContinueLine)
        {
            guideLineRendederer.positionCount = 2;
            guideLineRendederer.SetPosition(0, new Vector3(lastPointAdded.GetPos().x, lastPointAdded.GetPos().y, -2));
            guideLineRendederer.SetPosition(1, new Vector3(Pos.x, Pos.y, -2));
        }
        else if (currentState == State.NewLine)
        {
            guideLineRendederer.positionCount = 0;
        }
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
        lr.SetPosition(0, new Vector3(line.x, line.y, -2));
        lr.SetPosition(1, new Vector3(line.z, line.w, -2));
        allLineRenderers.Add(lr);
    }


    class Point
    {
        Vector2 pos;
        List<Point> PointsIPointTo = new List<Point>();

        public Point(Vector2 pos)
        {
            this.pos = pos;
        }

        public void AddPoint(Point nextPoint)
        {
            PointsIPointTo.Add(nextPoint);
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
            return pos;
        }
    }
}
