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
		NetworkClient.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkClient.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageReceived);
		NetworkClient.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageReceived);
		
		NetworkClient.RegisterHandler<StringComponentMessage>(StringComponentMessageReceived);
		
		NetworkServer.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkServer.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkServer.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageReceived);
		NetworkServer.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageReceived);
		
		NetworkServer.RegisterHandler<StringComponentMessage>(StringComponentMessageReceived);
	}


	private static void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		Debug.Log($"Create entity message received id: {msg.id}");
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);
		
		if (!NetworkServer.active && world.Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to create an entity, which it did and send that msg to all clients including us! 
		world.CreateEntity(msg.id, true);
	}
	
	private static void DestroyEntityMessageReceived(NetworkConnection conn, DestroyEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		IWorld world = SimulationController.Instance.GetWorld(msg.worldId);	
		
		if (!NetworkServer.active && !world.Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to destroy an entity, which it did and send that msg to all clients including us! 
		world.DestroyEntity(msg.id, true);
	}
	
	private static void ComponentAddedMessageReceived(NetworkConnection conn, ComponentAddedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

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
	
	private static void StringComponentMessageReceived(NetworkConnection conn, StringComponentMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;
		
		IWorld world = SimulationController.Instance.GetWorld(msg.worldID);
		
		world.Entities[msg.entityID].Set(msg.componentID, msg.Value, true);
	}
}