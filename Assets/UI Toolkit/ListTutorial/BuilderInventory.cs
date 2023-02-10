using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderInventory : MonoBehaviour
{
    UIDocument uiInventory;
    public VisualTreeAsset itemButtonTemplate;
    public List<Item> items;

    private void OnEnable()
    {
        //assign the component data
        uiInventory = GetComponent<UIDocument>();


        foreach (Item item in items)
        {
            InventorySlot newSlot = new InventorySlot(item, itemButtonTemplate);

            //get the root visual
            uiInventory.rootVisualElement.Q("ItemRow").Add(newSlot.button);

        }



    }
}
