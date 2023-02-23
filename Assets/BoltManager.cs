using System.Linq;
using Photon.Bolt;
using UnityEngine;
using Event = UnityEngine.Event;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class BoltManager : GlobalEventListener
{
   
    [Header("Player1 Camera Properties")]
    [Space(10)]
    private Vector3 _player1CameraPosition = new Vector3(0, 17f, -21f);
    private Quaternion _player1CameraRotation = Quaternion.Euler(37.7f, 0, 0);

    [Header("Player2 Camera Properties")]
    [Space(10)]
    private Vector3 _player2CameraPosition = new Vector3(0, 17f, 22.7f);
    private Quaternion _player2CameraRotation = Quaternion.Euler(37f, 180f, 0);

    
    
    //[SerializeField] private Transform player1, player2;
    public int playerCount = 0;


    public override void Connected(BoltConnection connection)
    {

        playerCount++;
        
        
        var evnt = TimerStart.Create();

        evnt.playerCounts = playerCount;
        
        if (playerCount == 1)
        {
            evnt.CameraPosition = _player1CameraPosition;
            evnt.CameraRotation = _player1CameraRotation;
        }

        else
        {
            evnt.CameraPosition = _player2CameraPosition;
            evnt.CameraRotation = _player2CameraRotation;
        }

        evnt.Send();
    }
    
    
    
    //public override void Connected(BoltConnection connection)
    //{
        //if (BoltNetwork.IsServer) return;

       // if (BoltNetwork.Connections.Count() == 1)
      //  {
            //playerCount = 1;
      //  }
       // else if (BoltNetwork.Connections.Count() == 2)
      //  {
            //playerCount = 2;
     //   }
   // }
}