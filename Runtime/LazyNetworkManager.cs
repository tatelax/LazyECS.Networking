using Mirror;
using UnityEngine;

public class LazyNetworkManager : NetworkManager
{
    [SerializeField] private SimulationController simulationController;
    
    public delegate void ServerConnect(NetworkConnection connection);
    public delegate void ClientConnect(NetworkConnection connection);
    public delegate void ClientDisconnect(NetworkConnection connection);
    public delegate void StopClient();
    public delegate void ServerDisconnect(NetworkConnection connection);
    public delegate void ServerStart();
    public delegate void ServerStop();
    public delegate void ServerReady(NetworkConnection connection);
    
    public event ServerConnect OnServerConnectedEvent; //A player joined
    public event ClientConnect OnClientConnectedEvent; //Client joined the server
    public event ClientDisconnect OnClientDisconnectEvent; // Client disconnected from server
    public event StopClient OnStopClientEvent; // The client stopped
    public event ServerDisconnect OnServerDisconnectEvent; // Client disconnected from server
    public event ServerStart OnServerStartEvent; // Server started
    public event ServerStop OnServerStopEvent; // Server stopped
    public event ServerReady OnServerReadyEvent; // A client sent the ready message to the server
        
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
