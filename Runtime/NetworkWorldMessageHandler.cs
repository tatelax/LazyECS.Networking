using LazyECS;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

public static class NetworkWorldMessageHandler
{
	public static void RegisterHandlers()
	{
		NetworkClient.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkClient.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkClient.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageRecieved);
		NetworkClient.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageRecieved);
		
		NetworkServer.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkServer.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkServer.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageRecieved);
		NetworkServer.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageRecieved);
	}

	private static void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

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
	
	private static void ComponentAddedMessageRecieved(NetworkConnection conn, ComponentAddedMessage msg)
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
	
	private static void ComponentRemovedMessageRecieved(NetworkConnection conn, ComponentRemovedMessage msg)
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
}