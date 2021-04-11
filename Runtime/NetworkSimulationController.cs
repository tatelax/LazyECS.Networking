public class NetworkSimulationController : SimulationController
{
	protected override void Awake()
	{
		base.Awake();
			
		NetworkWorldMessageHandler.RegisterHandlers();
	}
}