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

    Structure structure;
    public int currentItem;

    private GameObject currentStructure;

    public CinemachineSwitcher cameraSwitcher;


    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;

    private void OnEnable()
    {
        currentItem = 0;
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
        structure.DestroyGhosts();
    }



    /// <summary>
    /// all of the edit mode stuff is handled here but the actual controls to trigger it are back in the playercontroller script
    /// </summary>

    public void EditMode()
    {

        //toggle between edit/building and play mode
        if (GameManager.Instance.gameState == GameManager.GameState.play)
        {
            //slow down the overall timescale
            MMTimeManager.Instance.SetTimeScaleTo(0.3f);

            //add the ghost object to the screen
            AddGhostToScreen(currentItem);

            //change the virtual camera priority
            cameraSwitcher.SwitchPriority();

            CameraFollowCheck();

            GameManager.Instance.gameState = GameManager.GameState.building;

        }
        else if (GameManager.Instance.gameState == GameManager.GameState.building)
        {
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
        //add the ghost object to the screen
        //check if there is a ghost on screen from which the transform can be taken

        if (currentStructure != null)
        {
            structures[index].InstantiateObject(currentStructure.transform.position);
        }

        else
        {
            //if there is no ghost on screen when clicked, just add the object where the player is posiitoned
            structures[index].InstantiateObject(GameObject.Find("Player Alt").transform.position);

        }
    }

    public void Cycle(string direction)
    {
       


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