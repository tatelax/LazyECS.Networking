using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

public class NetworkWorld : World
{
	public override void Init()
	{
		base.Init();
		NetworkClient.RegisterHandler<CreateEntityMessage>(CreateEntityMessageReceived);
		NetworkClient.RegisterHandler<ComponentAddedMessage>(ComponentAddedMessageRecieved);
		NetworkClient.RegisterHandler<DestroyEntityMessage>(DestroyEntityMessageReceived);
	}



	/*
	 *
 	* 
 	*       NETWORK EVENTS
 	*
 	* 
 	*/
	
	
	private void ComponentAddedMessageRecieved(NetworkConnection conn, ComponentAddedMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host
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

	private void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host
			return;

		CreateEntity(msg.id);
	}
	
	private void DestroyEntityMessageReceived(NetworkConnection conn, DestroyEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host
			return;

		DestroyEntity(msg.id);
	}

	
	/*
	 *
	 * 
	 *       ENTITY EVENTS
	 *
	 * 
	 */
	
	public override void OnEntityCreated(Entity entity)
	{
		base.OnEntityCreated(entity);

		if (!NetworkServer.active) return;

		CreateEntityMessage msg = new CreateEntityMessage {id = entity.id};
		NetworkServer.SendToAll(msg);
	}

	public override void OnComponentAddedToEntity(Entity entity, IComponent component)
	{
		base.OnComponentAddedToEntity(entity, component);

		if (!NetworkServer.active) return;

		ComponentAddedMessage msg = new ComponentAddedMessage
		{
			entityId = entity.id,
			componentId = 0
		};
		
		NetworkServer.SendToAll(msg);
		
		Debug.Log("A component was added");
	}

	public override void OnComponentRemovedFromEntity(Entity entity, IComponent component)
	{
		base.OnComponentRemovedFromEntity(entity, component);

		if (!NetworkServer.active) return;

		Debug.Log("A component was removed");
	}

	public override void OnComponentSetOnEntity(Entity entity, IComponent component)
	{
		base.OnComponentSetOnEntity(entity, component);

		if (!NetworkServer.active) return;
		if (!(component is INetworkComponent)) return;


		//TODO: This is firing on the host and sending to itself
		INetworkComponent networkComponent = (INetworkComponent) component;
		networkComponent.SendMessage(true);
		
		Debug.Log("A component was set");
	}

}