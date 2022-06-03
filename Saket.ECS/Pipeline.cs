using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace engine.ecs
{
    internal class Pipeline
    {
        public List<Stage> stages;

        public void Update(float delta)
        {
            for (int i = 0; i < stages.Count; i++)
            {
                stages[i].
            }
        }

        public void AddStage(ISystem stage)
        {
            stages.Add(new Stage(stage));
        }


        public void AddStage(Delegate stage)
        {
            stage.GetMethodInfo().GetParameters();
            stage.DynamicInvoke(null, new object[] { stage });

            stages.Add(new Stage(stage));
        }

        public void AddStage(Stage stage)
        {
            stages.Add(stage);
        }
    }
}
