using Mirror;

public struct BoolComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public bool Value;
	
	public BoolComponentMessage(int worldID, int entityID, int componentID, bool value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendBoolComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendBoolComponent(int worldId, int entityId, int componentId, bool value)
	{
		BoolComponentMessage msg = new BoolComponentMessage
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