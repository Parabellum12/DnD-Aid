using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreatorHandler_Script : MonoBehaviour
{
    [SerializeField] StraightLineHandler_Script StraightLineHandler;
    [SerializeField] General_2D_Camera_Handler_Script General_2D_CameraHandler;
    [SerializeField] MarkerHandler_Script MarkerHandler;
    [SerializeField] InputManager inputManager;

    private void Start()
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
            }
        });


        inputManager.AddKeyBinding(KeyCode.Alpha1, InputManager.KeyActionType.Down, "SelectToolOff", () =>
        {
            SelectedTool = Tools.Off;
        });
        inputManager.AddKeyBinding(KeyCode.Alpha2, InputManager.KeyActionType.Down, "SelectToolSelect", () =>
        {
            SelectedTool = Tools.Select;
        });
        inputManager.AddKeyBinding(KeyCode.Alpha3, InputManager.KeyActionType.Down, "SelectToolStraightLine", () =>
        {
            SelectedTool = Tools.StraightLine;
        });
        inputManager.AddKeyBinding(KeyCode.Alpha4, InputManager.KeyActionType.Down, "SelectToolCurveLine", () =>
        {
            SelectedTool = Tools.CurvedLine;
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
    }

    void handleClick()
    {
        switch (SelectedTool)
        {
            case Tools.Off:
                break;
            case Tools.Select:
                break;
            case Tools.StraightLine:
                StraightLineHandler.AddPoint(MarkerHandler.transform.position, true);
                break;
            case Tools.CurvedLine:
                break;
        }
    }
}
