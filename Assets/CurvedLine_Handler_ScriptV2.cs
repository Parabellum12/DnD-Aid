using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLine_Handler_ScriptV2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum state
    {
        newCurve,
        continueCurve
    }

    public class CurvedLine
    {
        List<Vector3> allPoints = new List<Vector3>();


        GameObject holder;
        LineRenderer lr;
        public CurvedLine()
        {
            holder = new GameObject("Curved Line Holder");
            lr = holder.AddComponent<LineRenderer>();
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
