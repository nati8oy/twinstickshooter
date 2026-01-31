using System;
using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;



public class PlayerAttackManager : MonoBehaviour
{
    [SerializeField] private InputAction shoot;


    private void Start()
    {
        shoot = new InputAction(binding: "<Mouse>/leftButton" );
        shoot.performed += _ => OnShootPrimary();
        shoot.Enable();
    }
    //private  PlayerControls playerControls;


    private void OnShootPrimary()
    {

    }
  
}
