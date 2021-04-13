using Mirror;
using UnityEngine;

public struct Vector3ComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public Vector3 Value;
	
	public Vector3ComponentMessage(int worldID, int entityID, int componentID, Vector3 value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendVector3Component(worldID, entityID, componentID, Value);
	}
	
	private void SendVector3Component(int worldId, int entityId, int componentId, Vector3 value)
	{
		Vector3ComponentMessage msg = new Vector3ComponentMessage
		{
			worldID = worldId,
			entityID = entityId,
			componentID = componentId,
			Value = value
		};

		if (NetworkServer.active)
			NetworkServer.SendToAll(msg);
		else
			NetworkClient.Send(msg);
	}
}