using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FastDelegate.Net;

namespace engine.ecs
{
    /// <summary> 
    /// Control when a system is ran (fixed timestep or conditional).
    /// Control Parallel execution.
    /// Auto injection of method parameters.
    /// Invokation using fast dyamic invoke.
    /// </summary>
    public class Stage
    {
        //delegate Object SystemFunction (Object instance, Object[] arguments);
        internal struct System
        {
            public Delegate Method;
            public Func<Object,Object[],Object> SystemFunction;

            public object[] Arguments;

            public int ArgumentIndex_Delta;
            public int ArgumentIndex_Query;

            public System(Delegate method)
            {
                Method = method;
                var methodInfo = method.GetMethodInfo();
                var parameters = methodInfo.GetParameters();
                SystemFunction = methodInfo.Bind();
               
                
                Arguments = new object[parameters.Length];
                ArgumentIndex_Delta = ArgumentIndex_Query = -1;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(float))
                    {
                        if (parameters[i].Name.ToLowerInvariant() == "delta")
                        {
                            ArgumentIndex_Delta = i;
                        }
                    }
                    else if (parameters[i].ParameterType == typeof(Query))
                    {
                        ArgumentIndex_Query = i;
                    }
                }
            }
        }

        List<System> dynamicSystems;

        //
        public Stage()
        {
            dynamicSystems = new List<System>();
        }

        public void AddSystem(Delegate @delegate)
        {
            dynamicSystems.Add(new System(@delegate));
        }


        internal void Update(World world)
        {
            for (int i = 0; i < dynamicSystems.Count; i++)
            {
                Invoke(dynamicSystems[i], world);
            }
        }


        //
        internal void Invoke(System system, World world)
        {
            if(system.ArgumentIndex_Delta != -1)
            {
                system.Arguments[system.ArgumentIndex_Delta] = 0f;
            }

            if (system.ArgumentIndex_Query != -1)
            {
                // Get the query
                // TODO
                system.Arguments[system.ArgumentIndex_Query] = null;

            }

            //
            system.SystemFunction.Invoke(system.Method.Target, system.Arguments);
        }
    }
}