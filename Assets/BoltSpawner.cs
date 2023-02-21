using UnityEngine;
using Photon.Bolt;

using System;
using Random = UnityEngine.Random;

public class BoltSpawner : GlobalEventListener
{

    public GameObject knight;


    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        //base.SceneLoadLocalDone(scene, token);
        var randomX = Random.Range(-5, 5);
        Vector3 pos = new Vector3(randomX, 0, 0);
        knight.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        BoltNetwork.Instantiate(knight, pos, Quaternion.identity);

    }

}