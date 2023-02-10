using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAmount(int amount)
    {
        slider.value = amount;
    }

    public void SetMaxBarAmount(int amount)
    {
        slider.maxValue = amount;
        slider.value = amount;
    }

}
