Work in progress Archetype Entity Component System (ECS) in C#

Current Features:

- Full C# Archetype ECS system
- Non-limited Archetype and Component Count
- Easily add multiple components to Entities with Bundles
- Shared Resources (singleton components) per world.
- Fully Managed. No External dependencies.

Usage Example:
```csharp
struct Position{
	public float x;
	public float y;
}

struct Velocity{
	public float x;
	public float y;
}

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

Query query_positionVelocity = new Query().With<(Postion, Velocity)>();

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
	- Query Cache invalidation when adding or removing from archetype that was matched with query
	- Two options for when to requery
		- Manual Hard Sync points for when to refresh queries. This could increase performance since non depended stages do not need to refresh queries between them.
		- Automatically sync at the end of stages
		

- Fast Lockless Parallel System Execution
	- Avoid Locking to increase performance. This could be done by Defering Spawn/Destroy Commands. 
		- If multiple parallel systems seperatly destroy or spawn entities world would have to use locks to ensure no race condition.
		- Accumulating all commands until end of parallel execution and then merging and excuting them single threaded could allow for faster execution.
		- When merging:
			- if an entity is destroyed in one thread and then modified in an other this should result in an exception. The opposite wouldn't be wrong but would waste cycles.
			- 
		- Is there no need to syncronize writing/setting component data. Since there is no solution if two parallel threads change same component. It should therefore be avoided. This cannot be merged.
	- Add padding to stored components in ComponentStorage to avoid crossing cachline barriers to avoid false sharing


TODO Future Releases:
- Prefabs with nesting and auto update of instances
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
- Change Detection like BevyECS