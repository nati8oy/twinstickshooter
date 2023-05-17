using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pickup", menuName = "Pickup")]


public class Pickup : ScriptableObject
{

    //public GameObject model;
    public string collidesWithTag;
    public string type;
    public int strength;
    public float lifespan;
    public bool hasLifespan;

}
