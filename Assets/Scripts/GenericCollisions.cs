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
            //onHit.Invoke();
            Debug.Log("enemy hit");
        }
        
        if(collision.gameObject.tag == CollisionType.goal.ToString())
        {
            Debug.Log("Goal Hit");
        }

    }
}
