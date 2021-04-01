using LazyECS.Component;
using Mirror;

public interface INetworkComponent : IComponent, NetworkMessage
{
	void SendMessage();
}