using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    public HealthBar healthBar;

    //health related
    public int maxHealth = 100;
    public int currentHealth;
    public bool playerDead;
    public PlayerInput playerInput;


    //player XP related
    public HealthBar levelBar;
    public PlayerLevels playerLevels;
    public static int currentXPLevel;
    public int playerLevelMax;

    public Pickup XP;
   [SerializeField] private ParticleSystem playerDeath;


    public CharacterController characterController;
    [SerializeField] public LevelManager levelManager;


    private void OnEnable()
    {
        playerDead = false;
        currentXPLevel = 0;
        playerLevelMax = playerLevels.level1;
        levelBar.SetAmount(currentXPLevel);




    }
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        healthBar.SetMaxBarAmount(maxHealth);

        //set the health amount from the amount allocated in the player levels object  
        levelBar.SetMaxBarAmount(playerLevelMax);

   

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.levelComplete || playerDead)
        {
            characterController.enabled = false;

        } else
        {
            characterController.enabled = true;
        }
    }


    

    private void OnCollisionEnter(Collision collision)
    {

        if (DebugManager.Instance.godMode == false)
        {

            if (collision.gameObject.tag == "enemy")        
            {
                TakeDamage(10);
            }
            if (collision.gameObject.tag == "enemy bullet")
            {
                TakeDamage(10);
            }
            if (collision.gameObject.tag == "XP")
            {
                currentXPLevel += XP.strength;
                levelBar.SetAmount(currentXPLevel);
            }

        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetAmount(currentHealth);
        if (currentHealth <= 0)
        {
            playerDead = true;

            UIManager.Instance.gameoverUI.SetActive(true);
  
            gameObject.SetActive(false);
            Instantiate(playerDeath, gameObject.transform);
            playerDeath.Play();

        }
    }

    public void Heal(int health)
    {   
        //Debug.Log("health picked up " + health);
        if (currentHealth < maxHealth)
        {
            currentHealth += health;
            healthBar.SetAmount(currentHealth);
        }
    }


}
