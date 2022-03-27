using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_2D_Camera_Handler_Script : MonoBehaviour
{
    [SerializeField] bool HorizontalMovement = true;
    [SerializeField] bool VerticalMovement = true;
    [SerializeField] bool ShiftForFasterMovement = true;

    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float shiftMoveSpeedMultiplier = 2f;

    [SerializeField] bool MousePan = true;
    [SerializeField] int MouseButtonToPanWith = 2;

    [SerializeField] bool WASDPan = true;
    [SerializeField] bool ArrowKeyPan = true;

    [SerializeField] bool CamZoom = true;
    [SerializeField] float minCameraZoom = 50f;
    [SerializeField] float maxCameraZoom = 100f;

    [SerializeField] float yPos = -10;

    [SerializeField] Camera cam;


    int horizontalMove;
    int verticalMove;

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








        handleKeyMove();
        handleCameraPan();
    }


    void handleKeyMove()
    {
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
        if (!MousePan)
        {
            return;
        }

        if (Input.GetMouseButtonDown(MouseButtonToPanWith))
        {
            originalMousePos = UtilClass.getMouseWorldPosition();
        }
        else if (Input.GetMouseButton(MouseButtonToPanWith))
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
