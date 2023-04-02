using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditor.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

   // public TextMeshProUGUI resourceInput;
   // public GameObject builderHUD;
    public GameObject gameHUD;
    public GameObject gameoverUI;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private EndPoint endPoint;
    //public bool enableUI;

    [SerializeField] private GameManager gameManager;


    private void OnEnable()
    {
        gameHUD.SetActive(true);

        //subscribes to an event called PlayerDeath that is inside od the playerManager script
        playerManager.PlayerDeath += GameOver;
        //endPoint.BaseDeath += GameOver;

    }

    public void GameOver()
    {
        //show the game over UI screen 
        gameoverUI.SetActive(true);
    }

    private void OnDestroy()
    {
        //ubsubscribe on destroy
        playerManager.PlayerDeath -= GameOver;
       // endPoint.BaseDeath -= GameOver;

    }
}
