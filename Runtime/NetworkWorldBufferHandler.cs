using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;

public class NetworkWorldBufferHandler
{
	/// <summary>
	/// When a client joins, we need to give them all of the entities, components, and component values so they are in sync with the server
	/// </summary>
	/// <param name="conn">The NetworkConnection of the player</param>
	public void BufferClient(NetworkConnection conn)
	{
		// Loop through all worlds
		foreach (KeyValuePair<int,IWorld> world in SimulationController.Instance.Worlds)
		{
			// We only care about network worlds
			if(world.Value.GetType().BaseType != typeof(NetworkWorld)) continue;
			
			// Loop through all the entities in that network world
			//TODO: What happens if an entity is created while the player is joining?
			foreach (KeyValuePair<int,Entity> entity in world.Value.Entities.ToList())
			{
				// Send a message to the client that connected telling them to create an entity in the same world with the same id
				CreateEntityMessage createEntityMessage = new CreateEntityMessage {worldId = world.Key, id = entity.Value.id};
				conn.Send(createEntityMessage);
				
				// Loop through all components on the entity
				//TODO: What happens if a component is changed while the player is joining?
				foreach (KeyValuePair<Type,IComponent> component in entity.Value.Components.ToList())
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
					INetworkComponent networkComponent = (INetworkComponent)component.Value;
					networkComponent.SendMessage(world.Key, entity.Key, false, true, conn);
				}
			}
		}
	}
}
