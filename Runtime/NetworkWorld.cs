using System.Net;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

public class NetworkWorld : World
{
	protected NetworkWorld()
	{
		//TODO: This is going to overwrite the handlers for all other worlds!
		NetworkClient.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkClient.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkClient.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageRecieved);
		NetworkClient.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageRecieved);
		
		NetworkServer.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkServer.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
		NetworkServer.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageRecieved);
		NetworkServer.RegisterHandler<ComponentRemovedMessage>(ComponentRemovedMessageRecieved);
	}
	
	/*
	 *
 	* 
 	*       NETWORK EVENTS
 	*
 	* 
 	*/
	
	
	private void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if (!NetworkServer.active && Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to create an entity, which it did and send that msg to all clients including us! 
		CreateEntity(msg.id, true);
	}
	
	private void DestroyEntityMessageReceived(NetworkConnection conn, DestroyEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if (!NetworkServer.active && !Entities.ContainsKey(msg.id)) return; // We're a client and we told the server to destroy an entity, which it did and send that msg to all clients including us! 
		DestroyEntity(msg.id, true);
	}
	
	private void ComponentAddedMessageRecieved(NetworkConnection conn, ComponentAddedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;

		if (!Entities.ContainsKey(msg.entityId))
		{
			Debug.LogError("Tried to add a component to an entity that didnt exist!");
			return;
		}
		
		Entity entity = Entities[msg.entityId];

		if (!entity.Has(msg.componentId))
		{
			Debug.Log($"cool {msg.entityId} {msg.componentId}");
			entity.Add(msg.componentId);
		}
	}
	
	private void ComponentRemovedMessageRecieved(NetworkConnection conn, ComponentRemovedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host might not work for dedicated servers
			return;
		
		if (!Entities.ContainsKey(msg.entityId))
		{
			Debug.LogError("Tried to remove a component to from entity that didnt exist!");
			return;
		}
		
		Entity entity = Entities[msg.entityId];
		
		if(entity.Has(msg.componentId))
			entity.Remove(msg.componentId);
		
		Debug.Log("Received a message to add component " + msg.componentId + " to " + msg.entityId);
	}

	
	/*
	 *
	 * 
	 *       ENTITY EVENTS
	 *
	 * 
	 */
	
	
	public override void OnEntityCreated(Entity entity, bool entityCreatedFromNetworkMessage)
	{
		base.OnEntityCreated(entity, entityCreatedFromNetworkMessage); // Never skip this because this is how groups get updates
		
		if (entityCreatedFromNetworkMessage && NetworkClient.active && !NetworkServer.active) return; // We're a client and the server said to create an entity. We don't send a message. We just do what we are told!

		CreateEntityMessage msg = new CreateEntityMessage {id = entity.id};
		
		if(NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
		{
			NetworkClient.Send(msg);
		}
	}

	public override void OnEntityDestroyed(Entity entity, bool entityDestroyedFromNetworkMessage = false)
	{
		base.OnEntityDestroyed(entity, entityDestroyedFromNetworkMessage);
		
		if (entityDestroyedFromNetworkMessage && NetworkClient.active && !NetworkServer.active) return; // We're a client and the server said to destroy an entity. We don't send a message. We just do what we are told!
		
		DestroyEntityMessage msg = new DestroyEntityMessage{id = entity.id};
		
		if(NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);
	}

	public override void OnComponentAddedToEntity(Entity entity, IComponent component)
	{
		base.OnComponentAddedToEntity(entity, component);

		if (!(component is INetworkComponent)) return;

		ComponentAddedMessage msg = new ComponentAddedMessage
		{
			entityId = entity.id,
			componentId = ComponentLookup.Get(component.GetType())
		};
		
		if(NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else 
			NetworkClient.Send(msg);
		
		Debug.Log("A component was added");
	}

	public override void OnComponentRemovedFromEntity(Entity entity, IComponent component)
	{
		base.OnComponentRemovedFromEntity(entity, component);

		if (!(component is INetworkComponent)) return;

		ComponentRemovedMessage msg = new ComponentRemovedMessage
		{
			entityId = entity.id,
			componentId = ComponentLookup.Get(component.GetType())
		};
		
		if(NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);

		Debug.Log($"{component.GetType()} component was removed");
	}

	public override void OnComponentSetOnEntity(Entity entity, IComponent component, bool setFromNetworkMessage = false)
	{
		base.OnComponentSetOnEntity(entity, component);

		if (!(component is INetworkComponent)) return;

		INetworkComponent networkComponent = (INetworkComponent) component;
	   
		networkComponent.SendMessage(0, entity.id, NetworkServer.active, setFromNetworkMessage);
	}
}