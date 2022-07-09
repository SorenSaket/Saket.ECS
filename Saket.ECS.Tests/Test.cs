using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    [TestClass]
    public class Test
    {
        World world;

        Query query;

        [TestMethod]
        public void Start()
        {
            // Entities must both have position and velocity component
            query = new Query().With<(Position,Velocity)>();
            
            // Create new world
            world = new World();

            // Create stage
            var stage = new Stage();
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
            entity.Add(bundle);
        }

        public void Update(float delta)
        {
            world.Update(delta);
        }

        void SystemMove(World world)
        {
            foreach (var entity in world.Query(query))
            {
                // Move the entity forward
                var position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();
                
                entity.Set<Position>(position.Value + (velocity.Value * world.Delta));
            }
        }
    }
}