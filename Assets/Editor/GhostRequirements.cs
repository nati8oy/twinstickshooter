using UnityEngine;
using System.Collections;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(GhostAssembly))]
public class GhostRequirements : Editor
{
    [Tooltip("Adds a scriptable object with the name of the top layer game object in this hierarchy")]
    private bool addScriptableobject = true;

    public string layerName = "Ground";


    public override void OnInspectorGUI()
    {
        //adds the GUI toggle element
        addScriptableobject = EditorGUILayout.Toggle("Add scriptable object", addScriptableobject);

        GUI.backgroundColor = Color.green;

        DrawDefaultInspector();
        if (GUILayout.Button("Create New Structure"))
        {
            GameObject targetGameObject = ((GhostAssembly)target).gameObject;

            //create a new scriptable object that has the same name as the top level object
            Structure newStructure = CreateInstance<Structure>();
            AssetDatabase.CreateAsset(newStructure, "Assets/Prefabs/Data Objects/Structures/" + targetGameObject.name + ".asset");
            AssetDatabase.SaveAssets();

            //Add box collider
            //BoxCollider boxCollider = targetGameObject.AddComponent<BoxCollider>();
            //boxCollider.isTrigger = true;

            // Add ObjectTransform component
            ObjectTransform objectTransform = targetGameObject.AddComponent<ObjectTransform>();

            // Add SimpleMovement component
            SimpleMovement simpleMovement = targetGameObject.AddComponent<SimpleMovement>();

            // Add PlaceOnMap component
            PlaceOnMap placeOnMap = targetGameObject.AddComponent<PlaceOnMap>();
            //set the structure object in the newly created place on map script
            placeOnMap.structure = newStructure;

            //set the layermask to be Ground
            int layerMaskGroundIndex = 3;
            placeOnMap.layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerMaskGroundIndex));


            //set the tag as ghost
            targetGameObject.tag = "ghost";


            // Create the second child GameObject and add the MMF player
            GameObject feedbacksGameObject = new GameObject("Feedbacks");
            feedbacksGameObject.AddComponent<MMF_Player>();
            feedbacksGameObject.name = "Feedbacks";

            // Set the parent of the child GameObject
            feedbacksGameObject.transform.parent = targetGameObject.transform;

        }
    }
}
