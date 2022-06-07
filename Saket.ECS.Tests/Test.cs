using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace engine.ecs
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


    public class Test
    {
        World world;

        public void Start()
        {
            // Create new world
            world = new World();

            // Create stage
            var stage = new Stage();
            stage.AddSystem(SystemMove);

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

        void SystemMove(float delta, Query<Position, Velocity> query)
        {
            foreach (var (entity, positition, velocity) in query)
            {
                entity.Set<Position>(positition.Value + (velocity.Value * delta));
            }
        }
    }
}