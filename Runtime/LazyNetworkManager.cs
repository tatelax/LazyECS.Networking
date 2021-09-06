using System;
using Mirror;
using UnityEngine;

public class LazyNetworkManager : NetworkManager
{
    [SerializeField] private SimulationController simulationController;

    public Action<NetworkConnection> OnServerConnectedEvent;     // A client connected to us (the server)
    public Action<NetworkConnection> OnClientConnectedEvent;     // Client joined the server
    public Action<NetworkConnection> OnClientDisconnectEvent;    // Client disconnected from server
    public Action<Exception> OnClientErrorEvent;                 // Client experienced an exception
    public Action OnStartClientEvent;                            // Client started (doesnt mean connected)
    public Action OnStopClientEvent;                             // The client stopped
    public Action<NetworkConnection> OnServerDisconnectEvent;    // Client disconnected from server
    public Action OnServerStartEvent;                            // Server started
    public Action OnServerStopEvent;                             // Server stopped
    public Action<NetworkConnection> OnServerReadyEvent;         // A client sent the ready message to the server
    
    private WorldStateMessageSender worldStateMessageSender;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        WorldStateMessageReceiver.RegisterServerHandlers();
        simulationController.gameObject.SetActive(true);
        worldStateMessageSender = new WorldStateMessageSender();
        
        OnServerStartEvent?.Invoke();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        OnServerStopEvent?.Invoke();
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if (NetworkServer.active) // We're a host!
        {
            OnClientConnectedEvent?.Invoke(conn);
            return; 
        }
        
        simulationController.gameObject.SetActive(true);
        worldStateMessageSender = new WorldStateMessageSender();

        OnClientConnectedEvent?.Invoke(conn);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        WorldStateMessageReceiver.RegisterClientHandlers();
        OnStartClientEvent?.Invoke();
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();
        OnStopClientEvent?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnectEvent?.Invoke(conn);
    }

    public override void OnClientError(Exception exception)
    {
        base.OnClientError(exception);
        OnClientErrorEvent?.Invoke(exception);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        OnServerDisconnectEvent?.Invoke(conn);
    }

    /// <summary>
    /// A client connected to the server
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        if (conn.connectionId == 0 && NetworkServer.active) // We're a host!
        {
            OnServerConnectedEvent?.Invoke(conn);
            return;
        }
        
        worldStateMessageSender.BufferClient(conn);
        OnServerConnectedEvent?.Invoke(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        OnServerReadyEvent?.Invoke(conn);
    }
}
