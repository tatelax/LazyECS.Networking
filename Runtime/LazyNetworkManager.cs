﻿using Mirror;
using UnityEngine;

public class LazyNetworkManager : NetworkManager
{
    [SerializeField] private SimulationController simulationController;
    
    public delegate void ServerConnect(NetworkConnection connection);
    public delegate void ClientConnect(NetworkConnection connection);
    
    public event ServerConnect OnServerConnectedEvent; //A player joined
    public event ClientConnect OnClientConnectedEvent; //Client joined the server
        
    private WorldStateMessageSender worldStateMessageSender;
    
    public override void Awake()
    {
        base.Awake();

        WorldStateMessageReceiver.RegisterHandlers();
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
		
        simulationController.gameObject.SetActive(true);
        worldStateMessageSender = new WorldStateMessageSender();
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
		
        simulationController.gameObject.SetActive(true);
        worldStateMessageSender = new WorldStateMessageSender();

        OnClientConnectedEvent?.Invoke(conn);
    }

    /// <summary>
    /// A client connected to the server
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        
        if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
            return;
        
        worldStateMessageSender.BufferClient(conn);

        OnServerConnectedEvent?.Invoke(conn);
    }
}