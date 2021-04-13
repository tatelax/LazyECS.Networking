using Mirror;

public struct StringComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public string Value;
	
	public StringComponentMessage(int worldID, int entityID, int componentID, string value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendStringComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendStringComponent(int worldId, int entityId, int componentId, string value)
	{
		StringComponentMessage msg = new StringComponentMessage
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