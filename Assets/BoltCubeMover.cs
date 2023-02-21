using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;


public class BoltCubeMover : EntityBehaviour<INewCubeState>
{

    public override void Attached()
    {
        state.SetTransforms(state.CubeTransform, transform);
    }

    public override void SimulateOwner()
    {
        var speed = 4f;

        float axisH = Input.GetAxis("Horizontal");
        float axisV = Input.GetAxis("Vertical");

        var direction = new Vector3(axisH, 0, axisV);
        
        if(axisH != 0 || axisV != 0) 
        {
            transform.Translate(-direction * speed * BoltNetwork.FrameDeltaTime);
            //transform.forward = direction;
        }
    }


}