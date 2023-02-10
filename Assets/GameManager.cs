using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    public CharacterController characterController;


    [SerializeField] private GameObject builderHUD;
    [SerializeField] private GameObject gameHUD;

    [SerializeField] private GameObject feedbackPlayer;
    public GameObject player;

    //public CinemachineVirtualCamera virtualCamera;

    public float divider = 0.1f;
    public float timeInterval = 3f;
    public string gameMode;

    public static bool levelComplete;


    public static float resourceCount = 0f;
    private int randomNumber;

    public int enemyCount;

    public bool gameOver; 


    public static int maxEnemies;


    //used to set the game state so that we can tell if it's in build mode or not.
    public GameState gameState;

    public enum GameState{
        play,
        paused,
        building,
    }

    [SerializeField] private GameObject[] spawnPoints;

    void Start()
    {
        //set the current state of the game to playing
        gameState = GameState.play;

    //Start the coroutine we define below named EnemySpawn.
    gameMode = "tws";
        //StartEnemySpawn();


    }


    public void SpawnEnemy()
    {

        randomNumber = Random.Range(0,6);

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject("enemy");

        if (enemy != null)
        {
            enemy.transform.position = spawnPoints[randomNumber].transform.position;
            enemy.SetActive(true);
        }

    }

    IEnumerator EnemySpawn()
    {
        SpawnEnemy();
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(timeInterval);


        if (gameMode == "tws" && enemyCount < maxEnemies)
        {
            StartCoroutine(EnemySpawn());
            enemyCount += 1;
        }
    }
    public void StartEnemySpawn()
    {
        enemyCount = 1;
        //check the game mode is tws before starting the coroutine.
        //tws means twin stick shooter. The other mode is building mode. 
        if (gameMode == "tws" && enemyCount < maxEnemies)
        {
            StartCoroutine(EnemySpawn());

        }
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
        }
    }

  

}
