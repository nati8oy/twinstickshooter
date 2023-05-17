using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, iCollectible
{
    [SerializeField] Pickup pickup;
    public bool healing;
    public bool ammo;
    public bool disappearAfterTime = true;
    private float lifespan;


    private void OnEnable()
    {

        if (disappearAfterTime)
        {
            lifespan = pickup.lifespan;
            StartCoroutine(LifepspanTimer());
        }
      
    }


    private void OnCollisionEnter(Collision collision)
    {

        iCollectible collectible = collision.gameObject.GetComponent<iCollectible>();

        if (collectible != null)
        {
            collectible.Collect();
        }
    }

        public void Collect()
    {
        if (healing)
        {
            PlayerManager.Instance.Heal(pickup.strength);
        }

        gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator LifepspanTimer()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(lifespan);
        // Debug.Log("timer complete");
        gameObject.SetActive(false);

    }

    public void MaxAmmoCheck(int ammoToCheck, int ammoMax)
    {
        if (ammoToCheck > AmmoManager.maxAmmoSecondary)
        {
            ammoToCheck = AmmoManager.maxAmmoSecondary;
        }
    }

}
