using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class General_2D_Camera_Handler_Script : MonoBehaviour
{
    /*
     * general code created for 2d cameras
     * 
     * REQUIRES THE INPUT MANAGER TO FUNCTION
     */
    public bool HorizontalMovement = true;
    public bool VerticalMovement = true;
    public bool ShiftForFasterMovement = true;

    public float baseMoveSpeed = 50f;
    public float shiftMoveSpeedMultiplier = 2.5f;

    public bool MousePan = true;
    public int MouseButtonToPanWith = 2;
    public bool AllowSecondaryMouseButtonPan = true;
    public int SecondaryMouseButonTOPanWith = 1;

    public bool WASDPan = true;
    public bool ArrowKeyPan = true;

    public bool CamZoom = true;
    public bool CamZoomViaButtons = true;
    public bool CamZoomDoesAccel = true;
    public float CamZoomAccelRateWaitTime = 1f;
    public float CamZoomAccelRate = 10f;
    public float maxZoomRate = 25;
    public float TimeLimitBetweenScrollsForZoomAccel = .5f;
    public float minCameraZoom = 50f;
    public float maxCameraZoom = 500f;


    public bool lockMoveIfOverUi = true;
    public bool lockZoomIfOverUi = true;
    public bool lockMovement = false;

    [SerializeField] float yPos = -10;

    [SerializeField] Camera cam;

    [SerializeField] InputManager InputManager;
    int horizontalMove = 0;
    int verticalMove = 0;
    bool moveSpeedUpActive = false;

    private void Start()
    {



        InputManager.AddKeyBinding(KeyCode.A, false, InputManager.KeyActionType.Down, "KeyCameraMoveLeft", () =>
        {
            horizontalMove--;
            //Debug.Log("Pressed A");
        });
        InputManager.AddKeyBinding(KeyCode.D, false, InputManager.KeyActionType.Down, "KeyCameraMoveRight", () =>
        {
            horizontalMove++;
            //Debug.Log("Pressed D");
        });
        InputManager.AddKeyBinding(KeyCode.W, false, InputManager.KeyActionType.Down, "KeyCameraMoveUp", () =>
        {
            verticalMove++;
            //Debug.Log("Pressed W");
        });
        InputManager.AddKeyBinding(KeyCode.S, false, InputManager.KeyActionType.Down, "KeyCameraMoveDown", () =>
        {
            verticalMove--;
            //Debug.Log("Pressed S");
        });


        InputManager.AddKeyBinding(KeyCode.A, false, InputManager.KeyActionType.Up, "KeyCameraMoveLeftEND", () =>
        {
            horizontalMove++;
            //Debug.Log("Up A");
        });
        InputManager.AddKeyBinding(KeyCode.D, false, InputManager.KeyActionType.Up, "KeyCameraMoveRightEND", () =>
        {
            horizontalMove--;
            //Debug.Log("Up D");
        });
        InputManager.AddKeyBinding(KeyCode.W, false, InputManager.KeyActionType.Up, "KeyCameraMoveUpEND", () =>
        {
            verticalMove--;
            //Debug.Log("Up W");
        });
        InputManager.AddKeyBinding(KeyCode.S, false, InputManager.KeyActionType.Up, "KeyCameraMoveDownEND", () =>
        {
            verticalMove++;
            // Debug.Log("Up S");
        });


        InputManager.AddKeyBinding(KeyCode.LeftShift, false, InputManager.KeyActionType.Down, "KeyCameraMoveSpeedUpStart", () =>
        {
            moveSpeedUpActive = true;
        });
        InputManager.AddKeyBinding(KeyCode.LeftShift, false, InputManager.KeyActionType.Up, "KeyCameraMoveSpeedUpEnd", () =>
        {
            moveSpeedUpActive = false;
        });
    }
    // Update is called once per frame
    void Update()
    {

        if (!Application.isFocused)
        {
            horizontalMove = 0;
            verticalMove = 0;
            moveSpeedUpActive = false;
        }

        handleScroll();
        handleKeyMove();
        handleCameraPan();
    }

    public bool CameraMousePaning = false;


    bool buttonsHeldForZoomIn()
    {
        return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Equals);
    }
    bool buttonsHeldForZoomOut()
    {
        return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Minus);
    }


    float currentZoomChangeSpeed = 1;
    bool zooming = false;
    bool lockZoom = false;
    float TimeLastZoom = 0;
    float timeStartZoom = 0;
    void handleScroll()
    {
        if (lockMovement || !CamZoom || (lockZoomIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI"))) || !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("mouseOverCanvas")))
        {
            return;
        }


        if (Input.mouseScrollDelta.y != 0 || buttonsHeldForZoomIn() || buttonsHeldForZoomOut())
        {
            lockZoom = false;
            TimeLastZoom = Time.realtimeSinceStartup;
        }

        if ((Time.realtimeSinceStartup - TimeLastZoom < TimeLimitBetweenScrollsForZoomAccel || buttonsHeldForZoomIn() || buttonsHeldForZoomOut()) && !lockZoom)
        {
            if (!zooming)
            {
                timeStartZoom = Time.realtimeSinceStartup;
                zooming = true;
            }
            if (Time.realtimeSinceStartup - timeStartZoom > CamZoomAccelRateWaitTime && zooming && Input.mouseScrollDelta.y != 0)
            {
                //start increasing zoom speed
                currentZoomChangeSpeed += CamZoomAccelRate * Time.deltaTime;
            }
        }
        else
        {
            timeStartZoom = 0;
            TimeLastZoom = 0;
            zooming = false;
            currentZoomChangeSpeed = 1;
            lockZoom = true;
        }


        currentZoomChangeSpeed = Mathf.Clamp(currentZoomChangeSpeed, 0, maxZoomRate);

        Vector2 mouseScrollDelta = Input.mouseScrollDelta;
        if (buttonsHeldForZoomIn())
        {
            mouseScrollDelta.y = 1;
        }
        if (buttonsHeldForZoomOut())
        {
            mouseScrollDelta.y -= 1;
        }
        cam.orthographicSize -= mouseScrollDelta.y * currentZoomChangeSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minCameraZoom, maxCameraZoom);
    }


    void handleKeyMove()
    {
       //Debug.Log("handleKeyMove");
        if (lockMovement || lockMoveIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")) || !Application.isFocused)
        {
            return;
        }
        horizontalMove = Mathf.Clamp(horizontalMove, -1, 1);
        verticalMove = Mathf.Clamp(verticalMove, -1, 1);
        float moveSpeedMulti = 1;
        if (ShiftForFasterMovement && moveSpeedUpActive)
        {
            moveSpeedMulti = shiftMoveSpeedMultiplier;
        }
        cam.transform.position = new Vector3(cam.transform.position.x + (horizontalMove * (baseMoveSpeed * moveSpeedMulti) * Time.deltaTime),
                                                cam.transform.position.y + (verticalMove * (baseMoveSpeed * moveSpeedMulti) * Time.deltaTime),
                                                yPos);
    }

    Vector3 originalMousePos;
    bool waitingForRelease = false;
    void handleCameraPan()
    {
        if ((lockMovement || !MousePan || (lockMoveIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI"))) || !Application.isFocused) && !CameraMousePaning)
        {
            CameraMousePaning = false;
            waitingForRelease = false;
            return;
        }

        if (Input.GetMouseButtonDown(MouseButtonToPanWith) || Input.GetMouseButtonDown(SecondaryMouseButonTOPanWith))
        {
            originalMousePos = UtilClass.getMouseWorldPosition();
        }



        if (!waitingForRelease)
        {
            CameraMousePaning = false;
        }


        if (Input.GetMouseButtonUp(MouseButtonToPanWith) || Input.GetMouseButtonUp(SecondaryMouseButonTOPanWith))
        {
            waitingForRelease = false;
        }
        else if (Input.GetMouseButton(MouseButtonToPanWith) || Input.GetMouseButton(SecondaryMouseButonTOPanWith))
        {
            Vector2 newMousePos = UtilClass.getMouseWorldPosition();
            if (!originalMousePos.Equals(newMousePos))
            {
                waitingForRelease = true;
                CameraMousePaning = true;
            }
            float horizotalDist = newMousePos.x - originalMousePos.x;
            float verticalDist = newMousePos.y - originalMousePos.y;

            Vector3 newPos = new Vector3(cam.transform.position.x - horizotalDist,
                                            cam.transform.position.y - verticalDist,
                                            yPos);
            cam.transform.position = newPos;


            originalMousePos = UtilClass.getMouseWorldPosition();
        }
    }
}
