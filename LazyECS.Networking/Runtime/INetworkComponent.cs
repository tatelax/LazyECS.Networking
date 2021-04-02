using LazyECS.Component;
using Mirror;

public interface INetworkComponent : IComponent
{
	void SendMessage();
}