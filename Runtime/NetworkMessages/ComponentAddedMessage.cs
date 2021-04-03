using Mirror;

namespace NetworkMessages
{
	public struct ComponentAddedMessage : NetworkMessage
	{
		public int entityId;
		public int componentId;
	}
}