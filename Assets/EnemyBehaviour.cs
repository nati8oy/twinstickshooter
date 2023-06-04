using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;


public class EnemyBehaviour : MonoBehaviour
{

    //[SerializeField] private LevelManager levelManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] EnemyData enemyData;

    [SerializeField] EnemyNavMesh enemyNavMesh;

    [SerializeField] MMFeedbacks feedback;

    public UnityEvent onEnemyHit;

    public float hp;
    public float moveSpeed;
    private int damage;
    private int maxHealth;
    public int currentHealth;
    public float attackRange = 10f;

    public GameObject player;

    public Renderer renderer;
    public Material enemyMaterial;

    private bool findEndPoint;

    private bool canAttack;

    private string attackMode; 

    public float rateOfFire;
    public float cooldown;
    public int XPdrop;
    public GameObject resource;

    public GameObject XPObject;

    public Weapon weapon;
    //public ParticleSystem explosionParticles;

    private string resourceDrop;

    private State state;

    private enum State
    {
        Roaming,
        ChaseTarget,
        Attack,
    }

    private void Awake()
    {
         state = State.Roaming;

    }

    private void OnEnable()
    {
        //pull all of the values from the enemy data object
        maxHealth = enemyData.health;
        damage = enemyData.damage;
        rateOfFire = weapon.attackRate;
        canAttack = enemyData.canAttack;
        resourceDrop = enemyData.resourceDrop[Random.Range(0,4)].ToString();
        enemyMaterial = enemyData.enemyMaterial;
        XPdrop = enemyData.XP;


       // findEndPoint = enemyData.followPlayer;


        //find the bool in the enemyNav Mesh and fill it with whatever has been set in the data object
       // enemyNavMesh.followPlayer = findEndPoint;
    }

    private void Update()
    {

        switch (state)
        {
            default:
            case State.Roaming:
                //Debug.Log("Roaming");
                enemyNavMesh.followPlayer = true;

                FindTarget();

                break;
            case State.ChaseTarget:
                //Debug.Log("Chasing");
                enemyNavMesh.followPlayer = true;

                Debug.DrawLine(transform.position, GameManager.Instance.player.transform.position, color: Color.red);


                float exitRangeDistance = 15f;
                if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) > exitRangeDistance)
                {
                    //player within range
                    state = State.Roaming;
                }

                float attackRange = 10f;
                if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < attackRange)
                {
                    //Debug.Log("In attack range!");
                    //state = State.Attack;

                }

                //player within attack range and has canAttack enabled
                if (canAttack)
                {
                    if (Time.time > cooldown)
                    {
                        //triggers attack by going through the interface IAttack
                        gameObject.GetComponent<IAttack>().TimedAttack();
                        //cooldown = Time.time + rateOfFire;
                    }
                }

                break;
            case State.Attack:

                break;

        }


           

    }

    void Start()
    {

        currentHealth = maxHealth;

        //set the material up to change based on enemyData material
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = enemyMaterial;


    }

    private void OnCollisionEnter(Collision collision)
    {

        //use the interface to damage whatever is in its path that has the destructible script on it
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            //deals the amount of damage from the actual enemy data object
            damagable.Damage(damage);
            //Debug.Log("damage dealt: " + damage);
        }
    }


    public void FindTarget()
    {

        float targetRange = 10f;
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < targetRange)
        {
            //player within range
            state = State.ChaseTarget;
            Debug.DrawLine(transform.position, GameManager.Instance.player.transform.position, color:Color.red);
        }
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
