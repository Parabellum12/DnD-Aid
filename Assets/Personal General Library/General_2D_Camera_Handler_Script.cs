using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_2D_Camera_Handler_Script : MonoBehaviour
{
    /*
     * general code created for 2d cameras
     */
    public bool HorizontalMovement = true;
    public bool VerticalMovement = true;
    public bool ShiftForFasterMovement = true;

    public float baseMoveSpeed = 5f;
    public float shiftMoveSpeedMultiplier = 2f;

    public bool MousePan = true;
    public int MouseButtonToPanWith = 2;
    public bool AllowSecondaryMouseButtonPan = true;
    public int SecondaryMouseButonTOPanWith = 1;

    public bool WASDPan = true;
    public bool ArrowKeyPan = true;

    public bool CamZoom = true;
    public bool CamZoomDoesAccel = true;
    public float CamZoomAccelRateWaitTime = 3f;
    public float CamZoomAccelRate = 5f;
    public float TimeLimitBetweenScrollsForZoomAccel = .1f;
    float currentZoomChangeSpeed = 1;
    public float minCameraZoom = 50f;
    public float maxCameraZoom = 100f;


    public bool lockMoveIfOverUi = true;
    public bool lockZoomIfOverUi = true;
    public bool lockMovement = false;

    [SerializeField] float yPos = -10;

    [SerializeField] Camera cam;


    int horizontalMove;
    int verticalMove;

    float TimeSinceLastZoom;
    // Update is called once per frame
    void Update()
    {
        horizontalMove = 0;
        verticalMove = 0;
        if (WASDPan)
        {
            if (Input.GetKey(KeyCode.A))
            {
                horizontalMove--;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontalMove++;
            }

            if (Input.GetKey(KeyCode.W))
            {
                verticalMove++;
            }
            if (Input.GetKey(KeyCode.S))
            {
                verticalMove--;
            }
        }
        if (ArrowKeyPan)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalMove--;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalMove++;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                verticalMove++;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                verticalMove--;
            }
        }
        if (!HorizontalMovement)
        {
            horizontalMove = 0;
        }
        if (!VerticalMovement)
        {
            verticalMove = 0;
        }

        verticalMove = Mathf.Clamp(verticalMove, -1, 1);
        horizontalMove = Mathf.Clamp(horizontalMove, -1, 1);




        handleScroll();
        handleKeyMove();
        handleCameraPan();
    }


    void handleScroll()
    {
        if (lockMovement || !CamZoom || (lockZoomIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI"))))
        {
            return;
        }
        if (Time.realtimeSinceStartup - TimeSinceLastZoom < TimeLimitBetweenScrollsForZoomAccel)
        {
            currentZoomChangeSpeed += CamZoomAccelRate * Time.deltaTime;
        }
        else
        {
            currentZoomChangeSpeed = 1;
        }
        TimeSinceLastZoom = Time.realtimeSinceStartup;
        Vector2 mouseScrollDelta = Input.mouseScrollDelta;
        cam.orthographicSize -= mouseScrollDelta.y * currentZoomChangeSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minCameraZoom, maxCameraZoom);
    }


    void handleKeyMove()
    {
        if (lockMovement || lockMoveIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
        {
            return;
        }
        horizontalMove = Mathf.Clamp(horizontalMove, -1, 1);
        verticalMove = Mathf.Clamp(verticalMove, -1, 1);
        float moveSpeedMulti = 1;
        if (ShiftForFasterMovement && Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeedMulti = shiftMoveSpeedMultiplier;
        }
        cam.transform.position = new Vector3(cam.transform.position.x + (horizontalMove * (baseMoveSpeed * moveSpeedMulti) * Time.deltaTime),
                                                cam.transform.position.y + (verticalMove * (baseMoveSpeed * moveSpeedMulti) * Time.deltaTime),
                                                yPos);
    }

    Vector3 originalMousePos;
    void handleCameraPan()
    {
        if (lockMovement || !MousePan || (lockMoveIfOverUi && UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI"))))
        {
            return;
        }

        if (Input.GetMouseButtonDown(MouseButtonToPanWith) || Input.GetMouseButtonDown(SecondaryMouseButonTOPanWith))
        {
            originalMousePos = UtilClass.getMouseWorldPosition();
        }
        else if (Input.GetMouseButton(MouseButtonToPanWith) || Input.GetMouseButton(SecondaryMouseButonTOPanWith))
        {
            Vector2 newMousePos = UtilClass.getMouseWorldPosition();
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
