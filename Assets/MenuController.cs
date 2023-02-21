using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using System;
public class MenuController : GlobalEventListener
{

    public void StartServer() // Host button 
    {
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone() 
    {
        BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "GameScene");
    }

    public void StartClient() // Client button
    {
        BoltLauncher.StartClient();

    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        base.SessionListUpdated(sessionList);

        foreach (var session in sessionList)
        {
            UdpSession photonsession = session.Value as UdpSession;

            if (photonsession.Source == UdpSessionSource.Photon)
                BoltMatchmaking.JoinSession(photonsession);
        }
    }

}