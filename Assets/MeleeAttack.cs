using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using UnityEngine.AI;

public class MeleeAttack : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputAction meleeAttack;
    [SerializeField] private MMF_Player feedbackPlayer;
    [SerializeField] private Weapon weapon;
  
    private bool stun;

    [SerializeField] float hitDamage = 20f;
    private bool playedFeedbacks;

    // Update is called once per frame
    void Update()
    {
      
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            /*
            if (collision.gameObject.GetComponent<NavMeshAgent>())
            {
                collision.gameObject.GetComponent<NavMeshAgent>().enabled = false;

            }
            */
 
            if (weapon.stun)
            {
                collision.gameObject.GetComponent<EnemyStun>().Stun(weapon.stunTime);
                collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(weapon.knockback, transform.position, 2f);
                feedbackPlayer.PlayFeedbacks();
            }

            else
            {
                collision.gameObject.GetComponent<EnemyHealth>().Damage(weapon.damage);
                
            }

           


        }        
    }
}
