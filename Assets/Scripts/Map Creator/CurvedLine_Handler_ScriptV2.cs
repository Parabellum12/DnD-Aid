using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLine_Handler_ScriptV2 : MonoBehaviour
{
    List<CurvedLine> completeLines = new List<CurvedLine>();
    [SerializeField] int maxCurveLines = 3;
    [SerializeField] bool LowOrHighResLine = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void drawLines()
    {
        foreach (CurvedLine line in completeLines)
        {
            line.drawCurve();
        }
    }


    public void handleGuideLine(Vector3 mousePos)
    {
        if (currentState == state.continueCurve)
        {
            if (TempLine.getLastPoint() == mousePos)
            {
               // Debug.Log("Guide Point And Last Point Same");
                TempLine.drawCurve();
                return;
            }
            //Debug.Log("Guide Point And Last Point different");
            TempLine.addPoint(mousePos);
            TempLine.drawCurve();
            TempLine.removeLastPoint();
        }
    }

    public string getCurveLineSaveData()
    {
        string returner = "TCLD[";
        if (completeLines.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < completeLines.Count; i++)
        {
            returner += completeLines[i].getSaveDataAsString();
            if (i != completeLines.Count-1)
            {
                returner += "|";
            }
        }
        return returner;
    }

    public void loadFromString(string s)
    {
        string[] tagAndData = s.Split('[');
        if (tagAndData[0].Equals("TCLD"))
        {
            endCurve();
            foreach (CurvedLine cl in completeLines)
            {
                cl.killMe();
            }
            completeLines.Clear();
            string[] data = tagAndData[1].Split('|');
            foreach (string dataAndValue in data)
            {
                loadFromData(dataAndValue);
            }
        }
    }

    private void loadFromData(string data)
    {
        string[] tagAndValues = data.Split('}');
        if (tagAndValues[0].Equals("CLD"))
        {
            //valid data tag
            string[] values = tagAndValues[1].Split(':');
            foreach (string vecData in values)
            {
                string[] nums = vecData.Split(',');
                Vector3 vector3 = new Vector3(float.Parse(nums[0]), float.Parse(nums[1]), float.Parse(nums[2]));
                addPoint(vector3);
            }
            endCurve();


        }
    }

    public void handleEndGuideLine()
    {
        if (currentState == state.continueCurve)
        {
            TempLine.drawCurve();
        }
    }


    public enum state
    {
        newCurve,
        continueCurve
    }

    public state currentState = state.newCurve;


    CurvedLine TempLine;
    public void addPoint(Vector3 point)
    {
        if (currentState == state.newCurve)
        {
            TempLine = new CurvedLine(LowOrHighResLine);
            currentState = state.continueCurve;
        }
        TempLine.addPoint(point);
        TempLine.drawCurve();
        if (TempLine.maxPointsReached(maxCurveLines))
        {
            handleEndGuideLine();
            endCurve();
        }
    }

    public void endCurve()
    {
        if (currentState == state.continueCurve)
        {
            completeLines.Add(TempLine);
            currentState = state.newCurve;
        }
        TempLine = null;
    }

    

    public class CurvedLine
    {
        List<Vector3> allPoints = new List<Vector3>();

        bool LowOrHighResLine;
        GameObject holder;
        LineRenderer lr;
        public CurvedLine(bool LowOrHighResLine)
        {
            this.LowOrHighResLine = LowOrHighResLine;
            holder = new GameObject("Curved Line Holder");
            lr = holder.AddComponent<LineRenderer>();
        }

        public Vector3 getLastPoint()
        {
            return allPoints[allPoints.Count - 1];
        }

        public bool maxPointsReached(int max)
        {
            return (allPoints.Count == max);
        }

        public void addPoint(Vector3 newPoint)
        {
            allPoints.Add(newPoint);
        }
        public void removeLastPoint()
        {
            allPoints.RemoveAt(allPoints.Count - 1);
        }

        public void drawCurve()
        {
            lr.positionCount = 0;
            if (allPoints.Count <= 1)
            {
                return;
            }
            List<LerpData> lerps = new List<LerpData>();
           // Debug.Log("AllPos Count:" + allPoints.Count);
            for (int i = 0; i < allPoints.Count-1; i++)
            {
                lerps.Add(new LerpData(allPoints[i], allPoints[i + 1]));
            }
           // Debug.Log("lerps Count2:" + lerps.Count);

            while (lerps.Count > 1)
            {
                List<LerpData> temp = new List<LerpData>();
                for (int i = 0; i < lerps.Count-1; i++)
                {
                    //Debug.Log("ello:" + i + "::" + lerps.Count);
                    temp.Add(new LerpData(lerps[i], lerps[i+1]));
                }
                //Debug.Log("World:" + temp.Count + "::" + lerps.Count);
                lerps.Clear();
                lerps.AddRange(temp);
            }
            //Debug.Log("lerps3 Count:" + lerps.Count);
            List<Vector3> pointsToDraw = new List<Vector3>();

            float increaseAmount = 0.01f;
            if (LowOrHighResLine)
            {
                increaseAmount = 0.001f;
            }
            for (float t = 0f; t <= 1; t += increaseAmount)
            {
                pointsToDraw.Add(lerps[0].getLerpPos(t));
            }
            lr.positionCount = pointsToDraw.Count;
            lr.SetPositions(pointsToDraw.ToArray());
        }
    
        public string getSaveDataAsString()
        {
            string returner = "CLD}";
            for (int i = 0; i < allPoints.Count; i++)
            {
                Vector3 vec = allPoints[i];
                returner += Vector3ToSaveString(vec);
                if (i != allPoints.Count-1)
                {
                    returner += ":";
                }
            }
            return returner;
        }

        private string Vector3ToSaveString(Vector3 vec)
        {
            return vec.x + "," + vec.y + "," + vec.z;
        }

        public void killMe()
        {
            Destroy(holder);
        }
    }

    public class LerpData
    {
        Vector3 point1;
        Vector3 point2;

        LerpData lerp1;
        LerpData lerp2;
        //false = lerdata, true = vector3
        bool vecOrLerp;
        public LerpData(Vector3 p1, Vector3 p2)
        {
            point1 = p1;
            point2 = p2;
            vecOrLerp = true;
        }

        public LerpData(LerpData l1, LerpData l2)
        {
            lerp1 = l1;
            lerp2 = l2;
            vecOrLerp = false;
        }


        public Vector3 getLerpPos(float t)
        {
            if (vecOrLerp)
            {
                //vec
                return Vector3.Lerp(point1, point2, t);
            }
            else
            {
                return Vector3.Lerp(lerp1.getLerpPos(t), lerp2.getLerpPos(t), t);
            }
        }
    }
}
