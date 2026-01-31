using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySpawn : Singleton<RandomEnemySpawn>
{


    public float itemXSpread = 10;
    public float itemYSpread = 20;
    public float itemZSpread = 10;
    public float maxEnemies;

    private void OnEnable()
    {
        //maxEnemies = LevelManager.currentLevel*maxEnemies;
        maxEnemies = LevelManager.maxEnemies ;
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i<maxEnemies; i++)
        {
            RandomEnemySpawner();

        }
    }

    public void RandomEnemySpawner()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-itemXSpread, itemXSpread), Random.Range(-itemYSpread, itemYSpread), Random.Range(-itemZSpread, itemZSpread));

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject("enemy");

        if (enemy != null)
        {
            enemy.transform.position = randomPosition;
            enemy.SetActive(true);
        }
    }
}
