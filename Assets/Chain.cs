
using UnityEngine;

public class Chain : MonoBehaviour
{

    [SerializeField] private Rigidbody topLink;
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private int links = 7;

    [SerializeField] private Hook hook;
    // Start is called before the first frame update
    void Start()
    {
        GenerateChain();
    }

    private void GenerateChain()
    {
        Rigidbody previousRB = topLink;

        for (int i = 0; i < links; i++)
        {
            GameObject link = Instantiate(linkPrefab, transform);
            HingeJoint joint = link.GetComponent<HingeJoint>();
            joint.connectedBody = previousRB;



            if (i < links - 1)
            {
                previousRB = link.GetComponent<Rigidbody>();
            }
            else
            {
                hook.ConnectRopeEnd(link.GetComponent<Rigidbody>());
            }
        }
    }
}