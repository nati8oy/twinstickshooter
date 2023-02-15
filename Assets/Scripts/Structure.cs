using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName ="Structure")]

public class Structure : ScriptableObject
{
    [Header("Default")]
    public GameObject structureModel;
    public GameObject ghostModel;
    public Sprite shopThumnail;

    
    [Header("Resource Cost")]
    public Sprite candyIcon;
    public int candy;
    public Sprite juiceIcon;
    public int juice;
    public Sprite donutIcon;
    public int donut;
    public Sprite butterIcon;
    public int butter;


    public bool offensive;
    public int health;
    public int attackPower;
    public string buffType;

    [Header("Generator specific")]
    public bool generator;
    public GameObject resourceToGenerate;

    //the label for the tag of the object
    public string type;

    public void InstantiateObject(Vector3 spawnposition)
    {
        DestroyGhosts();
        //instantiate the object
        Instantiate(ghostModel);
        
    }

    public void DestroyGhosts()
    {
        //get a list of all the ghost objects and set them to false.
        //this just stops stray ghosts from being left in the scene
        var ghosts = GameObject.FindGameObjectsWithTag("ghost");
        foreach (GameObject ghost in ghosts)
        {
            Destroy(ghost);
            Debug.Log("ghosts killed");
        }
    }

}
