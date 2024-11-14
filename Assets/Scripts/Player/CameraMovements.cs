using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vc;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    private float xRotation;
    private float clampY = 60f;
    private bool isInMenu = false;

    public bool IsInMenu { get => isInMenu; set => isInMenu = value; }
    public float SensX { set => sensX = value; }
    public float SensY { set => sensY = value; }

    void Update()
    {
        if (!isInMenu)
        {
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        Vector2 mouse = PlayerInputManager.Instance.GetCameraMovementsValue();
        mouse.x *= sensX * Time.deltaTime;
        mouse.y *= sensY * Time.deltaTime;
        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -clampY, clampY);

        vc.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouse.x, 0);
    }
}