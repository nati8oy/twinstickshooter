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


    public void ReturnToGame()
    {
        GameManager.Instance.gameState = GameManager.GameState.play;
        GameManager.levelComplete = false;
        GameManager.maxEnemies = 5;
        //LevelManager.Instance.NextLevel();
        structure.DestroyGhosts();
        //GameManager.Instance.GetComponent<Camera>().enabled = true;
    }



    /// <summary>
    /// all of the edit mode stuff is handled here but the actual controls to trigger it are back in the playercontroller script
    /// </summary>

    public void EditMode()
    {
        //change the game mode to building
        ///Debug.Log("Entered Build Mode");

        //toggle between edit/building and play mode
        if (GameManager.Instance.gameState == GameManager.GameState.play)
        {
            //slow down the overall timescale
            //MMTimeManager.Instance.SetTimeScaleTo(0.5f);

            //add the ghost object to the screen
            AddGhostToScreen(currentItem);


            //change the virtual camera priority
            cameraSwitcher.SwitchPriority();

            //set the follow target of the virtual camera to the ghost in edit mode
            cameraSwitcher.vcam2.m_LookAt = GameObject.FindGameObjectWithTag("ghost").transform;
            cameraSwitcher.vcam2.m_Follow = GameObject.FindGameObjectWithTag("ghost").transform;

            //mainCamera = GameManager.Instance.cameraList[1];
            //mainCamera = GameObject.Find("CameraRigBase").GetComponentInChildren<Camera>();

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
        structures[index].InstantiateObject(GameObject.Find("Player Alt").transform.position);
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

            structures[currentItem].DestroyGhosts();
            AddGhostToScreen(currentItem);
            Debug.Log("right: current = " + currentItem);


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

            Debug.Log("left: current = " + currentItem);
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
}