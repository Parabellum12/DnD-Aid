using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class curvedLine_Handler_Script : MonoBehaviour
{
    List<Vector3> AllPositions = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (needToUpdateSpline && AllPositions.Count > 1)
        {
            updateSpline();
            needToUpdateSpline = false;
        }
    }
    bool needToUpdateSpline = false;


    public void addPos(Vector3 pos)
    {
        AllPositions.Add(pos);
        needToUpdateSpline = true;
    }


    public void updateSpline()
    {
        List<LerpData> TemplerpList = new List<LerpData>();
        Debug.Log("UpdateSpline: PosList:" + AllPositions.Count);
        for (int i = 0; i < AllPositions.Count; i++)
        {
            Debug.Log("Hello World:" + i);
            if (i >= AllPositions.Count - 1)
            {
                break;
            }
            TemplerpList.Add(new LerpData(AllPositions[i], AllPositions[i+1]));
        }

        Debug.Log("TemplerpList.Count:" + TemplerpList.Count);
        if (TemplerpList.Count > 1)
        {
            List<HigherLevelLerpData> finalList = new List<HigherLevelLerpData>();
            for (int i = 0; i < TemplerpList.Count; i++)
            {
                Debug.Log("Hello:" + i);
                if (i >= TemplerpList.Count - 1)
                {
                    break;
                }
                finalList.Add(new HigherLevelLerpData(TemplerpList[i], TemplerpList[i + 1]));
            }
            Debug.Log("finalList.Count:" + finalList.Count);
            while (finalList.Count > 1)
            {
                List<HigherLevelLerpData> tempFList = new List<HigherLevelLerpData>();
                for (int i = 0; i < finalList.Count; i++)
                {
                    Debug.Log("World:" + i);
                    if (i >= finalList.Count - 1)
                    {
                        break;
                    }
                    tempFList.Add(new HigherLevelLerpData(finalList[i], finalList[i + 1]));
                }
                Debug.Log("finalList.Count:" + finalList.Count);
                finalList.Clear();
                finalList.AddRange(tempFList);
            }
            RenderSplineLine(finalList[0]);
        }
        else
        {
            RenderSplineLine(TemplerpList[0]);
        }
    }


    [SerializeField] StraightLine_Handler_ScriptV2 V2;
    private void RenderSplineLine(HigherLevelLerpData finalSpline)
    {
        V2.clearPoints();
        List<Vector3> pointsToRender = new List<Vector3>();
        for (float i = 0; i < 1; i += 0.01f)
        {
            pointsToRender.Add(finalSpline.getLerpPoint(i));
        }

        //need to change
        for (int i = 0; i < pointsToRender.Count-1; i++)
        {
            V2.AddLine(pointsToRender[i], pointsToRender[i+1]);
        }
    }

    private void RenderSplineLine(LerpData finalSpline)
    {
        V2.clearPoints();
        List<Vector3> pointsToRender = new List<Vector3>();
        for (float i = 0; i < 1; i += 0.01f)
        {
            pointsToRender.Add(finalSpline.getLerpPoint(i));
        }

        //need to change
        for (int i = 0; i < pointsToRender.Count - 1; i++)
        {
            V2.AddLine(pointsToRender[i], pointsToRender[i + 1]);
        }
    }


    public class LerpData
    {
        Vector3 point1;
        Vector3 point2;

        public LerpData(Vector3 p1, Vector3 p2)
        {
            point1 = p1;
            point2 = p2;
        }

        public Vector3 getLerpPoint(float t)
        {
            return Vector3.Lerp(point1, point2, t);
        }
    }

    public class HigherLevelLerpData
    {
        LerpData lerp1;
        LerpData lerp2;
        //false = holds base lerp data, true = holds HigherLevelLerpData
        bool baseOrHigher;

        public HigherLevelLerpData(LerpData l1, LerpData l2)
        {
            lerp1 = l1;
            lerp2 = l2;
            baseOrHigher = false;
        }

        HigherLevelLerpData HLerp1;
        HigherLevelLerpData HLerp2;

        public HigherLevelLerpData(HigherLevelLerpData l1, HigherLevelLerpData l2)
        {
            HLerp1 = l1;
            HLerp2 = l2;
            baseOrHigher = true;
        }

        public Vector3 getLerpPoint(float t)
        {
            if (baseOrHigher)
            {
                return Vector3.Lerp(HLerp1.getLerpPoint(t), HLerp2.getLerpPoint(t), t);
            }
            else
            {
                return Vector3.Lerp(lerp1.getLerpPoint(t), lerp2.getLerpPoint(t), t);
            }
        }
    }


}
