using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraPan : MonoBehaviour
{
    [SerializeField] private float panSpeed = 2f;
    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera virtualCamera;

    public Transform cameraTransform;

    private void Awake()
    {
        inputProvider = GetComponent<CinemachineInputProvider>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        //cameraTransform = virtualCamera.VirtualCameraGameObject.transform;
    }
    // Update is called once per frame
    void Update()
    {
        float x = inputProvider.GetAxisValue(0);
        float y = inputProvider.GetAxisValue(1);
        float z = inputProvider.GetAxisValue(2);

        if (x != 0 || z != 0)
        {
            PanScreen(x, z);
        }
    }

    public Vector3 PanDirection(float x, float z)
    {

        Vector3 direction = Vector3.zero;
        if(z>= Screen.height*0.95f)
        {
            direction.z += 1;
        }

       else if (z <= Screen.height * 0.05f)
        {
            direction.z  -=1;
        }


        if (x >= Screen.width * 0.95f)
        {
            direction.x += 1;
        }

        else if (x <= Screen.width * 0.05f)
        {
            direction.x -= 1;
        }
        return direction;
    }


    public void PanScreen(float x, float y)
    {
        Vector3 direction = PanDirection(x, y);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTransform.position + (Vector3)direction * panSpeed, Time.deltaTime);
    }

}
