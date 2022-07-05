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

    Vector2 OriginPos = Vector2.zero;
    [SerializeField] float zPos = -2;
    [SerializeField] Material curvedLineMaterial;


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
            lastAddedLine = new CurvedLine(CurvedLineHolder, OriginPos, zPos);
            allLines.Add(lastAddedLine);
        }
        lastAddedLine.AddPoint(pos);
        lastPointAdded = pos;
        if (lastAddedLine.GetPointCount() == maxLineCount)
        {
            EndLine();
        }
        else if (lastAddedLine.GetPointCount() >= 2)
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
        DrawLine(lastAddedLine, 175);
        lastAddedLine = null;
    }






    public bool HandleSelect(Vector2 pos)
    {
        foreach (CurvedLine cl in allLines)
        {
            if (cl.IsSelected(pos, true))
            {
                //handle creating editing handles
                //Debug.Log("Curved HandleSelect");
                return true;
            }
        }
        /*
        foreach (CurvedLine cl in allLines)
        {
            if (cl.IsSelected(pos, false))
            {
                //handle creating editing handles
                //Debug.Log("Curved HandleSelect");
                return true;
            }
        }
        */
        return false;
    }



    public List<CurvedLine> GetSaveData()
    {
        return allLines;
    }

    public void LoadSaveData(List<CurvedLine> lines)
    {
        allLines = lines;
        DrawLines();
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
        if (line.GetPointCount() < 2)
        {
            line.KillMe();
            allLines.Remove(line);
        }
        else
        {
            line.draw(maxResCount, curvedLineMaterial);
        }
        if (updateRes)
        {
            updateMaxRes(Time.realtimeSinceStartup - startTime > minTestAmount, Time.realtimeSinceStartup - startTime);
        }
    }

    public void DrawLine(CurvedLine line, int resCount)
    {
        //Debug.Log("DrawLine");
        float startTime = Time.realtimeSinceStartup;
        if (line.GetPointCount() < 2)
        {
            line.KillMe();
            allLines.Remove(line);
        }
        else
        {
            line.draw(resCount, curvedLineMaterial);
        }
        updateMaxRes(Time.realtimeSinceStartup - startTime > minTestAmount, Time.realtimeSinceStartup - startTime);

    }

    public void DrawLines()
    {
        //Debug.Log("DrawAllLine");
        float startTime = Time.realtimeSinceStartup;
        List<CurvedLine> toRemove = new List<CurvedLine>();
        foreach (CurvedLine cl in allLines)
        {
            if (cl.GetPointCount() < 2)
            {
                toRemove.Add(cl);
            }
            else
            {
                cl.draw(maxResCount, curvedLineMaterial);
            }
        }
        updateMaxRes(Time.realtimeSinceStartup - startTime > minTestAmount, Time.realtimeSinceStartup - startTime);

        foreach (CurvedLine cl in toRemove)
        {
            cl.KillMe();
            allLines.Remove(cl);
        }
    }

    [SerializeField] float minTestAmount = .05f;
    [SerializeField] int minChangeBy = 5;
    void updateMaxRes(bool over, float amount)
    {
        int multi = 1;
        if (over)
        {
            multi = -1;
        }
        else
        {
            maxResCount += 2;
            maxResCount = Mathf.Clamp(maxResCount, 1, maxResCountHolder);
            Debug.Log("updateMaxRes Time: " + amount);
            return;
        }
        int changeby;
        if (amount < minTestAmount)
        {
            changeby = minChangeBy;
        }
        else if (amount < minTestAmount*2f)
        {
            changeby = minChangeBy*2;
        }
        else if (amount < minTestAmount*3)
        {
            changeby = minChangeBy*3;
        }
        else if (amount < minTestAmount*4)
        {
            changeby = minChangeBy*4;
        }
        else if (amount < minTestAmount*5)
        {
            changeby = minChangeBy*5;
        }
        else
        {
            changeby = minChangeBy*10;
        }
        maxResCount += changeby * multi;
        maxResCount = Mathf.Clamp(maxResCount, 1, maxResCountHolder);
        Debug.Log("updateMaxRes Time: " + amount);
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

        Vector2 originPoint;
        float zPos;
        public CurvedLine(GameObject CurvedLineLineRendererHolder, Vector2 originPoint, float zPos)
        {
            this.CurvedLineLineRendererHolder = CurvedLineLineRendererHolder;
            GameObject go = new GameObject("LineRenderer");
            go.transform.parent = this.CurvedLineLineRendererHolder.transform;
            go.transform.position = new Vector3(0, 0, zPos);
            lineRenderer = go.AddComponent<LineRenderer>();
            lerpData = null;
            this.originPoint = originPoint;
            this.zPos = zPos;
        }

        public bool IsSelected(Vector2 pos, bool firstORSecondCatch)
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

            if (firstORSecondCatch)
            {
                for (int i = 0; i < drawnPositions.Length - 1; i++)
                {
                    if (UtilClass.isPointWithinDistanceToLine(drawnPositions[i], drawnPositions[i + 1], pos, 1.5f))
                    {
                        Debug.Log("CurvedLine IsSelected First Catch");
                        return true;
                    }
                }
                return false;
            }
            else
            {
                for (float i = 0; i <= 1; i += 1f / 250)
                {
                    Vector2 a = lerpData.getLerpPos(i);
                    if (Vector2.Distance(a, pos) <= 1.5f)
                    {
                        Debug.Log("CurvedLine IsSelected Second Catch");
                        return true;
                    }
                }
                return false;
            }
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

        public void draw(int maxResolutionCount, Material curvedLineMaterial)
        {
            if (Points.Count <= 1)
            {
                return;
            }
            int lineResolution = Mathf.Clamp(maxResolutionCount, Mathf.RoundToInt(Points.Count*1.5f), maxResolutionCount);
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
                poses.Add(new Vector3(temp.x, temp.y, zPos));

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
                Vector3 tempVec3 = new Vector3(temp.x + originPoint.x, temp.y + originPoint.y, zPos);
                poses.Add(tempVec3);
            }
            maxDistFromCurveCenter = maxDistFromCenter;
            drawnPositions = poses.ToArray();
            lineRenderer.positionCount = poses.Count;
            lineRenderer.SetPositions(poses.ToArray());
            drawnPositions = poses.ToArray();
            lineRenderer.material = curvedLineMaterial;
        }

        public void KillMe()
        {
            Debug.LogError("Kill me");
            Destroy(lineRenderer.gameObject);
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
