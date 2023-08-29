using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackSO")] 
public class AttackSO : ScriptableObject
{

    public AnimatorOverrideController AnimatorOverrideController;
    public float damage;
    public float knockback;

}
