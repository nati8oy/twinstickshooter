using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class MeleeAttack : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputAction meleeAttack;
    [SerializeField] private MMF_Player feedbackPlayer;
    [SerializeField] private Weapon weapon;


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
            collision.gameObject.GetComponent<EnemyHealth>().Damage(weapon.damage);

            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(weapon.knockback, transform.position, 2f);

            feedbackPlayer.PlayFeedbacks();

            //Debug.Log("Hit feedbacks played");

            if (hitDamage >= 500f)
            {
            }

        }        
    }
}
