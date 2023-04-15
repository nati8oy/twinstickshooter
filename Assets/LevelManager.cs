using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameManager gameManager;
    public LayerMask layerMask;

    

    public static int enemyKillCount;
    public static int maxEnemies;

    public bool levelComplete;
    public int currentLevel;
    public int enemyCount;
    [SerializeField] private GameObject[] spawnPoints;


    [Header("Spawn Rate Controllers")]
    public float enemiesPerIncrease = 10;
    public bool increaseSpawnRate;
    public float spawnRateCounter = 5;
    public float spawnRateIncrease = 0.1f;
    public float spawnRate = 3f;
    private float minimumSpawnRate = 1.25f;




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


        spawnRateCounter = spawnRate;

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
        if (increaseSpawnRate)
        {
            spawnRateCounter++;

            if ((spawnRateCounter % enemiesPerIncrease == 0) && (spawnRateCounter > minimumSpawnRate))
            {
                Debug.Log("spawn rate increased");
                spawnRate-= spawnRateIncrease;

                //creates a rare enemy when the spawn speed increases
                GameObject rareEnemy = ObjectPooler.SharedInstance.GetPooledObject("enemy_lvl2");
                rareEnemy.transform.position = spawnPoints[randomNumber].transform.position;
                rareEnemy.SetActive(true);
                int layerIndex = LayerMask.NameToLayer("Enemies");
                rareEnemy.layer = layerIndex;


            }
        }

        

        randomNumber = Random.Range(0, 3);

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject("enemy");


        if (enemy != null)
        {
            //spawn enemies at the first location from the number of spawn points in this array
            enemy.transform.position = spawnPoints[randomNumber].transform.position;
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
