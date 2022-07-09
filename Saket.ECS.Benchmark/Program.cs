using BenchmarkDotNet.Running;
using Saket.ECS;

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
Console.ReadKey();