using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction action;
    private bool followCam = true;
    [SerializeField] public CinemachineVirtualCamera vcam1; // follow cam
    [SerializeField] public CinemachineVirtualCamera vcam2; // edit mode


    // Start is called before the first frame update

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }


    
    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
    private void Start()
    {
        action.performed += _ => SwitchPriority();
    }
    

    public void SwitchPriority()
    {
        if (followCam)
        {

            vcam1.Priority = 0;
            vcam2.Priority = 1;

        }
        else
        {

            vcam1.Priority = 1;
            vcam2.Priority = 0;

        }

        followCam = !followCam;

    }

}