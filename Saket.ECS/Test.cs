using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace engine.ecs
{
    struct Position : IComponent
    {
        public float x;
        public float y;
    }
    struct Velocity : IComponent
    {
        public float x;
        public float y;
    }

    internal class Test
    {
        World world;

        //Query query_move = new ;

        void Start()
        {
            // Create new world
            world = new World();
            // Create pipeline
            var pipeline = new Pipeline();
            pipeline.AddStage(SystemMove);
            // add pipeline
            world.AddPipeline(pipeline);
            // Create a new entity
            var entity = world.CreateEntity();
            // Create bundle
            var bundle = Bundle.Create<Position, Velocity>();
            // Add component bundle to entity
            entity.Add(bundle);
        }

        void Update(float delta)
        {
            world.Update(delta);
        }

        unsafe void SystemMove(float delta)
        {
            /*query.GetResults(out var positions, out var velocities);
            int count = 0;
            

            for (int i = 0; i < count; i += Vector<float>.Count)
            {
                Vector<float> positionX = new Vector<float>(
                    new ReadOnlySpan<float>(positions.GetFieldPointer(0), positions.GetFieldOffset(0)));




                Vector<float> velocity = new Vector<float>();



                position = Vector.Add(position, velocity);
                position.CopyTo()
            }*/
        }
    }
}