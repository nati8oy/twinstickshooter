using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]


public class EnemyData : ScriptableObject
{
    [Header("Attributes")]
    [Tooltip("Total enemy HP (int)")]
    public int health;
    [Tooltip("Default enemy damage amount (int)")]
    public int damage;
    [Tooltip("Rate of attack. Lower is faster (float) ")]
    public float attackSpeed;
    [Tooltip("Tag of the game object within the objectPooler prefab")]
    public string objectPoolerTag;
    [Tooltip("Speed of movement ")]
    public int moveSpeed;
    [Tooltip("Does the enemy have any shield?")]
    public bool shield;
    [Tooltip("Does the enemy just chase the player as its default behaviour?")]
    public bool followPlayer;
    [Tooltip("Can this enemy attack?")]
    public bool canAttack;

    [Header("Game Object Values")]
    [Tooltip("Default material for the enemy")]
    public Material enemyMaterial;
    [Tooltip("3D game object used for this enemy")]
    public GameObject enemyModel;

    [Header("Resources")]
    [Tooltip("What resource does the enemy drop when it dies?")]
    public string[] resourceDrop;
    [Tooltip("How much XP does this enemy drop when it dies?")]
    public int XP;


}
