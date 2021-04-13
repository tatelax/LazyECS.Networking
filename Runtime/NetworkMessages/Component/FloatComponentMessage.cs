using Mirror;

public struct FloatComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public float Value;
	
	public FloatComponentMessage(int worldID, int entityID, int componentID, float value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendFloatComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendFloatComponent(int worldId, int entityId, int componentId, float value)
	{
		FloatComponentMessage msg = new FloatComponentMessage
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