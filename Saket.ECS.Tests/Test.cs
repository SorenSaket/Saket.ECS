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
    public class Test
    {
        World world;

        public void Start()
        {
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

        void SystemMove(float delta, 
            Query<Position, Velocity, Without<Enemy>> query,
            Query<Pants, Without<Hat>> query2
            )
        {
            foreach (var entity in query)
            {
                // Move the entity forward
                var position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();
                
                entity.Set<Position>(position.Value + (velocity.Value * delta));

                var pants = entity.TryGet<Pants>();
                var hat = entity.TryGet<Hat>();

                if (hat.HasValue)
                {
                    // Do something with the hat
                   // hat.Value 
                }

                if(pants.HasValue)
                {
                    // Do something with the pants
                }
            }
        }
    }
}