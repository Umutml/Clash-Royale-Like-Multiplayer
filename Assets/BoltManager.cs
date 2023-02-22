using System.Linq;
using Photon.Bolt;
using UnityEngine;

public class BoltManager : GlobalEventListener
{
    public static BoltManager Instance;
    
    public Transform player1, player2;
    private Camera cam;

    [SerializeField] private GameObject hostPanel;
    
    private Transform camTransform;
    public int playerCount;

    //private int connectedClients;
    private void Awake()
    {
        cam = Camera.main;
        if (cam != null) camTransform = cam.GetComponent<Transform>();

        Instance = this;
        
        if (BoltNetwork.IsServer)
        {
            hostPanel.SetActive(true);
            //cam.gameObject.SetActive(false);
        }

        //CamBoltPosition();
    }

    //private void CamBoltPosition()
    //{
    //   if (BoltNetwork.IsServer)
    //    {

    //    }
    //     else
    //     {

    //     }
    //  }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        foreach(var session in BoltNetwork.SessionList)
        {
            playerCount = session.Value.ConnectionsCurrent;
            Debug.Log("who am i?  " + playerCount);

        }

        if (playerCount == 1)
        {
            var transform1 = player1.transform;
            camTransform.position = transform1.position;
            camTransform.rotation = transform1.rotation;
        }
        else
        {
            var transform2 = player2.transform;
            camTransform.position = transform2.position;
            camTransform.rotation = transform2.rotation;
        }
        
    } 
    
    
    public override void Connected(BoltConnection connection)
    {
        //if (BoltNetwork.IsServer) return;

        if (BoltNetwork.Connections.Count() == 1)
        {
            
            Debug.Log("1. client connected");
        }
        else if (BoltNetwork.Connections.Count() == 2)
        {
            
            Debug.Log("2. client connected");
        }
    }
}