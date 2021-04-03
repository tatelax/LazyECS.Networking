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
	}

	private void CreateEntityMessageReceived(NetworkConnection conn, CreateEntityMessage msg)
	{
		if (conn.connectionId == 0 && NetworkServer.active) // Check if we are the host
			return;

		CreateEntity(msg.id);
	}

	public override void OnEntityCreated(Entity entity)
	{
		if (!NetworkServer.active) return;
		
		base.OnEntityCreated(entity);

		CreateEntityMessage msg = new CreateEntityMessage {id = entity.id};
		NetworkServer.SendToAll(msg);
	}

	public override void OnComponentAddedToEntity(Entity entity, IComponent component)
	{
		if (!NetworkServer.active) return;

		base.OnComponentAddedToEntity(entity, component);
		Debug.Log("A component was added");
	}

	public override void OnComponentRemovedFromEntity(Entity entity, IComponent component)
	{
		if (!NetworkServer.active) return;

		base.OnComponentRemovedFromEntity(entity, component);
		Debug.Log("A component was removed");
	}

	public override void OnComponentSetOnEntity(Entity entity, IComponent component)
	{
		if (!NetworkServer.active) return;

		base.OnComponentSetOnEntity(entity, component);
		Debug.Log("A component was set");
	}

}