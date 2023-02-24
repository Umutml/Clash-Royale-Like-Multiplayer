using System;
using System.Collections;
using Photon.Bolt;
using UnityEngine;

public class CamPositioner : GlobalEventListener
{
    [SerializeField] private Transform cam;
    [SerializeField] private bool posSetted;
    [SerializeField] private int counter;

    [Header("Player1 Camera Properties")] [Space(10)]
    private readonly Vector3 _player1CameraPosition = new(0, 17f, -21f);

    private readonly Quaternion _player1CameraRotation = Quaternion.Euler(37.7f, 0, 0);

    [Header("Player2 Camera Properties")] [Space(10)]
    private readonly Vector3 _player2CameraPosition = new(0, 17f, 22.7f);

    private readonly Quaternion _player2CameraRotation = Quaternion.Euler(37.7f, 180f, 0);


    //public override void OnEvent(TimerStart count)
    // {
    //if (posSetted) return;

    // posSetted = true;
    //cam.transform.position = count.CameraPosition;
    // cam.transform.rotation = count.CameraRotation;
    //}

    
    private void Update()
    {
        SetCamPos();
    }

    private void SetCamPos()
    {
        if (posSetted) return;
        foreach (var session in BoltNetwork.SessionList)
        {
            counter = session.Value.ConnectionsCurrent;
            GameManager.Instance.pCount = counter;
        }
        //counter = 1;
        //foreach (var connection in BoltNetwork.Connections) counter++;

        if (counter == 1)
        {
            cam.transform.position = _player1CameraPosition;
            cam.transform.rotation = _player1CameraRotation;
        }
        else
        {
            cam.transform.position = _player2CameraPosition;
            cam.transform.rotation = _player2CameraRotation;
        }

        posSetted = true;
    }
}