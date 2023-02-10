using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : MonoBehaviour
{

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();


        if (rb == null)
        {
            Debug.LogError("You seem to be missing a rigid body on this object, sir/madam");
        }

        rb.WakeUp();
    }
}
