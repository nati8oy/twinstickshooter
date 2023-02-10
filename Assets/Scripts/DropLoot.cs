using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour, IDropLoot
{
    public bool dropsLoot;
    public GameObject lootObject;
    public int lootDropped;
    [SerializeField] EnemyData lootDataObject;



    private void OnEnable()
    {
        //use a data object to see what kind of loot to drop
        lootDropped = lootDataObject.XP;
        dropsLoot = true;

    }

    private void OnDisable()
    {
        //set this value to false due to the fact that it runs before the object is visible.
        //without this the function will be triggered before and after the object becomes visible.
        //I know, WTF?!
        //dropsLoot = false;
       // LootDrop();

    }

    public void LootDrop()
    {
        if (dropsLoot)
        {

        float randVelY = Random.Range(50f, 100f);

        //drop XP (happens with every kill)
        //these values below such as randX are to add random force to the loot in all different directions.
        for (int i = 0; i < lootDropped; i++)
        {
            float randX = Random.Range(-0.5f, 0.5f);
            float randZ = Random.Range(0.5f, 1f);

            lootObject = ObjectPooler.SharedInstance.GetPooledObject("XP");
            lootObject.transform.position = new Vector3(gameObject.transform.position.x + randX, gameObject.transform.position.y + 1, gameObject.transform.position.z + randZ);
            lootObject.SetActive(true);


            lootObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, randVelY, 0));
        }
        //Debug.Log("dropped loot!");
        }
    }
}
