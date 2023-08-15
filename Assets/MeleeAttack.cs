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

    void Start()
    {
        //feedbackPlayer = new MMF_Player();

        meleeAttack = new InputAction(binding: "<Mouse>/leftButton");
        meleeAttack.performed += _ => Swing();
        meleeAttack.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Swing()
    {
        animator.SetBool("Attack", true);
        feedbackPlayer.PlayFeedbacks();
    }

    private void StopSwing()
    {
        animator.SetBool("Attack", false);

    }
}
