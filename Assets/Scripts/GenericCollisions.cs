using UnityEngine;
using UnityEngine.Events;

public class GenericCollisions : MonoBehaviour
{
    public UnityEvent onHitGoal;
    public UnityEvent onHitHazard;
    
    
    public void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "hazard")
        {
            
            gameObject.SetActive(false);
            onHitHazard.Invoke();
        }
        
        if(collision.gameObject.tag == "goal")
        {
            Debug.Log(collision.gameObject.tag + " hit");
            gameObject.SetActive(false);
            onHitGoal.Invoke();
            
        }

    }
}
