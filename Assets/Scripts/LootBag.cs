using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    private GameObject lootGameObject;
    //public GameObject lootItem;
    public List<Loot> lootList = new List<Loot>();

    public Loot droppedItem;
    //public int itemAmount;

    // Start is called before the first frame update
    void Start()
    {

    }

    Loot GetDroppedItem()
    {
        //set the probability range for a drop of any kind of loot

        int randomNumber = Random.Range(1, 101);
        List<Loot> possibleItems = new List<Loot>();
        foreach (Loot item in lootList)
        {
            if (randomNumber <= item.dropChance)
            {
                possibleItems.Add(item);
                
            }
        }
        if (possibleItems.Count > 0)
        {
            droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }

        return null;
    }

    public void DropLoot()
    {

        droppedItem = GetDroppedItem();

        if (droppedItem != null)
        {
            float randVelY = Random.Range(50f, 100f);

            float randX = Random.Range(-0.5f, 0.5f);
            float randZ = Random.Range(0.5f, 1f);

            //Debug.Log(droppedItem);
            lootGameObject = ObjectPooler.SharedInstance.GetPooledObject(droppedItem.lootName.ToString());
            lootGameObject.transform.position = new Vector3(gameObject.transform.position.x + randX, gameObject.transform.position.y + 1, gameObject.transform.position.z + randZ);
            lootGameObject.SetActive(true);


            lootGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, randVelY, 0));
            lootGameObject.GetComponent<Rigidbody>().AddTorque(transform.forward * 100, ForceMode.Impulse);

        }

    }
}
