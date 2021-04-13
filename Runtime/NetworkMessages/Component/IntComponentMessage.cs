using Mirror;

public struct IntComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public int Value;
	
	public IntComponentMessage(int worldID, int entityID, int componentID, int value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendStringComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendStringComponent(int worldId, int entityId, int componentId, int value)
	{
		IntComponentMessage msg = new IntComponentMessage
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