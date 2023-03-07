using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;

using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    public CharacterController characterController;

    //this is used for the target tha the enemies are trying to get to
    public Transform endPoint;

    [SerializeField] private GameObject builderHUD;
    [SerializeField] private GameObject gameHUD;

    [SerializeField] private GameObject feedbackPlayer;
    public GameObject player;

    public Camera[] cameraList; 

    //public CinemachineVirtualCamera virtualCamera;

    public float divider = 0.1f;

    public static bool levelComplete;


    public static float resourceCount = 0f;
    private int randomNumber;


    public bool gameOver;

    //test



    public static int maxEnemies;


    //used to set the game state so that we can tell if it's in build mode or not.
    public GameState gameState;

    public enum GameState{
        play,
        paused,
        building,
        dead,
    }


    void Start()
    {
        //set the current state of the game to playing
        gameState = GameState.play;

    }



    public void EnemyCheck()
    {
        var checkEnemies = GameObject.FindGameObjectsWithTag("enemy");
        //Debug.Log(checkEnemies.Length + " enemies remaining");
        if (checkEnemies.Length <= 1)
        {
           // Debug.Log("checked for enemies");
            levelComplete = true;
            LevelManager.Instance.LevelComplete();

            //set the game state to build mode
            gameState = GameState.building;
            
        }
    }

    public void GameStateManager()
    {

    }

}
