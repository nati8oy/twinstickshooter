using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Grappleable Object")]

public class GrappleableObject : ScriptableObject
{
    [SerializeField] private bool breakable;
    [SerializeField] private float mass;
    [SerializeField] private float drag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
