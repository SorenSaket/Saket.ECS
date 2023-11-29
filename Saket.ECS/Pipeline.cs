using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Saket.ECS
{
    /// <summary>
    /// Grouping of behavior
    /// The purpose of the pipeline is to describe the system behaviour of a world
    /// Order of stage execution
    /// </summary>
    public class Pipeline
    {
        public List<IStage> Stages { get; set; }

        public Pipeline()
        {
            Stages = new List<IStage>();
        }

        public void Update(World world)
        {
            for (int i = 0; i < Stages.Count; i++)
            {
                Stages[i].Update(world);
                // TODO insert query updates
            }
        }

        public void AddStage(IStage stage)
        {
            Stages.Add(stage);
        }
    }
}
