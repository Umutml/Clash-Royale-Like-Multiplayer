using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class CamPositioner : GlobalEventListener
{
    private Camera cam;
    private bool posSetted;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    
    public override void OnEvent(TimerStart count)
    {
        if (posSetted) return;

        posSetted = true;
        cam.transform.position = count.CameraPosition;
        cam.transform.rotation = count.CameraRotation;
    }
}
