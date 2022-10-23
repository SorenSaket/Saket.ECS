using BenchmarkDotNet.Attributes;
using DefaultEcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Benchmark.Benchmarks.Iteration
{
    [BenchmarkCategory("Initialization")]
    [MemoryDiagnoser]
    public class Iteration
    {
        [Params(10000000)]
        public int EntityCount { get; set; }


        Query query = new Query().With<(Position, Velocity)>();
        Saket.ECS.World saket_world = new ();

        DefaultEcs.World default_world = new ();
        EntityQueryBuilder default_query;
        public Iteration()
        {
            for (int i = 0; i < EntityCount; i++)
            {
                var e = saket_world.CreateEntity();
                e.Add(new Position());
                e.Add(new Velocity());


                var e2 = default_world.CreateEntity();
                e2.Set(new Position());    
                e2.Set(new Velocity());
            }
            default_query = default_world.GetEntities().With<Position>().With<Velocity>();
        }

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < EntityCount; i++)
            {
                var e = saket_world.CreateEntity();
                e.Add(new Position());
                e.Add(new Velocity());


                var e2 = default_world.CreateEntity();
                e2.Set(new Position());
                e2.Set(new Velocity());
            }
            query = new Query().With<(Position, Velocity)>();
            default_query = default_world.GetEntities().With<Position>().With<Velocity>();
        }


        [BenchmarkCategory("SaketECS")]
        [Benchmark(Baseline = true)]
        public void SaketECS()
        {
            var entities = saket_world.Query(query);
            foreach (var entity in entities)
            {
                var position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();
                position.value += velocity.value;
                entity.Set(position);
            }
        }

        [BenchmarkCategory("DefaultECS")]
        [Benchmark]
        public void DefaultECS()
        {
            var entities = default_world.GetEntities().With<Position>().With<Velocity>().AsEnumerable();
            foreach (var entity in entities)
            {
                var  position = entity.Get<Position>();
                var velocity = entity.Get<Velocity>();
                position.value += velocity.value;
            }
        }


        [IterationCleanup]
        public void Cleanup()
        {

        }
    }
}
