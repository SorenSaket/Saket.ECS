Work in progress Archetype Entity Component System (ECS) in C#

Targeting release of 0.1.0 where all basic features are implemented, working and tested.

Usage Example:
```csharp
World world;
Pipeline pipeline_update;
void Start(){
	world = new World();

	pipeline_update = new Pipeline();

	Stage stage_update = new Stage();
				
	stage_update.Add(System_Move);

	pipeline_update.AddStage(stage_update);

	var entity = world.CreateEntity();
	entity.Add(new Position(0,0))
	entity.Add(new Velocity(1f,-1f))
}

void Update(float Delta){
	pipeline_update.Update(world, Delta);
}

Query query_positionVelocity = new Query().With<Postion>().With<Velocity>();

void System_Move(World world){
	var entities = world.Query(query_positionVelocity);

	foreach(var entity in entities){
		var position = entity.Get<Position>();
		var velocity = entity.Get<Velocity>();

		position.x += velocity.x * world.Delta;
		position.y += velocity.y * world.Delta;

		entity.Set(position);
	}
}
```



Current Features:
<ul>
	<li>Full C# Archetype ECS system</li>
	<li>Easily Add multiple components to Entity with Bundles</li>
</ul>

TODO for 0.1.0:
<ul>
	<li>Component Removal Functions</li>
	<li>Query Caching</li>
	<li>Entity World Transfer</li>
	<li>Defer Spawn/Destroy Commands. Ensure in system command execution is possible to allow for lockless parallel execution.</li>
	<li>Parallel System Execution</li>
	<li>Resource System</li>
</ul>

TODO Future Releases:
<ul>
	<li>Full world Serialization and Cloning. Allows for instant saving and restoring full game state. Allows easy rollback netcode.</li>
	<li>Ensure possiblity for garbage free loop execution path</li>
	<li>True SOA Storage to allow SIMD</li>
		To allow this queries should be able to match destoyed entities. This is to avoid sparse data in case of SIMD, but increases default use case complexity. To avoid unintended behavior implement Active Component.	
	<li>Event System</li>
	<li>Static parallel depency finder. Allows for easy view of shared resources</li>
	<li>Api documentation</li>
	<li>Extensive Testing</li>
	<li>Benchmark. And speed comparison to other C# ECS systems</li>
</ul>

Nice To have
<ul>
	<li>Built-in transform hierarchy</li>
</ul>