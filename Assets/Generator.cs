using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public float rateOfSpawn = 5f;
    public Transform generationPoint;
    public Structure structure;
    private Rigidbody resourceRB;

    // Start is called before the first frame update
    private void OnEnable()
    {
        InvokeRepeating(nameof(Generate), rateOfSpawn, rateOfSpawn);

    }
    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Generate()
    {
        var force = new Vector3(50, 50, 0);
        var resource = structure.resourceToGenerate;

        Instantiate(resource, generationPoint.transform.position, Quaternion.identity);
        resourceRB = resource.GetComponent<Rigidbody>();
        resourceRB.AddForce(force, ForceMode.Impulse);


    }
}
