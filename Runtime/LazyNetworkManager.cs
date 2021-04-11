using Mirror;
using UnityEngine;

public class LazyNetworkManager : NetworkManager
{
    [SerializeField] private SimulationController simulationController;

    private NetworkWorldBufferHandler networkWorldBufferHandler;
    
    public override void Awake()
    {
        base.Awake();

        NetworkWorldMessageHandler.RegisterHandlers();
        networkWorldBufferHandler = new NetworkWorldBufferHandler();
        
        if (simulationController == null)
        {
            Debug.LogError("Assign Simulation Controller in LazyNetworkManager!");
        }
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
		
        simulationController.gameObject.SetActive(true);
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
		
        simulationController.gameObject.SetActive(true);
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
        
        Debug.Log("BUFFERING!");
        
        networkWorldBufferHandler.OnClientConnected(conn);
    }
}
