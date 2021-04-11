using System;
using System.Collections.Generic;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using Mirror;
using NetworkMessages;

public class NetworkWorldBufferHandler
{
	public void OnClientConnected(NetworkConnection conn)
	{
		// Loop through all worlds
		foreach (KeyValuePair<int,IWorld> world in SimulationController.Instance.Worlds)
		{
			// We only care about network worlds
			if(world.Value.GetType().BaseType != typeof(NetworkWorld)) continue;
			
			// Loop through all the entities in that network world
			//TODO: Take some snapshot in case entities are modified while sending this?
			foreach (KeyValuePair<int,Entity> entity in world.Value.Entities)
			{
				// Send a message to the client that connected telling them to create an entity in the same world with the same id
				CreateEntityMessage createEntityMessage = new CreateEntityMessage {worldId = world.Key, id = entity.Value.id};
				conn.Send(createEntityMessage);
				
				// Loop through all components on the entity
				//TODO: Take some snapshot in case components are modified while sending this?
				foreach (KeyValuePair<Type,IComponent> component in entity.Value.Components)
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
