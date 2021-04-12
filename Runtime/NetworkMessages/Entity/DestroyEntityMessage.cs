using Mirror;

namespace NetworkMessages
{
	public struct DestroyEntityMessage : NetworkMessage
	{
		public int worldId;
		public int id;
	}
}