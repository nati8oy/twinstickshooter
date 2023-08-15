using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayFeedbacksGeneric : MonoBehaviour
{
    public UnityEvent onStart;
    public UnityEvent onHit;
    public UnityEvent onShoot;
    public UnityEvent onDisable;

    [Tooltip("Feedbacks that play when the object appears")]


    private void Awake()
    {
        onStart.Invoke();
    }

    [Tooltip("Feedbacks that play when this object is hit")]

    private void OnCollisionEnter(Collision collision)
    {
        onHit.Invoke();
        Debug.Log("Hit Somethin");
    }

    public void ShootFeedback()
    {
        onShoot.Invoke();
    }

    public void OnDisable()
    {
        onDisable.Invoke();
        Debug.Log("Disabled object");
    }

}
