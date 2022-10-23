
using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Saket.ECS;
//https://github.com/coder0xff/FastDelegate.Net
using FastDelegate.Net;


// RATIO:
// Invoke:          1
// DynamicInvoke    2685.60
// FastDelegateNet  30 
//



    [SimpleJob(RunStrategy.Monitoring, targetCount: 5)]
    public class Test_DynamicInvoke
    {
        Delegate _delegate;
        object[] args;
        EfficientInvoker invoker;

        Func<Object, Object[], Object> fastdel;

        public Test_DynamicInvoke()
        {
            _delegate = del;
            args = new object[2] {2f, "asdasd"};

            invoker = EfficientInvoker.ForDelegate(_delegate);
            fastdel = _delegate.Method.Bind();

    }

        float del(float delta, string data)
        {
            return delta + ((byte)data[0]);
        }


        [Benchmark(Baseline = true)]
        public void Invoke()
        {
            for (int i = 0; i < 10000; i++)
            {
                del(2f, "asdasd");
            }
        }
        [Benchmark]
        public void DynamicInvoke()
        {
            for (int i = 0; i < 10000; i++)
            {
                _delegate.DynamicInvoke(args);
            }
        }

        // Not working
        //[Benchmark]
        public void EfficientDynamicInvoke()
        {
            for (int i = 0; i < 10000; i++)
            {
                invoker.Invoke(this, args);
            }
        }

        [Benchmark]
        public void FastDelegateNet()
        {
            for (int i = 0; i < 10000; i++)
            {
                fastdel.Invoke(this, args);
            }
        }
}
