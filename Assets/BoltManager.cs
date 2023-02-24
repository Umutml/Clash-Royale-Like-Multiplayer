using System.Collections.Generic;
using System.Linq;
using Photon.Bolt;
using UnityEngine;
using Event = UnityEngine.Event;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class BoltManager : GlobalEventListener
{
    
    public int playerCount;


    public override void Connected(BoltConnection connection)
    {

        playerCount++;
        
        var evnt = TimerStart.Create();
        
        evnt.playerCounts = playerCount;
        
      //  if (playerCount == 1)
      //  {
      //      evnt.CameraPosition = _player1CameraPosition;
       //     evnt.CameraRotation = _player1CameraRotation;
      //      
      //  }
       // else
      //  {
       //     evnt.CameraPosition = _player2CameraPosition;
       //     evnt.CameraRotation = _player2CameraRotation;
       //     
        //}

        evnt.Send();
    }


}