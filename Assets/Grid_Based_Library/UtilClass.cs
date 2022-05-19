using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class UtilClass 
{



    //create world text
    public static TextMeshPro createWorldText(string text, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 40, Color color = default(Color), TMPro.TextContainerAnchors textAnchor = TMPro.TextContainerAnchors.Middle, TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Center, int sorintOrder = 0)
    {
        if (color == null)
        {
            color = Color.white;
        }
        return createWorldText2(parent, text, localPos, fontSize, color, textAnchor, textAlignment, sorintOrder);
    }

    public static TextMeshPro createWorldText2(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TMPro.TextContainerAnchors textAnchor, TMPro.TextAlignmentOptions textAlignment, int sorintOrder)
    {
        GameObject gameObject = new GameObject("WorldText", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
        TextMeshPro textmesh = gameObject.GetComponent<TextMeshPro>();
        textmesh.enableAutoSizing = true;
        textmesh.fontSizeMin = 5;
        textmesh.alignment = textAlignment;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
        textmesh.text = text;
        textmesh.fontSize = fontSize;
        textmesh.color = color;
        textmesh.GetComponent<MeshRenderer>().sortingOrder = sorintOrder;
        return textmesh;
    }






    //get mouse world position
    public static Vector3 getMouseWorldPosition()
    {
        Vector3 vec = getMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 getMouseWorldPositionWithZ()
    {
        return getMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 getMouseWorldPositionWithZ(Camera worldCam)
    {
        return getMouseWorldPositionWithZ(Input.mousePosition, worldCam);
    }

    public static Vector3 getMouseWorldPositionWithZ(Vector3 screenPos, Camera worldCam)
    {
        Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    static int UILayer;



    //ui stuff
    public static bool IsPointerOverUIElement(int UILayer)
    {
        UtilClass.UILayer = UILayer;
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public static float GetClosestDistanceBetweenLineAndPoint2D(Vector2 a, Vector2 b, Vector2 c)
    {
        float dist = Mathf.Abs(((c.x - a.x) * (-b.y + a.y)) + ((c.y - a.y) * (b.x - a.x))) / Mathf.Sqrt(((-b.y+a.y) * (-b.y + a.y)) + ((b.x-a.x)) * (b.x - a.x));
        return dist;
    }

    public static bool isPointWithinDistanceToLine(Vector2 a, Vector2 b, Vector2 c, float dist)
    {
        //point c in relation to line a-b
        return GetClosestDistanceBetweenLineAndPoint2D(a,b,c) <= dist;
    }



    //covert object <-> byte[]
    public static byte[] ObjectToByteArray<TObject>(TObject obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public static TObject ByteArrayToObject<TObject>(byte[] arrBytes)
    {
        if (arrBytes == null)
        {
            return default(TObject);
        }
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        TObject obj = (TObject)binForm.Deserialize(memStream);

        return obj;
    }



    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}

