using Mirror;

public struct UIntComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public uint Value;
	
	public UIntComponentMessage(int worldID, int entityID, int componentID, uint value)
	{
		this.worldID = worldID;
		this.entityID = entityID;
		this.componentID = componentID;
		Value = value;
		
		SendUIntComponent(worldID, entityID, componentID, Value);
	}
	
	private void SendUIntComponent(int worldId, int entityId, int componentId, uint value)
	{
		UIntComponentMessage msg = new UIntComponentMessage
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