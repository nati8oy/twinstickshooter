using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuilderManager : Singleton<BuilderManager>
{
    [SerializeField] public GameManager gameManager;

    Structure structure;

    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;


    public void SpawnBlock()
    {
        Debug.Log("new block");

        Vector3 randomPosition = new Vector3(Random.Range(-itemXSpread, itemXSpread), Random.Range(-itemYSpread, itemYSpread), Random.Range(-itemZSpread, itemZSpread));

        GameObject block = ObjectPooler.SharedInstance.GetPooledObject("environment");

        if (block != null)
        {
            block.transform.position = randomPosition;
            block.SetActive(true);
        }
    }

    public void ReturnToGame()
    {
        GameManager.levelComplete = false;
        GameManager.maxEnemies = 5;
        LevelManager.Instance.NextLevel();
        structure.DestroyGhosts();
        //GameManager.Instance.GetComponent<Camera>().enabled = true;
    }



}
