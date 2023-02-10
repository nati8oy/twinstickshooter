using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StructureListController
{
    // UXML template for list entries
    VisualTreeAsset m_ListEntryTemplate;

    // UI element references
    ListView m_CharacterList;
    Label resourceName;
   // Label m_CharNameLabel;
    //VisualElement m_CharPortrait;

    public void IntialiseStructureList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        EnumerateAllCharacters();

        // Store a reference to the template for the list entries
        m_ListEntryTemplate = listElementTemplate;

        // Store a reference to the character list element
        m_CharacterList = root.Q<ListView>("character-list");

        // Store references to the selected character info elements
        resourceName = root.Q<Label>("character-class");
        //m_CharNameLabel = root.Q<Label>("character-name");
        //m_CharPortrait = root.Q<VisualElement>("character-portrait");

        FillCharacterList();

        // Register to get a callback when an item is selected
        m_CharacterList.onSelectionChange += OnCharacterSelected;
    }

    List<Structure> m_AllCharacters;

    void EnumerateAllCharacters()
    {
        m_AllCharacters = new List<Structure>();
        m_AllCharacters.AddRange(Resources.LoadAll<Structure>("Characters"));
    }

    void FillCharacterList()
    {
        // Set up a make item function for a list entry
        m_CharacterList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = m_ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new StructureListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        m_CharacterList.bindItem = (item, index) =>
        {
            (item.userData as StructureListEntryController).SetStructure(m_AllCharacters[index]);
        };

        // Set a fixed item height
        m_CharacterList.fixedItemHeight = 45;

        // Set the actual item's source list/array
        m_CharacterList.itemsSource = m_AllCharacters;
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedStructure = m_CharacterList.selectedItem as Structure;

        // Handle none-selection (Escape to deselect everything)
        if (selectedStructure == null)
        {
            // Clear
            resourceName.text = "";
           // m_CharNameLabel.text = "";
            //m_CharPortrait.style.backgroundImage = null;

            return;
        }

        // Fill in character details
        resourceName.text = selectedStructure.candy.ToString();
        //m_CharNameLabel.text = selectedStructure.juice.ToString();
       // m_CharPortrait.style.backgroundImage = new StyleBackground(selectedCharacter.m_PortraitImage);
    }
}