using BenchmarkDotNet.Running;

var a = new Test_DynamicInvoke();
a.EfficientDynamicInvoke();


//BenchmarkRunner.Run(typeof(Test_DynamicInvoke));
Console.ReadKey();