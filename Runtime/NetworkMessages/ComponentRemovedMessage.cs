using Mirror;

namespace NetworkMessages
{
	public struct ComponentRemovedMessage : NetworkMessage
	{
		public int entityId;
		public int componentId;
	}
}