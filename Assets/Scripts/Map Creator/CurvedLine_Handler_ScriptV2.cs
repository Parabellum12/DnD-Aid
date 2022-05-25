using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class CurvedLine_Handler_ScriptV2 : MonoBehaviour
{
    /*
     * handles creating, editing, moving, saving and loading curved lines
     */
    List<CurvedLine> completeLines = new List<CurvedLine>();
    [SerializeField] int maxCurveLines = 3;
    [SerializeField] bool LowOrHighResLine = false;
    // Start is called before the first frame update
    void Start()
    {
        UtilClass.isPointWithinDistanceToLine(new Vector2(1,1), new Vector2(1, 5), new Vector2(0, 10), 1);
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

    public float3[][] getCurvedLinesAsSaveData()
    {
        float3[][] returner = new float3[completeLines.Count][];
        for (int i = 0; i < completeLines.Count; i++)
        {
            returner[i] = completeLines[i].getAsSaveData();
        }
        return returner;
    }

    public void setDataFromSaveData(float3[][] data)
    {
        deleteAnyHandles();
        endCurve();
        foreach (CurvedLine cl in completeLines)
        {
            cl.killMe();
        }
        completeLines.Clear();
        

        foreach (float3[] points in data)
        {
            foreach (float3 point in points)
            {
                addPoint(new Vector3(point.x, point.y, point.z));
            }
            endCurve();
        }
    }
    
    public bool HandleIfSelected(Vector2 Pos)
    {
        CurvedLine temp = null;
        foreach (CurvedLine cl in completeLines)
        {
            cl.removeAllHandles();
            if (cl.isCLickedOn(Pos))
            {
                temp = cl;
            }
        }
        if (temp != null)
        {
            temp.CreateHandles();
            return true;
        }
        return false;
    }

    public void deleteAnyHandles()
    {
        foreach (CurvedLine cl in completeLines)
        {
            cl.removeAllHandles();
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

    [SerializeField] GameObject handlePrefab;
    CurvedLine TempLine;
    public void addPoint(Vector3 point)
    {
        if (currentState == state.newCurve)
        {
            TempLine = new CurvedLine(LowOrHighResLine, handlePrefab);
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
        public List<Vector3> allPoints = new List<Vector3>();
        bool LowOrHighResLine;
        GameObject holder;
        LineRenderer lr;
        GameObject handlePreFab;


        public float3[] getAsSaveData()
        {
            float3[] returner = new float3[allPoints.Count];
            for (int i = 0; i < allPoints.Count; i++)
            {
                returner[i] = new float3(allPoints[i].x, allPoints[i].y, allPoints[i].z);
            }
            return returner;
        }


        public CurvedLine(bool LowOrHighResLine, GameObject handlePreFab)
        {
            this.LowOrHighResLine = LowOrHighResLine;
            holder = new GameObject("Curved Line Holder");
            lr = holder.AddComponent<LineRenderer>();
            this.handlePreFab = handlePreFab;
        }

        List<HandleMarker_Handler_Script> handleList = new List<HandleMarker_Handler_Script>();
        public void CreateHandles()
        {
            for (int i = 0; i < allPoints.Count; i++)
            {
               
                GameObject go = Instantiate(handlePreFab);
                HandleMarker_Handler_Script scr = go.GetComponent<HandleMarker_Handler_Script>();
                handleList.Add(scr);
                scr.setPos(allPoints[i]);
                scr.StartCoroutine(scr.returnMyPos(i, (vec, pos) =>
                {
                    Vector3[] temp = allPoints.ToArray();
                    temp[pos] = vec;
                    allPoints.Clear();
                    allPoints.AddRange(temp);
                    drawCurve();
                }));
                
            }
        }

        public void removeAllHandles()
        {
            foreach (HandleMarker_Handler_Script hr in handleList)
            {
                hr.KILLME();
            }
            handleList.Clear();
        }

        public bool isCLickedOn(Vector2 mousePos)
        {
            Vector3[] poses = curvePoints.ToArray();
            for (int i = 0; i < poses.Length; i++)
            {
                if (Vector2.Distance(mousePos, poses[i]) <= 1)
                {
                    Debug.Log("CurvedLine: I Was TOUCHED!!!");
                    return true;
                }
            }
            return false;
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
        List<Vector3> curvePoints = new List<Vector3>();

        public void drawCurve()
        {
            lr.positionCount = 0;
            curvePoints.Clear();
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
                Vector3 tempPoint = lerps[0].getLerpPos(t);
                pointsToDraw.Add(tempPoint);
                curvePoints.Add(tempPoint);
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
