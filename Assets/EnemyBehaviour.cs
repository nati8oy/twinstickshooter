using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBehaviour : MonoBehaviour
{

    //[SerializeField] private LevelManager levelManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] EnemyData enemyData;

    [SerializeField] EnemyNavMesh enemyNavMesh;

    public float hp;
    public float moveSpeed;
    private int damage;
    private int maxHealth;
    public int currentHealth;
    public float attackRange = 10f;

    public GameObject player;

    public Renderer renderer;
    public Material enemyMaterial;

    private bool followPlayer;

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
        //set the resource object up using the right tag so it can be switched in the switch statement later on.

    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {

        switch (state)
        {
            default:
            case State.Roaming:
                //Debug.Log("Roaming");
                enemyNavMesh.isFollower = false;
                FindTarget();

            break;
            case State.ChaseTarget:
                //Debug.Log("Chasing");
                enemyNavMesh.isFollower = true;

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

    // Start is called before the first frame update
    void Start()
    {
       // resource = ObjectPooler.SharedInstance.GetPooledObject(resourceDrop);


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
            damagable.Damage(1);

        }

       


    }

    /*
    private void OnCollisionExit(Collision collision)
    {
        
            gameObject.GetComponent<EnemyNavMesh>().enabled = true;
            gameObject.GetComponent<EnemyBehaviour>().enabled = true;
        gameObject.GetComponent<EnemyNavMesh>().isFollower = true;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;


    }*/


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

        resource = ObjectPooler.SharedInstance.GetPooledObject(resourceDrop);

        //GameObject explosion = ObjectPooler.SharedInstance.GetPooledObject("explosion");

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
            //check the type of enemy and select the right type of resource to drop

            /*
            //set the probability range for a drop of any kind
            int probability = Random.Range(1, 101);


            //probability of dropping anything at all is 25% using this formula
            if (Random.Range(1, 101) <= 25)
            {
                //pull the resourceDrop type from the data object and put it here
                switch (resourceDrop)
                {
                    case "fire":
                        resource.transform.position = gameObject.transform.position;
                        resource.SetActive(true);
                        break;

                    case "health":
                        resource.transform.position = gameObject.transform.position;
                        resource.SetActive(true);
                        break;
                    case "ammo":
                        resource.transform.position = gameObject.transform.position;
                        resource.SetActive(true);
                        break;
                    case "none":
                        resource.transform.position = gameObject.transform.position;
                        resource.SetActive(true);
                        break;
                }
            }*/
        }

    }

}
