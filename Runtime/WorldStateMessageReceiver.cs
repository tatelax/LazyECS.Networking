using LazyECS;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

// Disable warning about obsolete RegisterHandler
#pragma warning disable CS0618
public static class WorldStateMessageReceiver
{
	public static void RegisterHandlers()
	{
		NetworkClient.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkServer.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);

		NetworkClient.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkServer.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);

		NetworkClient.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageReceived);
		NetworkServer.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageReceived);

		NetworkClient.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageReceived);
		NetworkServer.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageReceived);
		
		
		NetworkClient.RegisterHandler<StringComponentMessage>(StringComponentMessageReceived);
		NetworkServer.RegisterHandler<StringComponentMessage>(StringComponentMessageReceived);
		
		NetworkClient.RegisterHandler<FloatComponentMessage>(FloatComponentMessageReceived);
		NetworkServer.RegisterHandler<FloatComponentMessage>(FloatComponentMessageReceived);
		
		NetworkClient.RegisterHandler<IntComponentMessage>(IntComponentMessageReceived);
		NetworkServer.RegisterHandler<IntComponentMessage>(IntComponentMessageReceived);
		
		NetworkClient.RegisterHandler<BoolComponentMessage>(BoolComponentMessageReceived);
		NetworkServer.RegisterHandler<BoolComponentMessage>(BoolComponentMessageReceived);
		
		NetworkClient.RegisterHandler<Vector3ComponentMessage>(Vector3ComponentMessageReceived);
		NetworkServer.RegisterHandler<Vector3ComponentMessage>(Vector3ComponentMessageReceived);
	}

	
/*
 *
 *
 *    ENTITY CHANGES
 * 
 * 
 */
	
	
	private static void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if(SimulationController.Instance.LogLevel == LogLevel.Verbose)
			Debug.Log($"<color=#00ffff>[LazyECS Networking] Create entity message received with entity id {msg.id} in world {msg.worldId}</color>");
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);
		
		if (!NetworkServer.active && world.Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to create an entity, which it did and send that msg to all clients including us! 
		world.CreateEntity(msg.id, true);
	}
	
	private static void DestroyEntityMessageReceived(NetworkConnection conn, DestroyEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if(SimulationController.Instance.LogLevel == LogLevel.Verbose)
			Debug.Log($"<color=#00ffff>[LazyECS Networking] Destroy entity message received with entity id {msg.id} in world {msg.worldId}</color>");
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);	
		
		if (!NetworkServer.active && !world.Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to destroy an entity, which it did and send that msg to all clients including us! 
		world.DestroyEntity(msg.id, true);
	}
	
	private static void ComponentAddedMessageReceived(NetworkConnection conn, ComponentAddedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if(SimulationController.Instance.LogLevel == LogLevel.Verbose)
			Debug.Log($"<color=#00ffff>[LazyECS Networking] Component <b>ADDED</b> message received with entity id  {msg.entityId} and type {ComponentLookup.Get(msg.componentId)} in world {msg.worldId}</color>");
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);
		
		if (!world.Entities.ContainsKey(msg.entityId))
		{
			Debug.LogError("Tried to add a component to an entity that didnt exist!");
			return;
		}
		
		Entity entity = world.Entities[msg.entityId];

		if (!entity.Has(msg.componentId))
		{
			entity.Add(msg.componentId);
		}
	}
	
	private static void ComponentRemovedMessageReceived(NetworkConnection conn, ComponentRemovedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if(SimulationController.Instance.LogLevel == LogLevel.Verbose)
			Debug.Log($"<color=#00ffff>[LazyECS Networking] Component <b>REMOVED</b> message received with entity id  {msg.entityId} and  and type {ComponentLookup.Get(msg.componentId)} in world {msg.worldId}</color>");
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);
		
		if (!world.Entities.ContainsKey(msg.entityId))
		{
			Debug.LogError("Tried to remove a component to from entity that didnt exist!");
			return;
		}
		
		Entity entity = world.Entities[msg.entityId];
		
		if(entity.Has(msg.componentId))
			entity.Remove(msg.componentId);
	}
	
/*
 *
 *
 *    COMPONENT VALUE CHANGES
 * 
 * 
 */

	private static void StringComponentMessageReceived(NetworkConnection conn, StringComponentMessage msg)
	{
		SetComponentFromNetworkMessage(conn.connectionId, msg.worldID, msg.entityID, msg.componentID, msg.Value);
	}
	
	private static void Vector3ComponentMessageReceived(NetworkConnection conn, Vector3ComponentMessage msg)
	{
		SetComponentFromNetworkMessage(conn.connectionId, msg.worldID, msg.entityID, msg.componentID, msg.Value);
	}

	private static void BoolComponentMessageReceived(NetworkConnection conn, BoolComponentMessage msg)
	{
		SetComponentFromNetworkMessage(conn.connectionId, msg.worldID, msg.entityID, msg.componentID, msg.Value);
	}

	private static void IntComponentMessageReceived(NetworkConnection conn, IntComponentMessage msg)
	{
		SetComponentFromNetworkMessage(conn.connectionId, msg.worldID, msg.entityID, msg.componentID, msg.Value);
	}

	private static void FloatComponentMessageReceived(NetworkConnection conn, FloatComponentMessage msg)
	{
		SetComponentFromNetworkMessage(conn.connectionId, msg.worldID, msg.entityID, msg.componentID, msg.Value);
	}

	private static void SetComponentFromNetworkMessage(int connectionId, int worldId, int entityId, int componentId, object value)
	{
		if (connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;
		
		if(SimulationController.Instance.LogLevel == LogLevel.Verbose)
			Debug.Log($"<color=#00ffff>[LazyECS Networking] Component set message received with entity id  {entityId} and type {ComponentLookup.Get(componentId)} in world {worldId} with type of {value.GetType().Name}</color>");
		
		IWorld world = SimulationController.Instance.GetWorld(worldId);
		
		world.Entities[entityId].Set(componentId, value, true);
	}
}