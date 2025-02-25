using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    CharacterController characterController;
    public Transform cameraTransform;

    public float WalkingSpeed = 7;
    public float gravity = 10;
    public float terminalSpeed = 20;
    public float jumpSpeed = 10;

    public float mouseSense = 0.2f;
    InputAction moveAction;
    InputAction lookAction;

    float verticalAngle;
    float horizontalAngle;
    float verticalSpeed;
    float groundedTimer;
    bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputActionAsset inputActions = GetComponent<PlayerInput>().actions;

        moveAction = inputActions.FindAction("Move");
        lookAction = inputActions.FindAction("Look");

        characterController = GetComponent<CharacterController>();

        horizontalAngle = transform.localEulerAngles.y;
        verticalAngle = 0;
    }

    void Update()
    {
        if(InGameUIManager.Instance.IsPopupUIOpen() == false)
        {
            Moving();
        }
    }

    #region MOVING
    void Moving()
    {
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y);

        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        move = move * WalkingSpeed * Time.deltaTime;
        move = transform.TransformDirection(move);
        characterController.Move(move);


        Vector2 look = lookAction.ReadValue<Vector2>();

        float turnPlayer = look.x * mouseSense;
        horizontalAngle += turnPlayer;
        if (horizontalAngle >= 360)
        {
            horizontalAngle -= 360;
        }
        if (horizontalAngle < 0)
        {
            horizontalAngle += 360;
        }

        Vector3 currentAngle = transform.localEulerAngles;
        currentAngle.y = horizontalAngle;
        transform.localEulerAngles = currentAngle;

        float turnCam = look.y * mouseSense;
        verticalAngle -= turnCam;
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f);
        currentAngle = cameraTransform.localEulerAngles;
        currentAngle.x = verticalAngle;
        cameraTransform.localEulerAngles = currentAngle;

        verticalSpeed -= gravity * Time.deltaTime;
        if (verticalSpeed < -terminalSpeed)
        {
            verticalSpeed = terminalSpeed;
        }
        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0);
        verticalMove *= Time.deltaTime;

        CollisionFlags flag = characterController.Move(verticalMove);
        if ((flag & CollisionFlags.Below) != 0)
        {
            verticalSpeed = 0;
        }

        if (characterController.isGrounded == false)
        {
            if (isGrounded == true)
            {
                groundedTimer += Time.deltaTime;
                if (groundedTimer > 0.2f)
                {
                    isGrounded = false;
                }
            }
        }
        else
        {
            isGrounded = true;
            groundedTimer = 0;
        }
    }

    void OnJump()
    {
        if (isGrounded == true)
        {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
        }
    }
    #endregion
}
