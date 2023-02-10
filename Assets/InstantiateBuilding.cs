using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBuilding : MonoBehaviour
{

    public GameObject objectToInstantiate;

    public void InstantiateObject()
    {
        Instantiate(objectToInstantiate);
    }

}
