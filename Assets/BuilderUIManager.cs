using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderUIManager : MonoBehaviour
{
    //reference the UI document
    public UIDocument mainUIdoc;

    VisualTreeAsset m_ListEntryTemplate;


    public Button blockButton;
    ListView ShopItemList;
    Label resourceName;



    private void OnEnable()
    {
        mainUIdoc = GetComponent<UIDocument>();

        if (mainUIdoc == null)
        {
            Debug.LogError("no button found");
        }

        //represent visual element as a button
        blockButton = mainUIdoc.rootVisualElement.Q("ShopButton") as Button;

        if (blockButton != null)
        {
            Debug.Log("Button found!");
        }

//        blockButton.RegisterCallback<ClickEvent>(OnButtonClick);
    }

    public void OnButtonClick(ClickEvent evt)
    {
        Debug.Log("UI button clicked ;) ");
    }


}
