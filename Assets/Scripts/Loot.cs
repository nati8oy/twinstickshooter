using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]


public class Loot : ScriptableObject
{       
    [Tooltip("Loot name must match tag of the game object within the objectPooler prefab")]
    public string lootName;
    [Tooltip("0-100 chance of dropping")]
    public int dropChance;


    public Loot (string lootName, int dropChance)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
    }


}
