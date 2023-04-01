using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;



public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameManager gameManager;
    public LayerMask layerMask;



    public static int enemyKillCount;
    public static int maxEnemies;

    public bool levelComplete;
    public int currentLevel;
    public int enemyCount;
    public float spawnRate = 3f; 

    [SerializeField] private GameObject[] spawnPoints;


    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
    }

    private Difficulty difficulty;
    private int randomNumber;

    void Start()
    {

        

        //start spawning enemies
        StartCoroutine(EnemySpawn());
        //manually set difficulty level
        difficulty = Difficulty.Hard;

        //currentLevel = 1;
        //SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);

        enemyKillCount = 0;
        maxEnemies = 20;
        GameManager.maxEnemies = maxEnemies;

    }


    public void UpdateKillCount()
    {
        //update the tracker for the number of enemies killed
        enemyKillCount += 1;
    }

    public void LevelComplete()
    {
       

        switch (difficulty)
        {
            default:
            case Difficulty.Easy:
                //add 5 more enemies for each level
                maxEnemies += 5;
                break;
            case Difficulty.Medium:
                //add 10 more enemies for each level
                maxEnemies += 10;
                break;
            case Difficulty.Hard:
                //add 20 more enemies for each level
                maxEnemies += 20;
                break;
        }

        ///
        ///Legay stuff from when the build mode was only at the end of the level
        ///
        /*
        //load the next scene on top of the current scene.
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        //RandomEnemySpawn.Instance.maxEnemies = currentLevel * maxEnemies;

        //set the game state back to play
        GameManager.Instance.gameState = GameManager.GameState.play;
        */
    }

    



    /*
    public void NextLevel()
    {
        //mainCamera.enabled = true;

        //SceneManager.UnloadScene(2);

        //SceneManager.UnloadScene(1);

        //load the next scene on top of the current scene.
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        //RandomEnemySpawn.Instance.maxEnemies = currentLevel * maxEnemies;

        //set the game state back to play
        GameManager.Instance.gameState = GameManager.GameState.play;
    }

    */


    public void SpawnEnemy()
    {

        randomNumber = Random.Range(0, 5);

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject("enemy");


        if (enemy != null)
        {
            //spawn enemies at the first location from the number of spawn points in this array
            enemy.transform.position = spawnPoints[0].transform.position;
            enemy.SetActive(true);
        }

    }

    IEnumerator EnemySpawn()
    {
        SpawnEnemy();
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(spawnRate);

        StartCoroutine(EnemySpawn());
     
    }

}
