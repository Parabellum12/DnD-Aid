using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLineHandler_Script : MonoBehaviour
{
    List<CurvedLine> allLines = new List<CurvedLine>();
    CurvedLine lastAddedLine;
    GameObject CurvedLineHolder;
    public int maxLineCount = 9;
    [SerializeField] int maxResCountHolder = 1000;
    [SerializeField]int maxResCount;

    private void Start()
    {
        maxResCount = maxResCountHolder;
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
            maxResCount = maxResCountHolder;
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
        lastAddedLine.draw(175);
        lastAddedLine = null;
    }






    public bool HandleSelect(Vector2 pos)
    {
        foreach (CurvedLine cl in allLines)
        {
            if (cl.IsSelected(pos))
            {
                //handle creating editing handles
                //Debug.Log("Curved HandleSelect");
                return true;
            }
        }
        return false;
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
        //Debug.Log("DrawLine");
        float startTime = Time.realtimeSinceStartup;
        line.draw(maxResCount);
        if (updateRes)
        {
            updateMaxRes(Time.realtimeSinceStartup - startTime > .05f, Time.realtimeSinceStartup - startTime);
        }
    }

    public void DrawLines()
    {
        //Debug.Log("DrawAllLine");
        float startTime = Time.realtimeSinceStartup;
        foreach (CurvedLine cl in allLines)
        {
            cl.draw(maxResCount);
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
            maxResCount += 10;
            maxResCount = Mathf.Clamp(maxResCount, 1, maxResCountHolder);
            return;
        }
        int changeby = 50;
        if (amount < .05f)
        {
            changeby = 25;
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
        maxResCount = Mathf.Clamp(maxResCount, 1, maxResCountHolder);
    }





    public class CurvedLine
    {
        List<Vector2> Points = new List<Vector2>();
        GameObject CurvedLineLineRendererHolder;
        LineRenderer lineRenderer;
        LerpData lerpData = null;
        Vector3[] drawnPositions = null;
        Vector2 centerOfCurve;
        float maxDistFromCurveCenter;
        public CurvedLine(GameObject CurvedLineLineRendererHolder)
        {
            this.CurvedLineLineRendererHolder = CurvedLineLineRendererHolder;
            GameObject go = new GameObject("LineRenderer");
            go.transform.parent = this.CurvedLineLineRendererHolder.transform;
            go.transform.position = new Vector3(0,0,-2);
            lineRenderer = go.AddComponent<LineRenderer>();
            lerpData = null;
        }

        public bool IsSelected(Vector2 pos)
        {
            if (lerpData == null)
            {
                //Debug.Log("CurvedLine IsSelected First Catch");
                return false;
            }
            if (Vector2.Distance(centerOfCurve, pos) > maxDistFromCurveCenter)
            {
                
                return false;
            }
            Debug.Log("Proccess Line");


            for (int i = 0; i < drawnPositions.Length-1; i++)
            {
                if (UtilClass.isPointWithinDistanceToLine(drawnPositions[i], drawnPositions[i+1], pos, 1.25f))
                {
                    Debug.Log("CurvedLine IsSelected First Catch");
                    return true;
                }
            }


            for (float i = 0; i <= 1; i += 1f/250)
            {
                Vector2 a = lerpData.getLerpPos(i);
                if (Vector2.Distance(a ,pos) <= 1.25f)
                {
                    Debug.Log("CurvedLine IsSelected Second Catch");
                    return true;
                }
            }
            return false;
        }


        public void AddPoint(Vector2 newPoint)
        {
            Points.Add(newPoint);
            lerpData = null;
        }

        public void RemoveLastPoint()
        {
            Points.RemoveAt(Points.Count - 1);
            lerpData = null;
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
            int lineResolution = Mathf.Clamp(maxResolutionCount, Points.Count*2, maxResolutionCount);
            if (lerpData == null)
            {

                centerOfCurve = Vector2.zero;
                List<LerpData> lerps = new List<LerpData>();
                float maxDist = 0;
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    lerps.Add(new LerpData(Points[i], Points[i + 1]));
                    maxDist += Vector2.Distance(Points[i], Points[i + 1]);
                    centerOfCurve += Points[i];
                }
                centerOfCurve = centerOfCurve / Points.Count;







                while (lerps.Count > 1)
                {
                    List<LerpData> tempLerps = new List<LerpData>();
                    for (int i = 0; i < lerps.Count - 1; i++)
                    {
                        tempLerps.Add(new LerpData(lerps[i], lerps[i + 1]));
                    }
                    lerps.Clear();
                    lerps.AddRange(tempLerps);
                }
                lineRenderer.positionCount = 0;
                lerpData = lerps[0];
            }
            float lastDrawPoint = 0;
            List<Vector3> poses = new List<Vector3>();



            float maxDistFromCenter = 0;

            for (float i = 0; i <= 1f; i += 1f/ lineResolution)
            {
                Vector2 temp = lerpData.getLerpPos(i);
                poses.Add(new Vector3(temp.x, temp.y, -2));

                if (Vector2.Distance(centerOfCurve, temp) > maxDistFromCenter)
                {
                    maxDistFromCenter = Vector2.Distance(centerOfCurve, temp);
                }

                lastDrawPoint = i;
            }
            if (lastDrawPoint != 1f)
            {
                Vector2 temp = lerpData.getLerpPos(1);
                if (Vector2.Distance(centerOfCurve, temp) > maxDistFromCenter)
                {
                    maxDistFromCenter = Vector2.Distance(centerOfCurve, temp);
                }
                poses.Add(new Vector3(temp.x, temp.y, -2));
            }
            maxDistFromCurveCenter = maxDistFromCenter;
            drawnPositions = poses.ToArray();
            lineRenderer.positionCount = poses.Count;
            lineRenderer.SetPositions(poses.ToArray());
            drawnPositions = poses.ToArray();
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
