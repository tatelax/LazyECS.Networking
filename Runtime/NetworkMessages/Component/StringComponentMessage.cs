using Mirror;

public struct StringComponentMessage : NetworkMessage
{
	public int worldID;
	public int entityID;
	public int componentID;
	public string Value;
}