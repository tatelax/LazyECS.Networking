using LazyECS.Component;

public interface INetworkComponent : IComponent
{
	void SendMessage(int _worldId, int _entityId, bool toClients, bool setFromNetworkMessage = false);
}