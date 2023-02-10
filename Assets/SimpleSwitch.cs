using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleSwitch : MonoBehaviour
{
    public GameObject controlledObject;
    public Vector3 originalPosition;
    public float moveSpeed;

    public bool active;

    public GameObject[] waypoints;

    private Rigidbody rb;
    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
       originalPosition = controlledObject.transform.position;

        rb = controlledObject.GetComponent<Rigidbody>();


        if (rb == null)
        {
            Debug.LogError("You seem to be missing a rigid body on this object, sir/madam");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, waypoints[0].transform.position, moveSpeed * Time.deltaTime);
        }
        if (active == false)
        {
            
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, waypoints[1].transform.position, moveSpeed * Time.deltaTime);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        active = false;

    }

}
