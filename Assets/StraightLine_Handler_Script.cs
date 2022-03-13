using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLine_Handler_Script : MonoBehaviour
{

    List<StraightLineShape> straightLineShapes = new List<StraightLineShape>();
    [SerializeField] Transform snapMarker;
    static Transform snapMarkerTransform;
    public handlerState state;


    StraightLineShape mainShape;




    // Start is called before the first frame update
    void Start()
    {
        state = handlerState.New;
        snapMarkerTransform = snapMarker;
        mainShape = new StraightLineShape(snapMarkerTransform);
    }
    bool needToUpdateRenderers = false;
    public StraightLinePoint pointToMove;
    // Update is called once per frame
    void Update()
    {
        if (state == handlerState.currentlyDrawing)
        {
            StraightLinePoint point = new StraightLinePoint();
            point.x = snapMarkerTransform.position.x;
            point.y = snapMarkerTransform.position.y;
            CurrentSelectedPoint.nextPoints.Add(point);
            newShape.shapePoints.Add(point);
            newShape.UpdateRenderer();
            newShape.shapePoints.RemoveAt(newShape.shapePoints.Count-1);
            CurrentSelectedPoint.nextPoints.Remove(point);
        }
        if (needToUpdateRenderers)
        {
            updateAllShapes();
        }
    }

    public void handleMove()
    {
        //Debug.Log("handleMove");
        if (pointToMove != null)
        {
            pointToMove.x = snapMarkerTransform.position.x;
            pointToMove.y = snapMarkerTransform.position.y;
            needToUpdateRenderers = true;
        }
    }


    private void updateAllShapes()
    {
        foreach (StraightLineShape sr in straightLineShapes)
        {
            sr.UpdateRenderer();
        }
        needToUpdateRenderers = false;
    }

    public enum handlerState
    {
        currentlyDrawing,
        New,
    }


    public void handleLeftClick()
    {
        if (state == handlerState.currentlyDrawing)
        {
            handleContinueDrawing();
        }
        else if (state == handlerState.New)
        {
            handleNewShapeClick();
            state = handlerState.currentlyDrawing;
        }
    }

    public void handleRightClick()
    {
        CurrentSelectedPoint = null;
        straightLineShapes.Add(newShape);
        state = handlerState.New;
        needToUpdateRenderers = true;
    }


    StraightLinePoint CurrentSelectedPoint;
    private void handleContinueDrawing()
    {

        StraightLinePoint clickedPoint = getClickedPoint();
        if (clickedPoint == null)
        {
            //not clicked on point as starter
            StraightLinePoint newPoint = new StraightLinePoint();
            newPoint.x = snapMarker.position.x;
            newPoint.y = snapMarker.position.y;
            Debug.Log("new Point: continue");
            CurrentSelectedPoint.nextPoints.Add(newPoint);
            newShape.shapePoints.Add(newPoint);
            CurrentSelectedPoint = newPoint;
        }
        else
        {
            //newShape.shapePoints.Add(clickedPoint);


            StraightLinePoint newPoint = new StraightLinePoint();
            newPoint.x = snapMarker.position.x;
            newPoint.y = snapMarker.position.y;
            Debug.Log("existing Point: continue");
            CurrentSelectedPoint.nextPoints.Add(clickedPoint);
            CurrentSelectedPoint = clickedPoint;
            //newShape.shapePoints.Add(newPoint);

        }
    }





    StraightLineShape newShape;
    private void handleNewShapeClick()
    {
        newShape = mainShape;
        StraightLinePoint clickedPoint = getClickedPoint();
        if (clickedPoint == null)
        {
            //not clicked on point as starter
            StraightLinePoint newPoint = new StraightLinePoint();
            newPoint.x = snapMarker.position.x;
            newPoint.y = snapMarker.position.y;
            Debug.Log("new Point");
            newShape.shapePoints.Add(newPoint);
            CurrentSelectedPoint = newPoint;
        }
        else
        {
            //newShape.shapePoints.Add(clickedPoint);
            CurrentSelectedPoint = clickedPoint;
            /*
            StraightLinePoint newPoint = new StraightLinePoint();
            newPoint.x = snapMarker.position.x;
            newPoint.y = snapMarker.position.y;
            Debug.Log("Existing Point");
            CurrentSelectedPoint.nextPoints.Add(newPoint);
            newShape.shapePoints.Add(newPoint);
            CurrentSelectedPoint = newPoint;
            */

        }





    }

    public StraightLinePoint getClickedPoint()
    {
        StraightLinePoint temp;
        foreach (StraightLineShape shape in straightLineShapes)
        {
            temp = shape.returnClickedPoint();
            if (temp != null)
            {
                return temp;
            }
        }
        if (newShape != null)
        {
            temp = newShape.returnClickedPoint();
            if (temp != null)
            {
                return temp;
            }
        }
        return null;
    }


    public class StraightLineShape
    {
        public List<StraightLinePoint> shapePoints = new List<StraightLinePoint>();
        LineRenderer[] shapeRenderers = new LineRenderer[0];
        GameObject[] lineShowers = new GameObject[0];
        Transform snapMarker;
        GameObject shower;
        public StraightLineShape(Transform t)
        {
            Debug.Log("Make New LineShape");
            snapMarker = t;
            shower = new GameObject("LineRendererObject");
        }

        public void UpdateRenderer()
        {
            foreach (GameObject lr in lineShowers)
            {
                Destroy(lr);
            }
            

            Vector4[] lines = getLines();
            shapeRenderers = new LineRenderer[lines.Length];
            lineShowers = new GameObject[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            { 
                lineShowers[i] = new GameObject("Line:"+i);
                lineShowers[i].transform.parent = shower.transform;
                lineShowers[i].AddComponent<LineRenderer>();
                shapeRenderers[i] = lineShowers[i].GetComponent<LineRenderer>();
            }

            for (int i = 0; i < shapeRenderers.Length; i++)
            {
                shapeRenderers[i].positionCount = 2;
                Vector3[] arr = {new Vector3(lines[i].x, lines[i].y, -1), new Vector3(lines[i].z, lines[i].w, -1) };
                shapeRenderers[i].SetPositions(arr);
                shapeRenderers[i].startWidth = 1;
                shapeRenderers[i].endWidth = 1;
                shapeRenderers[i].useWorldSpace = true;
            }
        }

        private Vector4[] getLines()
        {
            List<Vector4> returner = new List<Vector4>();
            foreach (StraightLinePoint point in shapePoints)
            {
                point.lineAlreadyDrawn = false;
            }

            foreach (StraightLinePoint point in shapePoints)
            {
                if (!point.lineAlreadyDrawn)
                {
                    returner.AddRange(point.getLines());
                }
            }


            return returner.ToArray();
        }

        public StraightLinePoint returnClickedPoint()
        {
            foreach (StraightLinePoint point in shapePoints)
            {
                if (snapMarker.position.x == point.x && snapMarker.position.y == point.y)
                {
                    return point;
                }
            }
            return null;
        }
    }


    public class StraightLinePoint
    {
        public float x, y;
        public List<StraightLinePoint> nextPoints = new List<StraightLinePoint>();
        public bool lineAlreadyDrawn = false;

        public List<Vector4> getLines()
        {
            lineAlreadyDrawn = true;
            List<Vector4> returner = new List<Vector4>();
            foreach (StraightLinePoint poi in nextPoints)
            {
                returner.Add(new Vector4(x,y, poi.x, poi.y));
            }
            return returner;
        }
    }
}
