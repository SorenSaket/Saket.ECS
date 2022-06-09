using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Saket.ECS
{
    // Pipline
    // The purpose of the pipeline is to describe the system behaviour of a world
    // Order of stage execution
    // 

   

    public class Pipeline
    {
        public List<Stage> stages;

        public Pipeline()
        {
            stages = new List<Stage>();
        }

        internal void Update(World world)
        {
            for (int i = 0; i < stages.Count; i++)
            {
                stages[i].Update(world);
            }
        }

        public void AddStage(Stage stage)
        {
            stages.Add(stage);
        }
    }
}
