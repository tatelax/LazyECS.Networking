# LazyECS.Networking
This is a networking addon for LazyECS. It uses Mirror behind the scenes to automatically synchronize entity changes from Server to Client (vice versa not available yet). You can pick which components get synchronized on a given entity. All entities in a NetworkWorld are synchronized across the network.

# Example Project

[Example Project](https://github.com/tatelax/LazyECSNetworkingExample)

# Dependencies

* [Lazy ECS](https://github.com/tatelax/LazyECS) v1.1.x+
* [Mirror Networking](https://github.com/vis2k/Mirror) v35.1.x+
* Unity 2019.1.x+

# How To Install
LazyECS Networking can be installed via the Unity Package Manager.
```
Window/Package Manager => + Button => "Add package from git URL..." => https://github.com/tatelax/LazyECS.Networking.git
```
# Usage

### Step 1

Change the base class of the World you wish to sync from **World** to **NetworkWorld**.

```csharp
using LazyECS;

public class MainWorld : NetworkWorld
{
	public override void Init()
	{
		base.Init();

		features = new Feature[]
		{
			new MainFeature()
		};
	}
}
```

### Step 2

Change the base class of the components you wish to sync from **IComponent** to **INetworkComponent**.

```csharp
public class PlayerNameComponent : INetworkComponent
{
	public string Value { get; private set; }

	public bool Set(object value)
	{
		Value = (string) value;
		return true;
	}

	public object Get() => Value;
}
```

### Step 3

Use LazyECS as normal.

# Important Info

Lazy ECS Networking is not production ready. There might be bugs.

# Support
You can contact me on Discord: ```tatelax#0001```
