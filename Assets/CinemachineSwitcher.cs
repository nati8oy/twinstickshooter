using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction action;
    private bool followCam = true;
    [SerializeField] private CinemachineVirtualCamera vcam1; // follow cam
    [SerializeField] private CinemachineVirtualCamera vcam2; // edit mode


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
            Debug.Log("vcam 1 priority: " + vcam1.Priority);
            Debug.Log("vcam 2 priority: " + vcam2.Priority);

            vcam1.Priority = 0;
            vcam2.Priority = 1;

        }
        else
        {
            Debug.Log("swapped back");

            vcam1.Priority = 1;
            vcam2.Priority = 0;

            Debug.Log("vcam 1 priority: " + vcam1.Priority);
            Debug.Log("vcam 2 priority: " + vcam2.Priority);
        }

        followCam = !followCam;

    }

}