using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace engine.ecs
{
    // Pipline
    // The purpose of the pipeline is to describe the system behaviour of a world
    // Order of stage execution
    // 

    // Stage: 
    // Control when a system is ran (fixed timestep or conditional)
    // Control Parallel execution
    // 
    // 


    // Auto injection of method parameters
    // Invokation using fast dyamic invoke

    internal class Pipeline
    {
        public List<Stage> stages;

        public void Update(float delta)
        {
            for (int i = 0; i < stages.Count; i++)
            {
                //stages[i].
            }
        }
        // 
        public void AddStage(ISystem stage)
        {
            //stages.Add(new Stage(stage));
        }

        public void AddStage(Delegate stage)
        {
            stages.Add(new Stage(stage));
        }

        public void AddStage(Stage stage)
        {
            stages.Add(stage);
        }
    }
}
