using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyAttributes : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] private GameManager gameManager;


    private float hp;
    public float moveSpeed;
    private int damage;
    private int maxHealth;
    private int currentHealth;
    private float attackRange = 10f;
    public GameObject player;


    private Material enemyMaterial;
    private bool canAttack;
    private float rateOfFire;
    private float cooldown;
    private int XPdrop;


    public Weapon weapon;


    public UnityEvent onEnemyHit;
    private GameObject resource;



    //public ParticleSystem explosionParticles;

    private string resourceDrop;

    private void OnEnable()
    {
        //pull all of the values from the enemy data object
        maxHealth = enemyData.health;
        damage = enemyData.damage;
        rateOfFire = weapon.attackRate;
        canAttack = enemyData.canAttack;
        resourceDrop = enemyData.resourceDrop[Random.Range(0, 4)].ToString();
        enemyMaterial = enemyData.enemyMaterial;
        XPdrop = enemyData.XP;


        // findEndPoint = enemyData.followPlayer;


        //find the bool in the enemyNav Mesh and fill it with whatever has been set in the data object
        // enemyNavMesh.followPlayer = findEndPoint;
    }

    public void Damage(int damageAmount)
    {
        //invokes the hit flicker from the MMFeedbacks plugin.
        onEnemyHit.Invoke();
        resource = ObjectPooler.SharedInstance.GetPooledObject(resourceDrop);

        maxHealth -= damageAmount;
        currentHealth = maxHealth;



        float randVelY = Random.Range(50f, 100f);


        if (maxHealth <= 0)
        {

            gameObject.GetComponent<LootBag>().DropLoot();

            //add to the kill count i 
            LevelManager.enemyKillCount += 1;

            gameManager.EnemyCheck();
            //set enemy to inactive
            gameObject.SetActive(false);

        }

    }
}
