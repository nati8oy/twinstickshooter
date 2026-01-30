using UnityEngine;
using UnityEngine.Events;

public class GenericCollisions : MonoBehaviour
{
 
   
    public enum CollisionType { enemy, goal }
    public CollisionType collisionType;
    
    public UnityEvent onHit;
    
    public void OnCollisionEnter(Collision collision)
    {
        
        
        if(collision.gameObject.tag == CollisionType.enemy.ToString())
        {
            
            Debug.Log("enemy hit");
            gameObject.SetActive(false);
            onHit.Invoke();
        }
        
        if(collision.gameObject.tag == CollisionType.goal.ToString())
        {
            Debug.Log("Goal Hit");
            gameObject.SetActive(false);
            onHit.Invoke();
            
        }

    }
}
