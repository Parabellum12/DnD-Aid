using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCreatorHandler_Script : MonoBehaviour
{
    [SerializeField] StraightLineHandler_Script StraightLineHandler;
    [SerializeField] CurvedLineHandler_Script CurvedLineHandler;
    [SerializeField] General_2D_Camera_Handler_Script General_2D_CameraHandler;
    [SerializeField] MarkerHandler_Script MarkerHandler;
    [SerializeField] InputManager inputManager;
    [SerializeField] bool autoAddKeyBindings = true;

    [SerializeField] Canvas canvas;
    private void Start()
    {
        if (autoAddKeyBindings)
        {
            inputManager.AddKeyBinding(KeyCode.Mouse0, InputManager.KeyActionType.Down, "AddStraightLine", () =>
            {
                handleClick();
            });
            inputManager.AddKeyBinding(KeyCode.Mouse1, InputManager.KeyActionType.Up, "EndStraightLine", () =>
            {
                if (!General_2D_CameraHandler.CameraMousePaning)
                {
                    StraightLineHandler.EndCurrentLine();
                    CurvedLineHandler.EndLine();
                }
            });


            inputManager.AddKeyBinding(KeyCode.Alpha1, InputManager.KeyActionType.Down, "SelectToolOff", () =>
            {
                setTool(Tools.Off);
            });
            inputManager.AddKeyBinding(KeyCode.Alpha2, InputManager.KeyActionType.Down, "SelectToolSelect", () =>
            {
                setTool(Tools.Select);
            });
            inputManager.AddKeyBinding(KeyCode.Alpha3, InputManager.KeyActionType.Down, "SelectToolStraightLine", () =>
            {
                setTool(Tools.StraightLine);
            });
            inputManager.AddKeyBinding(KeyCode.Alpha4, InputManager.KeyActionType.Down, "SelectToolCurveLine", () =>
            {
                setTool(Tools.CurvedLine);
            });
        }


        UtilClass.FileSelector(canvas, Application.streamingAssetsPath, new string[] { ".txt" }, true, (selectedFiles) =>
        {

        });

    }

    public enum Tools
    {
        Off,
        Select,
        StraightLine,
        CurvedLine
    }
    public Tools SelectedTool = Tools.Off;

    private void Update()
    {
        StraightLineHandler.HandleGuideLine(MarkerHandler.transform.position);
        CurvedLineHandler.HandleGuideLine(MarkerHandler.transform.position);
    }

    void setTool(Tools newTool)
    {
        if (newTool.Equals(SelectedTool))
        {
            return;
        }

        //endlines
        StraightLineHandler.EndCurrentLine();
        CurvedLineHandler.EndLine();

        SelectedTool = newTool;
    }

    void handleClick()
    {
        if (!Application.isFocused || !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("mouseOverCanvas")))
        {
            return;
        }
        switch (SelectedTool)
        {
            case Tools.Off:
                break;
            case Tools.Select:
                float starTime = Time.realtimeSinceStartup; if (StraightLineHandler.HandleSelect(MarkerHandler.transform.position))
                {
                    Debug.Log("StraightLineHandler Select");
                }
                else if (CurvedLineHandler.HandleSelect(MarkerHandler.transform.position))
                {
                    Debug.Log("CurvedLineHandler Select");
                }
                else
                {
                    //Debug.Log("None Select");
                }
                Debug.Log("SelectLine Time:"+ (Time.realtimeSinceStartup - starTime));
                break;
            case Tools.StraightLine:
                StraightLineHandler.AddPoint(MarkerHandler.transform.position, true);
                break;
            case Tools.CurvedLine:
                CurvedLineHandler.AddPoint(MarkerHandler.transform.position);
                break;
        }
    }
}
