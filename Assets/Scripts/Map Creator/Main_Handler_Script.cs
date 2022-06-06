using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Main_Handler_Script : MonoBehaviour
{
    /*
     * handles the editor code for adding changing, save and loading maps 
     */
    [SerializeField] LineRenderer BackGroundLR;
    [SerializeField] int modValue = 8;
    [SerializeField] Transform mainCam;
    [SerializeField] StraightLine_Handler_ScriptV2 straightLine_Handler;
    [SerializeField] CurvedLine_Handler_ScriptV2 splineLineHandler;
    [SerializeField] Transform GridSnapMarker;
    [SerializeField] General_2D_Camera_Handler_Script cameraMoveScr;

    // Start is called before the first frame update
    void Start()
    {
        quitToMainMenuButton.onClick.AddListener(() => { quitToMainMenu(); });
        foreach (selectButton_Handler scr in buttonHandlers)
        {
            scr.setup((tool) =>
            {
                setActiveTool(tool);
            });
        }
        buttonHandlers[1].select();
    }

    public enum tools
    {
        select,
        DrawStraightLine,
        move,
        curve,
    }

    public tools ActiveTool = tools.DrawStraightLine;
    bool Moveing = false;

    HandleMarker_Handler_Script selectedHandle = null;
    public bool lockInCurrentSelectedHandle = false;

    public void setSelectedHandle(HandleMarker_Handler_Script scr)
    {
        if (lockInCurrentSelectedHandle)
        {
            return;
        }
        selectedHandle = scr;
    }

    public HandleMarker_Handler_Script getSelectedHandle()
    {
        return selectedHandle;
    }
    // Update is called once per frame
    void Update()
    {
        //change selected tools 1:select 2:move 3:straight line 4:curve
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setActiveTool(tools.select);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setActiveTool(tools.DrawStraightLine);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            setActiveTool(tools.curve);
        }


        if (ActiveTool != tools.select)
        {
            straightLine_Handler.deleteAnyHandles();
            splineLineHandler.deleteAnyHandles();
        }

        if (Input.GetMouseButtonDown(0) && !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
        {
            //left click

            

            if (ActiveTool == tools.DrawStraightLine)
            {
                handleCreateLine();
            }
            else if (ActiveTool == tools.curve)
            {
                splineLineHandler.addPoint(new Vector3(GridSnapMarker.transform.position.x, GridSnapMarker.transform.position.y, -2));
            }
            else if (ActiveTool == tools.select && selectedHandle == null)
            {
                straightLine_Handler.deleteAnyHandles();
                splineLineHandler.deleteAnyHandles();
                if (splineLineHandler.HandleIfSelected(UtilClass.getMouseWorldPosition()))
                {

                }
                else if (straightLine_Handler.HandleIfSelected(UtilClass.getMouseWorldPosition()))
                {

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            splineLineHandler.deleteIfSelected();
            straightLine_Handler.deleteLine();
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (ActiveTool == tools.DrawStraightLine)
            {
                needToRemove = false;
                straightLine_Handler.endGuideLine();
                startNew = true;
                pos1 = new Vector2();
            }
            if (ActiveTool == tools.curve)
            {
                splineLineHandler.handleEndGuideLine();
                splineLineHandler.endCurve();
            }
        }


        if (Input.GetMouseButton(0))
        {
            if (ActiveTool == tools.move && Moveing)
            {
                straightLine_Handler.handleMove(new Vector2(GridSnapMarker.transform.position.x, GridSnapMarker.transform.position.y));
            }
            else if (ActiveTool == tools.select && selectedHandle != null)
            {
                Debug.Log("Want To Move");
                selectedHandle.setPos(GridSnapMarker.transform.position);
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (ActiveTool == tools.move && Moveing)
            {
                straightLine_Handler.handleMoveEnd();
                Moveing = false;
            }
        }


        splineLineHandler.handleGuideLine(new Vector3(GridSnapMarker.transform.position.x, GridSnapMarker.transform.position.y, -2));


        if (!startNew)
        {
            straightLine_Handler.drawGuideLine(new Vector3(pos1.x, pos1.y, -1), new Vector3(GridSnapMarker.transform.position.x, GridSnapMarker.transform.position.y, -1));
            needToRemove = true;
        }
        else if (needToRemove)
        {
            needToRemove = false;
            straightLine_Handler.endGuideLine();
        }

    }


    [SerializeField] List<selectButton_Handler> buttonHandlers = new List<selectButton_Handler>();

    void deselectAll()
    {
        foreach (selectButton_Handler scr in buttonHandlers)
        {
            scr.deSelect();
        }
    }

    public void setActiveTool(tools tool)
    {
        switch(tool)
        {
            case tools.select:
                deselectAll();
                ActiveTool = tool;
                buttonHandlers[0].select();
                break;
            case tools.DrawStraightLine:
                deselectAll();
                buttonHandlers[1].select();
                ActiveTool = tool;
                break;
            case tools.curve:
                deselectAll();
                buttonHandlers[2].select();
                ActiveTool = tool;
                break;
        }
    }

    //camera stuff
    int numOfControlHold = 0;
    public void handleCameraMove(bool moveAllowed)
    {
        if (moveAllowed)
        {
            numOfControlHold++;
        }
        else
        {
            numOfControlHold--;
        }

        if (numOfControlHold > 0)
        {
            cameraMoveScr.HorizontalMovement = false;
            cameraMoveScr.VerticalMovement = false;
        }
        else
        {
            cameraMoveScr.HorizontalMovement = true;
            cameraMoveScr.VerticalMovement = true;
        }
    }

    






    //background stuff
    Vector2 pos1 = new Vector2();
    bool startNew = true;
    bool needToRemove = false;

    private void handleCreateLine()
    {
        if (startNew)
        {
            //new line
            startNew = false;
            pos1 = new Vector2(GridSnapMarker.position.x, GridSnapMarker.position.y);
        }
        else
        {
            //continue line
            Vector2 pos2 = new Vector2(GridSnapMarker.position.x, GridSnapMarker.position.y);
            straightLine_Handler.AddLine(pos1, pos2);
            pos1 = pos2;
        }
    }

    List<Vector3> pointsList = new List<Vector3>();
    

    [SerializeField] Button quitToMainMenuButton;

    public void quitToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
