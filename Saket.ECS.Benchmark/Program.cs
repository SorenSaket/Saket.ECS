using BenchmarkDotNet.Running;
using Saket.ECS;
using Saket.ECS.Benchmark.Benchmarks.Iteration;
using System.Diagnostics;
using System.Numerics;


public static class Program
{
    [STAThread]
    static void Main()
    {
        BenchmarkRunner.Run(typeof(Iteration));
        Console.ReadKey();
    }
}



/*

var test = new Test();
test.Start();
while (true)
{
    test.Update(1f / 60f);
    Thread.Sleep(16);
}*/


/*
//var a = new Test_DynamicInvoke();
//a.EfficientDynamicInvoke();
Console.WriteLine(LogicalProcessorInformation.Information.Length);

for (int i = 0; i < LogicalProcessorInformation.Information.Length; i++)
{
    Console.WriteLine(Convert.ToString((uint)LogicalProcessorInformation.Information[i].ProcessorMask, 2));
    Console.WriteLine(LogicalProcessorInformation.Information[i].ProcessorInformation.ProcessorCore.Flags);
    Console.WriteLine(LogicalProcessorInformation.Information[i].ProcessorInformation.Cache.Type);
    Console.WriteLine(LogicalProcessorInformation.Information[i].ProcessorInformation.Cache.LineSize);
    Console.WriteLine("--------");
}


//BenchmarkRunner.Run(typeof(Test_DynamicInvoke));
Console.ReadKey();*/


// Stuff to benchmark against:
// Entitas | 5.6k | https://github.com/sschmid/
// MonoGame.Extended| 1.1k | https://github.com/craftworkgames/MonoGame.Extended |
//| Svelto ecs | 797 | https://github.com/sebas77/Svelto.ECS |
//| DefaultEcs | 423 | https://github.com/Doraku/DefaultEcs |
//| ecsrx | 388 | https://github.com/EcsRx/ecsrx |
// https://github.com/Leopotam/ecs
//
// Smaller less known Hobby ecs projects:
// https://github.com/NicholasHallman/MonoECS
// https://github.com/ykafia/ECSharp
// https://github.com/hippiehunter/ArchECS/tree/main/ArchECS
// https://github.com/huodianyan/Poly.ArcEcs
// https://github.com/voledyhil/MiniEcs