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
            // Skip hazard collision if player is grappling
            CM_Hookshot hookshot = GetComponent<CM_Hookshot>();
            if (hookshot != null && hookshot.isGrappling)
                return;

            gameObject.SetActive(false);
            onHitHazard.Invoke();
        }
        
        if(collision.gameObject.tag == "goal")
        {
            Debug.Log(collision.gameObject.tag + " hit");

            // Only deactivate if this is NOT the player (Gummies get collected, player walks through)
            if (GetComponent<CM_Hookshot>() == null)
            {
                gameObject.SetActive(false);
            }

            onHitGoal.Invoke();
        }

    }
}
