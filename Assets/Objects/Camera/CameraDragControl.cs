using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;


//Cinemachine FreeLook Camera Drag Control

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraDragControl : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float xSpeed = 300f;
    public float ySpeed = 2f;

    [Header("Mouse Settings")]
    [Tooltip("0=L, 1=R, 2=Mid)")]
    public int dragMouseButton = 1; 

    private CinemachineFreeLook freeLook;
    private ICameraInput cameraInput;

    void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        cameraInput = new MouseDragCameraInput(dragMouseButton); // Possible Input Interface
    }

    void Update()
    {
        if (cameraInput == null) return;

        if (cameraInput.IsActive())
        {
            Vector2 input = cameraInput.GetLookInput();
            freeLook.m_XAxis.Value += input.x * xSpeed * Time.deltaTime;
            freeLook.m_YAxis.Value -= input.y * ySpeed * Time.deltaTime;
        }
    }

    public void SetInputProvider(ICameraInput provider)
    {
        cameraInput = provider;
    }
}

public interface ICameraInput
{
    Vector2 GetLookInput();
    bool IsActive();
}

public class MouseDragCameraInput : ICameraInput
{
    private int mouseButton;

    public MouseDragCameraInput(int button = 1)
    {
        mouseButton = button;
    }

    public Vector2 GetLookInput()
    {
        if (!IsActive() || Mouse.current == null)
            return Vector2.zero;
        return Mouse.current.delta.ReadValue();
    }

    public bool IsActive()
    {
        if (Mouse.current == null)
            return false;

        switch (mouseButton)
        {
            case 0:
                return Mouse.current.leftButton.isPressed;
            case 1:
                return Mouse.current.rightButton.isPressed;
            case 2:
                return Mouse.current.middleButton.isPressed;
            default:
                return false;
        }
    }
}
