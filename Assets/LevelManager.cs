using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameManager gameManager;

    
    public static int enemyKillCount;
    public static int maxEnemies;

    public bool levelComplete;
    public int currentLevel;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
    }

    private Difficulty difficulty;

    void Start()
    {
        //manually set difficulty level
        difficulty = Difficulty.Hard;

        currentLevel = 1;
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);

        enemyKillCount = 0;
        maxEnemies = 5;
        GameManager.maxEnemies = maxEnemies;

    }


    public void UpdateKillCount()
    {
        //update the tracker for the number of enemies killed
        enemyKillCount += 1;
    }

    public void LevelComplete()
    {
        //set the game state to build mode
        GameManager.Instance.gameState = GameManager.GameState.building;



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
      
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

    }

    public void NextLevel()
    {
        //mainCamera.enabled = true;

        SceneManager.UnloadScene(2);

        SceneManager.UnloadScene(1);

        //load the next scene on top of the current scene.
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        //RandomEnemySpawn.Instance.maxEnemies = currentLevel * maxEnemies;

        //set the game state back to play
        GameManager.Instance.gameState = GameManager.GameState.play;
    }
}
