using UnityEngine;

public class Hook : MonoBehaviour
{
    public float distanceFromChainEnd = 2f;
   public void ConnectRopeEnd (Rigidbody endRB)
    {
       HingeJoint joint =  gameObject.AddComponent<HingeJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = endRB;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = new Vector3(0f, -distanceFromChainEnd, 0);
    }
}
