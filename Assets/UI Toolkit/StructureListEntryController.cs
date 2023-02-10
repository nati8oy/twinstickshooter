using UnityEngine.UIElements;

public class StructureListEntryController
{
    Label m_NameLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        m_NameLabel = visualElement.Q<Label>("structure-name");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetStructure(Structure structure)
    {
        m_NameLabel.text = structure.candy.ToString();
    }
}