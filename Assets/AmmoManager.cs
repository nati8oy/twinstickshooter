using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AmmoManager : Singleton<AmmoManager>
{
    public static int maxAmmoPrimary = 100;
    public static int maxAmmoSecondary = 10;

    public static int ammoPrimary = 100;
    public static int ammoSecondary = 10;

    [SerializeField] TextMeshProUGUI ammo1;
    [SerializeField] TextMeshProUGUI ammo2;

    private void OnEnable()
    {
        ammoPrimary = 100;
        ammoSecondary = 10;

    }

    // Start is called before the first frame update
    void Start()
    {
        ammo1.text = ammoPrimary.ToString() + "/" + maxAmmoPrimary.ToString();
        ammo2.text = ammoSecondary.ToString() + "/" + maxAmmoSecondary.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ammo1.text = ammoPrimary.ToString() + "/" + maxAmmoPrimary.ToString();
        ammo2.text = ammoSecondary.ToString() + "/" + maxAmmoSecondary.ToString();
    }
}
