using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using UnityEngine;

public class NetworkWorld : World
{
	public override void ComponentAddedToEntity(IEntity entity, IComponent component)
	{
		base.ComponentAddedToEntity(entity, component);
		Debug.Log("A component was added");
	}

	public override void ComponentRemovedFromEntity(IEntity entity, IComponent component)
	{
		base.ComponentRemovedFromEntity(entity, component);
		Debug.Log("A component was removed");
	}

	public override void ComponentSetOnEntity(IEntity entity, IComponent component)
	{
		base.ComponentSetOnEntity(entity, component);
		Debug.Log("A component was set");
	}
}