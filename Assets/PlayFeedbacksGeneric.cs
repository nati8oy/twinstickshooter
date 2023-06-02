using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayFeedbacksGeneric : MonoBehaviour
{
    public UnityEvent onStart;
    public UnityEvent onHit;
    public UnityEvent onShoot;

    [Tooltip("Feedbacks that play when the object appears")]


    private void Awake()
    {
        onStart.Invoke();
    }

    [Tooltip("Feedbacks that play when this object is hit")]

    private void OnCollisionEnter(Collision collision)
    {
        onHit.Invoke();
    }

    public void ShootFeedback()
    {
        onShoot.Invoke();
    }


}
