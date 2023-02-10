using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{

    public Transform target;
    public float speed = 1f;
    public float countdownTimer;
    public float updateSpeed;
    private GameObject currentTarget;
    private GameObject[] targets;

    public float attackRange = 15f;

    List<float> enemyProximity = new List<float>();


    private void OnEnable()
    {
        //subscribe to the onLevelComplete event
        EventManager.onLevelComplete += TargetCheck;
    }

    private void OnDisable()
    {
        //unsubscribe from the onLevelComplete event
        EventManager.onLevelComplete -= TargetCheck;
    }

    private Coroutine LookCoroutine;

    private void Start()
    {
        StartRotating();
    }

    public void StartRotating()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt());
    }

    private IEnumerator LookAt()
    {

        if (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
            lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);


            float time = 0;

            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

                time += Time.deltaTime * speed;

                yield return null;
            }
        }

        else if (target == null)
        {
            Debug.LogError("No target available for turret " + gameObject.name + " right now");
        }
       
    }

    private void Update()
    {


        if (GameManager.Instance.gameState == GameManager.GameState.play)
        {
            //get all of the nearby enemies and put them in an array
            targets = GameObject.FindGameObjectsWithTag("enemy");

            //loop through that array and for each one see if the distance from the first one in the array [0] to the gun is closer than the attack range.
            foreach (GameObject targetEnemy in targets)
            {


                if (Vector3.Distance(transform.position, targets[0].transform.position) < attackRange)
                {
                    //this attack is based on a timer that can be set in the inspector
                    gameObject.GetComponent<IAttack>().TimedAttack();
                    Debug.DrawLine(transform.position, targets[0].transform.position, color: Color.red);

                    //Debug.Log("target acquired");
                }


            }
                
            if (Time.time > countdownTimer)
            {
                StartRotating();
                countdownTimer = Time.time + updateSpeed;
                targets = GameObject.FindGameObjectsWithTag("enemy");

                target = targets[0].transform;
            }

            /*
            if (currentTarget != null && (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < attackRange))
            {
                currentTarget = targets[0];
            } else if (targets[0] == null)
            {
                currentTarget = null;
            }
            */


        }


    }
    public void TargetCheck()
    {
        Debug.Log("target has been checked for the turret");
    }
}
