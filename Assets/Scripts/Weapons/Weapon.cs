using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]


public class Weapon : ScriptableObject
{
    public int damage;
    public int shotSpeed;
    public float attackRate;
    //bullet lifetim
    public float bulletLifespan;
    //public GameObject ammoType;
    public string ammoType;
    public int ammo;
    public float cooldown;

}
