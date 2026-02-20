using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : PersistentSingleton<DebugManager>
{
    public bool godMode = false;
    public bool infiniteAmmo = true;
    public bool autoTargetEnabled = false;
    public bool fuzzyTargetingEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GodMode()
    {

    }
}
