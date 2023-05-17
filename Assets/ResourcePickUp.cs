using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePickUp : MonoBehaviour
{
    //[SerializeField] GameManager gameManager;
    private float lifespan;
    private string pickupType;
    private bool hasLifespan; 
            
    [SerializeField] Pickup pickup;


    private void OnEnable()
    {
        hasLifespan = pickup.hasLifespan;

       //check if there is a lifespan or not that is set in the scriptable object
        if (hasLifespan)
        {
            lifespan = pickup.lifespan;
            StartCoroutine(LifepspanTimer());
        }

        pickupType = pickup.type;
       

    }


    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.gameObject.tag == pickup.collidesWithTag.ToString())
        {
            if (pickupType == "ammo1" && (AmmoManager.ammoPrimary < AmmoManager.maxAmmoPrimary) && (AmmoManager.ammoPrimary != AmmoManager.maxAmmoPrimary))
            {
                AmmoManager.ammoPrimary += pickup.strength;
                MaxAmmoCheck(AmmoManager.ammoPrimary, AmmoManager.maxAmmoPrimary);
                gameObject.SetActive(false);

            }

            //check that the ammo is the correct type, and that the current amount is less than the max
            // also check if the ammo amount is at max. If it is, don't collect the pickup.
            if (pickupType == "ammo2" && (AmmoManager.ammoSecondary < AmmoManager.maxAmmoSecondary) &&(AmmoManager.ammoSecondary != AmmoManager.maxAmmoSecondary))
            {
                AmmoManager.ammoSecondary += pickup.strength;

                MaxAmmoCheck(AmmoManager.ammoSecondary, AmmoManager.maxAmmoSecondary);
                gameObject.SetActive(false);

            }

            if (pickupType == "health")
            {
                PlayerManager.Instance.Heal(pickup.strength);
                gameObject.SetActive(false);

            }

            if (pickupType == "XP")
            {
                //PlayerManager.Instance.Heal(pickup.strength);
                gameObject.SetActive(false);

            }

            if (pickupType == "resource")
            {
                //PlayerManager.Instance.Heal(pickup.strength);
                gameObject.SetActive(false);

            }

            if (pickupType == "goal")
            {
                //PlayerManager.Instance.Heal(pickup.strength);
                gameObject.SetActive(false);
                Debug.Log("collected via resource script");
            }
        }
    }
  

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Remove()
    {
        gameObject.SetActive(false);

    }

    IEnumerator LifepspanTimer()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(lifespan);
       // Debug.Log("timer complete");
        Remove();
    }

    public void MaxAmmoCheck(int ammoToCheck, int ammoMax)
    {
        if (ammoToCheck > AmmoManager.maxAmmoSecondary)
        {
            ammoToCheck = AmmoManager.maxAmmoSecondary;
        }
    }
}
