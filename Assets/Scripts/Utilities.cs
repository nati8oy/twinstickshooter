using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class Utilities 
{
    public static bool levelComplete;

    public static void DebugKillAllEnemies()
    {
        var checkEnemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in checkEnemies)
        {
            enemy.SetActive(false);
            GameManager.Instance.EnemyCheck();
        }
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
    }


}
