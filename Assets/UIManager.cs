using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditor.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    public TextMeshProUGUI resourceInput;
    public GameObject builderHUD;
    public GameObject gameHUD;
    public GameObject gameoverUI;
    //public bool enableUI;

    [SerializeField] private GameManager gameManager;


    private void OnEnable()
    {
        //builderHUD.SetActive(false);
        gameHUD.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        resourceInput.SetText(GameManager.resourceCount.ToString());


    }
    
    // Update is called once per frame
    void Update()
    {
        resourceInput.SetText(GameManager.resourceCount.ToString());


        /*
        if (GameManager.levelComplete ==true)
        {
            builderHUD.SetActive(true);
            gameHUD.SetActive(false);
        }
        else
        {
            builderHUD.SetActive(false);
            gameHUD.SetActive(true);
        }*/
    }

}
