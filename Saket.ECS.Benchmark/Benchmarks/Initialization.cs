using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Saket.ECS.Benchmark.Benchmarks.Initialization
{
    [BenchmarkCategory("Initialization")]
    [MemoryDiagnoser]
    internal class Initialization
    {
       
        [Params(100000)]
        public int EntityCount { get; set; }

        [IterationSetup]
        public void Setup()
        {
            
        }


        [BenchmarkCategory("SveltoECS")]
        [Benchmark]
        public void SveltoECS()
        {
            
        }

        [IterationCleanup]
        public void Cleanup()
        {
            
        }
    }
}
