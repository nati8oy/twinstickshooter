using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Attack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Dash(Transform dashTarget)
    {
        /*
        if (gameObject.GetComponent<EnemyNavMesh>())
        {
            gameObject.GetComponent<EnemyNavMesh>().enabled =false;
        }

        transform.DOMove(dashTarget.position, 0.5f).SetEase(Ease.InOutCirc);
        */

        Debug.Log("Dash!");
    }

    public void ShootPlayer()
    {
        if (GetComponent<Shooting>() != null)
        {
            GetComponent<Shooting>().Shoot();
        }
    }
}
