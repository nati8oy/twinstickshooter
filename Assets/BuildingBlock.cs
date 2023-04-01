using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(ObjectChanger))]
public class BuildingBlock : Editor
{
    private string[] objectNames;
    private int selectedIndex = 0;

    private void OnEnable()
    {
        ObjectChanger objectChanger = (ObjectChanger)target;
        objectNames = new string[objectChanger.gameObjects.Length];

        for (int i = 0; i < objectNames.Length; i++)
        {
            objectNames[i] = objectChanger.gameObjects[i].name;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        selectedIndex = EditorGUILayout.Popup("Active Object", selectedIndex, objectNames);

        ObjectChanger objectChanger = (ObjectChanger)target;
        for (int i = 0; i < objectChanger.gameObjects.Length; i++)
        {
            objectChanger.gameObjects[i].SetActive(i == selectedIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
