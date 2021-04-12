﻿using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;
using UnityEngine;

public class NetworkWorldStateHandler
{
	public NetworkWorldStateHandler()
	{
		SimulationController.Instance.OnWorldsInitialized += InstanceOnOnWorldsInitialized;
	}

	private void InstanceOnOnWorldsInitialized(object sender, EventArgs e)
	{
		foreach (KeyValuePair<int,IWorld> world in SimulationController.Instance.Worlds)
		{
			world.Value.OnEntityCreatedEvent += (entity, message) => { OnEntityCreated(world.Key, entity, message); };
			world.Value.OnEntityDestroyedEvent += (entity, message) => { OnEntityDestroyed(world.Key, entity, message); };
			world.Value.OnComponentAddedToEntityEvent += (entity, component) => { OnComponentAddedToEntity(world.Key, entity, component); };
			world.Value.OnComponentRemovedFromEntityEvent += (entity, component) => { OnComponentRemovedFromEntity(world.Key, entity, component); };
			world.Value.OnComponentSetOnEntityEvent += (entity, component, message) => { OnComponentSetOnEntity(world.Key, entity, component, message); };
		}
	}

	private void OnEntityCreated(int worldId, Entity entity, bool entityCreatedFromNetworkMessage)
	{
		if (entityCreatedFromNetworkMessage && NetworkClient.active && !NetworkServer.active)
			return; // We're a client and the server said to create an entity. We don't send a message. We just do what we are told!

		CreateEntityMessage msg = new CreateEntityMessage {worldId = worldId, id = entity.id};

		if (NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
		{
			NetworkClient.Send(msg);
		}
	}

	private void OnEntityDestroyed(int worldId, Entity entity, bool entityDestroyedFromNetworkMessage = false)
	{
		if (entityDestroyedFromNetworkMessage && NetworkClient.active && !NetworkServer.active)
			return; // We're a client and the server said to destroy an entity. We don't send a message. We just do what we are told!

		DestroyEntityMessage msg = new DestroyEntityMessage {worldId = worldId, id = entity.id};

		if (NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);
	}

	private void OnComponentAddedToEntity(int worldId, Entity entity, IComponent component)
	{
		if (!(component is INetworkComponent)) return;

		ComponentAddedMessage msg = new ComponentAddedMessage
		{
			worldId = worldId,
			entityId = entity.id,
			componentId = ComponentLookup.Get(component.GetType())
		};

		if (NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);

		Debug.Log("A component was added");
	}

	private void OnComponentRemovedFromEntity(int worldId, Entity entity, IComponent component)
	{
		if (!(component is INetworkComponent)) return;

		ComponentRemovedMessage msg = new ComponentRemovedMessage
		{
			worldId = worldId,
			entityId = entity.id,
			componentId = ComponentLookup.Get(component.GetType())
		};

		if (NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);

		Debug.Log($"{component.GetType()} component was removed");
	}

	private void OnComponentSetOnEntity(int worldId, Entity entity, IComponent component, bool setFromNetworkMessage = false)
	{
		if (!(component is INetworkComponent)) return;

		INetworkComponent networkComponent = (INetworkComponent) component;

		switch (networkComponent.Get().GetType().Name)
		{
			case "String":
				StringComponentMessage msg = new StringComponentMessage
				{
					worldID = worldId,
					entityID = entity.id,
					Value = (string) networkComponent.Get()
				};


				break;
		}
	}

	/// <summary>
	/// When a client joins, we need to give them all of the entities, components, and component values so they are in sync with the server
	/// </summary>
	/// <param name="conn">The NetworkConnection of the player</param>
	public void BufferClient(NetworkConnection conn)
	{
		// Loop through all worlds
		foreach (KeyValuePair<int, IWorld> world in SimulationController.Instance.Worlds)
		{
			// Loop through all the entities in that network world
			//TODO: What happens if an entity is created while the player is joining?
			foreach (KeyValuePair<int, Entity> entity in world.Value.Entities.ToList())
			{
				// Send a message to the client that connected telling them to create an entity in the same world with the same id
				CreateEntityMessage createEntityMessage = new CreateEntityMessage {worldId = world.Key, id = entity.Value.id};
				conn.Send(createEntityMessage);

				// Loop through all components on the entity
				//TODO: What happens if a component is changed while the player is joining?
				foreach (KeyValuePair<Type, IComponent> component in entity.Value.Components.ToList())
				{
					// We only care about network components
					if (!(component.Value is INetworkComponent)) continue;

					// Send a message to the client to add the component
					ComponentAddedMessage addComponentMessage = new ComponentAddedMessage
					{
						worldId = world.Key,
						entityId = entity.Value.id,
						componentId = ComponentLookup.Get(component.Value.GetType())
					};
					conn.Send(addComponentMessage);

					// Send a message to the client to set the component's value
					// INetworkComponent networkComponent = (INetworkComponent) component.Value;
					// networkComponent.SendMessage(world.Key, entity.Key, false, true, conn);
				}
			}
		}
	}
}