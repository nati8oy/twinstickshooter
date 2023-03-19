using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StructureAssembly))]
public class StructureSetup : Editor
{
    public Mesh mesh;
    private GameObject meshObject;


    public override void OnInspectorGUI()
    {

        GUI.backgroundColor = Color.yellow;

        meshObject = (GameObject)EditorGUILayout.ObjectField("Mesh Object:", meshObject, typeof(GameObject), true);
        mesh = meshObject.GetComponent<Mesh>();

        DrawDefaultInspector();
        if (GUILayout.Button("Create New Structure"))
        {

            // Create the second child GameObject and add the MMF player
            GameObject structureObject = new GameObject("New Structure");
            structureObject.name = "New Structure";

            GameObject rotationTarget = new GameObject("RotationTarget");
            rotationTarget.name = "RotationTarget";



            // Add a MeshFilter component
            rotationTarget.AddComponent<MeshFilter>();
           MeshFilter meshFilter = meshObject.GetComponentInChildren<MeshFilter>();


            // Add a MeshRenderer component
            rotationTarget.AddComponent<MeshRenderer>();
            MeshRenderer meshRenderer = meshObject.GetComponentInChildren<MeshRenderer>();

            meshFilter.sharedMesh = mesh;


            // Set the parent of the transform target
            rotationTarget.transform.parent = structureObject.transform;

            //BoxCollider boxCollider = structureObject.AddComponent<BoxCollider>();


            // Get the BoxCollider component
            //boxCollider = structureObject.GetComponent<BoxCollider>();

            // Get the size of the mesh
            //Vector3 meshSize = rotationTarget.GetComponent<MeshRenderer>().bounds.size;

            // Set the size of the BoxCollider to match the mesh size
            //boxCollider.size = meshSize;

        }

    }
}