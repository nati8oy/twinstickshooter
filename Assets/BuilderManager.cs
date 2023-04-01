using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;


public class BuilderManager : Singleton<BuilderManager>
{
    [SerializeField] public GameManager gameManager;

    public List<Structure> structures = new List<Structure>();

    public int currentItem;

    private Vector3 prevGhostPosition;
    private Vector3 playerPos;

    public GameObject placeholderObj;

    private GameObject currentStructure;

    public CinemachineSwitcher cameraSwitcher;

    private Vector3 playerPositionOnMap;

    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;

    private void OnEnable()
    {
        currentItem = 0;
        playerPos = GameObject.Find("Player Alt").transform.position;

        prevGhostPosition = new Vector3(playerPos.x, 0, playerPos.z);
    }

    
    private void Update()
    {
        //this ensures that the edit mode camera is always locked to the ghost object
        if (GameManager.Instance.gameState == GameManager.GameState.building)
        {
            CameraFollowCheck();
        }
    }

    public void ReturnToGame()
    {
        GameManager.Instance.gameState = GameManager.GameState.play;
        GameManager.levelComplete = false;
        GameManager.maxEnemies = 5;
    }



    /// <summary>
    /// all of the edit mode stuff is handled here but the actual controls to trigger it are back in the playercontroller script
    /// </summary>

    public void EditMode()
    {
       playerPositionOnMap = GameObject.Find("Player Alt").transform.position;

        //change the game mode to building
        ///Debug.Log("Entered Build Mode");

        //toggle between edit/building and play mode
        if (GameManager.Instance.gameState == GameManager.GameState.play)
        {

            structures[currentItem].InstantiateObject(playerPositionOnMap);


            //slow down the overall timescale
            //MMTimeManager.Instance.SetTimeScaleTo(0.5f);

            //add the ghost object to the screen
            //AddGhostToScreen(currentItem);

            //change the virtual camera priority
            cameraSwitcher.SwitchPriority();

            CameraFollowCheck();

            //mainCamera = GameManager.Instance.cameraList[1];
            //mainCamera = GameObject.Find("CameraRigBase").GetComponentInChildren<Camera>();

            GameManager.Instance.gameState = GameManager.GameState.building;

        }
        else if (GameManager.Instance.gameState == GameManager.GameState.building)
        {
            //reset the player pos value for when the player re-enters building mode. 
            prevGhostPosition = new Vector3(playerPos.x, 0, playerPos.z); 
            //buildCamera.SetActive(false);
            MMTimeManager.Instance.SetTimeScaleTo(1f);
            GameManager.Instance.gameState = GameManager.GameState.play;

            //mainCamera = GameManager.Instance.cameraList[0];

            //mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            cameraSwitcher.SwitchPriority();

            //remove the ghost object that is in the scene.
            structures[currentItem].DestroyGhosts();

        }

    }

    private void AddGhostToScreen(int index)
    {

        structures[index].InstantiateObject(prevGhostPosition);

        /*
        Debug.Log("prev position: " + prevGhostPosition);
        if (currentStructure == null)
        {
            structures[index].InstantiateObject(playerPositionOnMap);
            Debug.Log("no prev");

        }

        else if(currentStructure!=null)
        {
            Debug.Log("using prev");
        }*/
    }

    public void Cycle(string direction)
    {

        //remove the previous ghost

        if (direction == "right")
        {

            if (currentItem == structures.Count - 1)
            {
                currentItem = 0;
            }

            else
            {
                currentItem += 1;
            }
            prevGhostPosition = GameObject.FindGameObjectWithTag("ghost").transform.position;

            structures[currentItem].DestroyGhosts();
            AddGhostToScreen(currentItem);

            //check each time you cycle through the items that the camera is still following the ghost.
            CameraFollowCheck();


        }

        else if (direction == "left")
        {

            if (currentItem == 0)
            {
                currentItem = structures.Count - 1;
            }

            else
            {
                currentItem -= 1;
            }

            prevGhostPosition = GameObject.FindGameObjectWithTag("ghost").transform.position;
            structures[currentItem].DestroyGhosts();
            AddGhostToScreen(currentItem);

            //check each time you cycle through the items that the camera is still following the ghost.
            CameraFollowCheck();

        }
    }

    public void RotateObject(string direction)
    {
        currentStructure = GameObject.FindGameObjectWithTag("ghost");
        if (direction == "right")
        {
            if (currentStructure.GetComponent<ObjectTransform>())
            {
                currentStructure.GetComponent<ObjectTransform>().onRotate90degrees.Invoke();
            }
        }

    }

    public void CameraFollowCheck()
    {
        //check each time you cycle through the items that the camera is still following the ghost.
        //set the follow target of the virtual camera to the ghost in edit mode
        cameraSwitcher.vcam2.m_LookAt = GameObject.FindGameObjectWithTag("ghost").transform;
        cameraSwitcher.vcam2.m_Follow = GameObject.FindGameObjectWithTag("ghost").transform;
    }
}