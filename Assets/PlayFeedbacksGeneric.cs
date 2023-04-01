using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayFeedbacksGeneric : MonoBehaviour
{
    public UnityEvent onStart;

    private void Awake()
    {
        onStart.Invoke();
    }

}
