using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreatorHandler_Script : MonoBehaviour
{
    [SerializeField] StraightLineHandler_Script StraightLineHandler;
    [SerializeField] InputManager inputManager;

    private void Start()
    {
        inputManager.AddKeyBinding(KeyCode.Mouse0, InputManager.KeyActionType.Down, "AddStraightLine", () =>
        {
            StraightLineHandler.AddPoint(UtilClass.getMouseWorldPosition(), true);
        });
        inputManager.AddKeyBinding(KeyCode.Mouse1, InputManager.KeyActionType.Down, "EndStraightLine", () =>
        {
            StraightLineHandler.EndCurrentLine();
        });
    }

    private void Update()
    {
        StraightLineHandler.HandleGuideLine(UtilClass.getMouseWorldPosition());
    }
}
