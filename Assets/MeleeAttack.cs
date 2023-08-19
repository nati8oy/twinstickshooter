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

    [SerializeField] private Animator animator;

    [SerializeField] private float attackRate;

    [SerializeField] private float nextAttackTime;

    [SerializeField] bool handSwitcher;

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button is pressed and the fire rate cooldown has passed
        if (Mouse.current.leftButton.isPressed && Time.time >= nextAttackTime)
        {
            // Fire a bullet
            Swing();

            // Set the next allowed fire time based on the fire rate
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void Swing()
    {
        if(handSwitcher)
        {
            animator.SetTrigger("AttackL");
            //Debug.Log("Left");
        }

        else
        {
            animator.SetTrigger("AttackR");
            // Debug.Log("Right");
            feedbackPlayer.PlayFeedbacks();

        }

        //animator.SetTrigger("Attack");

    }

    private void StopSwing()
    {
        //toggle the bool to alternate between hands when attacking
        handSwitcher = !handSwitcher;

    }
}
