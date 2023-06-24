using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // make this enemy move in a direction until it collides with a wall
        // then change direction to the opposite direction

        // if the enemy is moving right
        if (transform.position.x < 10)
        {
            // move right
            transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
        }
        else
        {
            // move left
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
        }

        // if the enemy is moving up
        if (transform.position.z < 10)
        {
            // move up
            transform.position += new Vector3(0, 0, 1) * Time.deltaTime;
        }
        else
        {
            // move down
            transform.position += new Vector3(0, 0, -1) * Time.deltaTime;
        }

        //make the enemy randomly change direction every 4 seconds
        if (Time.time % 4 == 0)
        {
          // change direction

           // if the enemy is moving right
          if (transform.position.x < 10)
          {
              // move right
               transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
          }

           // if the enemy is moving left
            if (transform.position.x > -10)
          {
                // move left
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
        }

        // if the enemy is moving up
        
            if (transform.position.z < 10)
        {
                // move up
            transform.position += new Vector3(0, 0, 1) * Time.deltaTime;
        }

        // if the enemy is moving down

            if (transform.position.z > -10)
        {
                // move down
            transform.position += new Vector3(0, 0, -1) * Time.deltaTime;
        }
        }

        
    }
}
