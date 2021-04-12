using Mirror;

namespace NetworkMessages
{
	public struct CreateEntityMessage : NetworkMessage
	{
		public int worldId;
		public int id;
	}
}