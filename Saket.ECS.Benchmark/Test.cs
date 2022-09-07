using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;

namespace Saket.ECS
{

    struct Position : IComponent
    {
        public Vector2 Value;

        public Position(float x, float y)
        {
            Value = new Vector2(x, y);
        }
        public Position(Vector2 value)
        {
            Value = value;
        }
        public static implicit operator Position(Vector2 v) => new Position(v);

        public override string? ToString()
        {
            return Value.ToString();
        }
    }
    struct Velocity : IComponent
    {
        public Vector2 Value;

        public Velocity(float x, float y)
        {
            Value = new Vector2(x, y);
        }
        public Velocity(Vector2 value)
        {
            Value = value;
        }
        public static implicit operator Velocity(Vector2 v) => new Velocity(v);
    }
    struct Hat : IComponent
    {

    }
    struct Pants : IComponent
    {

    }
    struct Enemy : IComponent
    {

    }

    public class Test
    {
        public World world;

        public Query query;

        public void Start()
        {
            // Create a query that requires both position and velocity component
            query = new Query().With<(Position, Velocity)>();

            // Create new world
            world = new World();

            // Create stage
            var stage = new Stage();
            // Add the system to the stage
            stage.Add(SystemMove);

            // Create pipeline
            var pipeline = new Pipeline();
            pipeline.AddStage(stage);


            world.SetPipeline(pipeline);

            // Create position velocity bundle
            var bundle = Bundle.Create<Position, Velocity>();

            // Create a new entity
            var entity = world.CreateEntity();
            // Add component bundle to entity
            entity.Add(new Position());
            entity.Add(new Velocity(1,1));
        }

        public void Update(float delta)
        {
            world.Update(delta);
        }

        void SystemMove(World world)
        {
            var result = world.Query(query);
            foreach (var entity in result)
            {
                // Move the entity forward
                var position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();

                entity.Set<Position>(position.Value + (velocity.Value * world.Delta));
                Console.WriteLine(entity.Get<Position>());
            }
        }

        void SystemMoveParallel(World world)
        {
            var result = world.Query(query);
            foreach (var entity in result)
            {
                // Move the entity forward
                var position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();

                entity.Set<Position>(position.Value + (velocity.Value * world.Delta));
                Console.WriteLine(entity.Get<Position>());
            }
        }
    }

}