using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

public class NetworkWorld : World
{
	private int WorldId;

	public override void Start()
	{
		WorldId = SimulationController.Instance.GetWorldId(this);

		base.Start();
	}
	
	public override void OnEntityCreated(Entity entity, bool entityCreatedFromNetworkMessage)
	{
		base.OnEntityCreated(entity, entityCreatedFromNetworkMessage); // Never skip this because this is how groups get updates
		
		if (entityCreatedFromNetworkMessage && NetworkClient.active && !NetworkServer.active) return; // We're a client and the server said to create an entity. We don't send a message. We just do what we are told!

		CreateEntityMessage msg = new CreateEntityMessage {worldId = WorldId, id = entity.id};
		
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
		
		DestroyEntityMessage msg = new DestroyEntityMessage{worldId = WorldId, id = entity.id};
		
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
			worldId = WorldId,
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
			worldId = WorldId,
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