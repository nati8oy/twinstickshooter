using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationController : MonoBehaviour
{
    private Vector2 aim;
    private PlayerControls playerControls;


    private void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

    }
    private void Update()
    {
        HandleRotation();
    }

    public void HandleRotation()
    {
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(aim);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            LookAt(point);
        }
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }
}


