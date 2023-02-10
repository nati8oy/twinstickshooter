using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<LevelManager>
{
    public delegate void EnemyDeath();
    public static event EnemyDeath OnDeath;

    public delegate void LevelComplete();
    public static event LevelComplete onLevelComplete;
    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
        {
            if (OnDeath != null)
                OnDeath();
        }
    }
    */
 
}
