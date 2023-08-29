using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
   [SerializeField] private List<AttackSO> combo;
    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

    [SerializeField] private Rigidbody playerRB;

    [Header("Attack Settings")]
    [SerializeField] private float comboDelay = 0.5f;
    [SerializeField] private float comboAttackDelay = 0.2f;

    private bool hasFired;

    [SerializeField] Animator animator;
    [SerializeField] Weapon weapon;



    [SerializeField] private MMF_Player feedbackPlayer;
    [SerializeField] private UnityEvent onSwing;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();

        // Check if the left mouse button is pressed and the fire rate cooldown has passed
        if (Mouse.current.leftButton.isPressed && !hasFired)
        {
            hasFired = true;

            // Fire a bullet
            Attack();
            
        }

        else if (!Mouse.current.leftButton.isPressed)
        {
            // Reset the flag when the button is released
            hasFired = false;
        }
    }

    private void Attack()
    {
        

        if (Time.time - lastComboEnd > comboDelay && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= comboAttackDelay)
            {

                animator.runtimeAnimatorController = combo[comboCounter].AnimatorOverrideController;


                animator.Play("Attack", 0, 0);

                onSwing.Invoke();




                //weapon.damage = combo[comboCounter].damage;
                //add knockback here
                //add VFX or particle effects here

                comboCounter++;
                lastClickedTime = Time.time;


                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    private void ExitAttack()
    {
        
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.90f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    private void EndCombo()
    {
        //Debug.Log("Combo Ended");
        comboCounter = 0;
        lastComboEnd = Time.time;

    }

 
}
