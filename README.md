Work in progress Archetype Entity Component System (ECS) in C#

Current Features:

- Full C# Archetype ECS system
- Easily add multiple components to Entities with Bundles
- Shared Resources (singleton components) per world.

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









Targeting release of 0.1.0 where all basic features are implemented, working and tested.

TODO for 0.1.0:
- Query Caching
	- Query Cache invalidation? dynamically modify queries as entities are created/modified
- Convert storage to resizing model
- Defer Spawn/Destroy Commands. Ensure in system command execution is possible to allow for lockless parallel execution.
- Parallel System Execution


TODO Future Releases:
- Entity World Transfer
- Remove Dependency of GCHandle to allow not blittable types like bool
- Support for Query Any<> any groups
- Full world Serialization and Cloning. Allows for instant saving and restoring full game state. Allows easy rollback netcode.
- Ensure possiblity for garbage free loop execution path
- True SOA Storage to allow SIMD
	- To allow this queries should be able to match destoyed entities. This is to avoid sparse data in case of SIMD, but increases default use case complexity. To avoid unintended behavior implement Active Component.	
- Event System
- Api documentation
- Extensive Testing
- Benchmark. And speed comparison to other C# ECS systems


Nice To have:
- Built-in transform hierarchy
- Full Tuple Support. / Remove bundles?
- Static parallel dependency finder. Allows for easy view of shared resources