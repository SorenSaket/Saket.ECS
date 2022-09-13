using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public class StageParallel : IStage
    {
        ManualResetEvent[] manualResetEvents;
        List<DelegateSystemParallel> systems;

        private readonly int numberOfThreads;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfThreads"></param>
        public StageParallel(int numberOfThreads)
        {
            this.manualResetEvents = Array.Empty<ManualResetEvent>();
            this.systems = new List<DelegateSystemParallel>();
            this.numberOfThreads = numberOfThreads;
        }

        public StageParallel Add(DelegateSystemParallel @delegate)
        {
            systems.Add(@delegate);

            int previousSize = manualResetEvents.Length;
            Array.Resize(ref manualResetEvents, systems.Count*numberOfThreads);
            for (int i = previousSize; i < manualResetEvents.Length; i++)
            {
                manualResetEvents[i] = (new ManualResetEvent(false));
            }
            return this;
        }

        public void Update(World world)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                for (int y = 0; y < numberOfThreads; y++)
                {
                    manualResetEvents[i*numberOfThreads + y].WaitOne();

                    ThreadPool.QueueUserWorkItem(
                        (a) => {
                            systems[i].Invoke(world, y, numberOfThreads);
                            
                            manualResetEvents[i].Set();
                        }
                   );

                }
            }

            WaitHandle.WaitAll(manualResetEvents);
        }
    }
}
