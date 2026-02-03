using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hookshot Data")] 
public class HookshotData : ScriptableObject
{
    public float strength;
    public int level;
    public float maxRange;
    //min and max speed is divided in the hookshot script to give you a minimum speed you will travel and a maximum speed you can travel
    public float speedMin;
    public float speedMax;
    public float bounciness;
    public float maxLineDistance;
}