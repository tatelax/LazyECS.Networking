using Mirror;

public struct StringArrayComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public string[] Value;
	
	public StringArrayComponentMessage(int worldID, int entityID, int componentID, string[] value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendStringArrayComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendStringArrayComponent(int worldId, int entityId, int componentId, string[] value)
	{
		StringArrayComponentMessage msg = new StringArrayComponentMessage
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