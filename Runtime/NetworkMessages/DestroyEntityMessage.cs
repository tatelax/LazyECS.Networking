using Mirror;

namespace NetworkMessages
{
	public struct DestroyEntityMessage : NetworkMessage
	{
		public int id;
	}
}