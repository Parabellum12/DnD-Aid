using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLineHandler_Script : MonoBehaviour
{
    List<CurvedLine> allLines = new List<CurvedLine>();
    CurvedLine lastAddedLine;
    GameObject CurvedLineHolder;
    public int maxLineCount = 9;
    [SerializeField] int maxResCount = 1000;
    private void Start()
    {
        CurvedLineHolder = new GameObject("CurvedLineHolder");
    }

    public enum State
    {
        NewLine,
        ContinueLine
    }
    State currentState = State.NewLine;
    Vector2 lastPointAdded;
    public void AddPoint(Vector2 pos)
    {
        if (currentState == State.NewLine)
        {
            currentState = State.ContinueLine;
            lastAddedLine = new CurvedLine(CurvedLineHolder);
            allLines.Add(lastAddedLine);
        }
        lastAddedLine.AddPoint(pos);
        lastPointAdded = pos;
        if (lastAddedLine.GetPointCount() == maxLineCount)
        {
            EndLine();
        }
        else
        {
            DrawLine(lastAddedLine, true);
        }
    }

    public void EndLine()
    {
        if (currentState == State.NewLine)
        {
            return;
        }
        currentState = State.NewLine;
        DrawLine(lastAddedLine, true);
        lastAddedLine = null;
    }

    Vector2 lastPointGuideLineDrawnAt;

    public void HandleGuideLine(Vector2 pos)
    {
        if (currentState == State.ContinueLine && lastAddedLine != null && !lastPointGuideLineDrawnAt.Equals(pos))
        {
            lastPointGuideLineDrawnAt = pos;
            lastAddedLine.AddPoint(pos);
            DrawLine(lastAddedLine, true);
            lastAddedLine.RemoveLastPoint();
        }
    }





    public void DrawLine(CurvedLine line, bool updateRes)
    {
        Debug.Log("DrawLine");
        float startTime = Time.realtimeSinceStartup;
        line.draw(maxResCount);
        if (updateRes)
        {
            updateMaxRes(Time.realtimeSinceStartup - startTime > .05f, Time.realtimeSinceStartup - startTime);
        }
    }

    public void DrawLines()
    {
        Debug.Log("DrawAllLine");
        float startTime = Time.realtimeSinceStartup;
        foreach (CurvedLine cl in allLines)
        {
            lastAddedLine.draw(maxResCount);
        }
        updateMaxRes(Time.realtimeSinceStartup - startTime > .05f, Time.realtimeSinceStartup - startTime);
    }

    void updateMaxRes(bool over, float amount)
    {
        int multi = 1;
        if (over)
        {
            multi = -1;
        }
        else
        {
            maxResCount += 100;
            maxResCount = Mathf.Clamp(maxResCount, 1, 1000);
            return;
        }
        int changeby = 50;
        if (amount < .05f)
        {
            changeby = 50;
        }
        else if (amount < .1f)
        {
            changeby = 100;
        }
        else if (amount < .15f)
        {
            changeby = 150;
        }
        else if (amount < .2f)
        {
            changeby = 200;
        }
        else if (amount < .25f)
        {
            changeby = 250;
        }
        else
        {
            changeby = 500;
        }
        maxResCount += changeby * multi;
        maxResCount = Mathf.Clamp(maxResCount, 1, 1000);
    }





    public class CurvedLine
    {
        List<Vector2> Points = new List<Vector2>();
        GameObject CurvedLineLineRendererHolder;
        LineRenderer lineRenderer;

        public CurvedLine(GameObject CurvedLineLineRendererHolder)
        {
            this.CurvedLineLineRendererHolder = CurvedLineLineRendererHolder;
            GameObject go = new GameObject("LineRenderer");
            go.transform.parent = this.CurvedLineLineRendererHolder.transform;
            go.transform.position = new Vector3(0,0,-2);
            lineRenderer = go.AddComponent<LineRenderer>();
        }


        public void AddPoint(Vector2 newPoint)
        {
            Points.Add(newPoint);
        }

        public void RemoveLastPoint()
        {
            Points.RemoveAt(Points.Count - 1);
        }

        public int GetPointCount()
        {
            return Points.Count;
        }

        public void draw(int maxResolutionCount)
        {
            if (Points.Count <= 1)
            {
                return;
            }
            List<LerpData> lerps = new List<LerpData>();
            float maxDist = 0;
            for (int i = 0; i < Points.Count-1; i++)
            {
                lerps.Add(new LerpData(Points[i], Points[i+1]));
                maxDist += Vector2.Distance(Points[i], Points[i + 1]);
            }

            int lineResolution = Mathf.Clamp(maxResolutionCount, Points.Count, maxResolutionCount);

            while (lerps.Count > 1)
            {
                List<LerpData> tempLerps = new List<LerpData>();
                for (int i = 0; i < lerps.Count-1; i++)
                {
                    tempLerps.Add(new LerpData(lerps[i], lerps[i+1]));
                }
                lerps.Clear();
                lerps.AddRange(tempLerps);
            }
            lineRenderer.positionCount = 0;

            float lastDrawPoint = 0;
            List<Vector3> poses = new List<Vector3>();
            for (float i = 0; i <= 1f; i += 1f/ lineResolution)
            {
                Vector2 temp = lerps[0].getLerpPos(i);
                poses.Add(new Vector3(temp.x, temp.y, -2));
                lastDrawPoint = i;
            }
            if (lastDrawPoint != 1f)
            {
                Vector2 temp = lerps[0].getLerpPos(1);
                poses.Add(new Vector3(temp.x, temp.y, -2));
            }
            lineRenderer.positionCount = poses.Count;
            lineRenderer.SetPositions(poses.ToArray());
        }


        class LerpData
        {
            Vector2 startPos;
            Vector2 endPos;

            LerpData startPos2;
            LerpData endPos2;
            bool vec = true;
            public LerpData(Vector2 start, Vector2 end)
            {
                vec = true;
                startPos = start;
                endPos = end;
            }

            public LerpData(LerpData start, LerpData end)
            {
                vec = false;
                startPos2 = start;
                endPos2 = end;
            }

            public Vector2 getLerpPos(float time)
            {
                if (vec)
                {
                    return Vector2.Lerp(startPos, endPos, time);
                }
                else
                {
                    return Vector2.Lerp(startPos2.getLerpPos(time), endPos2.getLerpPos(time), time);
                }
            }
        }
    }
}
