using LazyECS.Component;

public interface INetworkComponent : IComponent
{
	void SendMessage(bool toClients);
}