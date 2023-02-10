using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class InventorySlot {


    public Item item;
    public Button button;

    public InventorySlot(Item item, VisualTreeAsset template)
    {

        //set a template container
        TemplateContainer itemButtonContainer = template.Instantiate();
        this.item = item;

        //button.style.backgroundImage = new StyleBackground(item.icon);


        button = itemButtonContainer.Q<Button>();
        button.RegisterCallback<ClickEvent>(OnClick);


    }

    public void OnClick(ClickEvent evt)
    {
        Debug.Log("inventory item is " + item.displayName);
    }
}
