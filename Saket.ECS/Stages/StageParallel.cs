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
        List<DelegateSystemParallel> systems;

        public StageParallel()
        {
            systems = new List<DelegateSystemParallel>();
        }

        public StageParallel Add(DelegateSystemParallel @delegate)
        {
            systems.Add(@delegate);
            return this;
        }

        public void Update(World world)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                for (int y = 0; y < ThreadPool.ThreadCount; y++)
                {
                    ThreadPool.QueueUserWorkItem((a) => systems[i].Invoke(world, y));
                }
            }
            // Only return once all work is done
            while(ThreadPool.PendingWorkItemCount != 0)
            {
                // TODO: find a better way of waiting for all work to be completed
                Thread.Sleep(1);
            }
        }
    }
}
