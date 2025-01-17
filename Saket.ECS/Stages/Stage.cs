using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FastDelegate.Net;

namespace Saket.ECS
{
    /// <summary> 
    /// Control when a system is ran (fixed timestep or conditional).
    /// Control Parallel execution.
    /// Auto injection of method parameters.
    /// Invokation using fast dyamic invoke.
    /// </summary>
    public class Stage : IStage
    {
        /*
        //delegate Object SystemFunction (Object instance, Object[] arguments);
        /// <summary>
        /// Internal representation of a Dynamic System
        /// Uses reflection to inject values
        /// </summary>
        internal struct DynamicSystem
        {
            public Delegate Method;
            // Instance, Arguments, Return Value
            public Func<Object, Object[], Object> SystemFunction;

            public object[] Arguments;

            public int ArgumentIndex_World;

            public DynamicSystem(Delegate method)
            {
                Method = method;
                var methodInfo = method.GetMethodInfo();
                var parameters = methodInfo.GetParameters();
                SystemFunction = methodInfo.Bind();
               
                
                Arguments = new object[parameters.Length];
                ArgumentIndex_World = -1;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(World))
                    {
                        ArgumentIndex_World = i;
                    }
                }
            }
        }

        List<DynamicSystem> dynamicSystems;
        */
        //

        /*
        public Stage Add(Delegate @delegate)
        {
            dynamicSystems.Add(new DynamicSystem(@delegate));
            return this;
        }*/
        List<DelegateSystem> systems;

        public Stage()
        {
            systems = new List<DelegateSystem>();
        }

        public Stage Add(DelegateSystem @delegate)
        {
            systems.Add(@delegate);
            return this;
        }

        public Stage Add(Span<DelegateSystem> @delegates)
        {
            systems.AddRange(@delegates);
            return this;
        }
        public void Update(World world)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Invoke(world);
            }
        }
        ///
       
        /*internal void Invoke(DynamicSystem system, World world)
        {
            if(system.ArgumentIndex_Delta != -1)
            {
                system.Arguments[system.ArgumentIndex_Delta] = 0f;
            }

            for (int i = 0; i < system.QueryIndexes.Count; i++)
            {
                // Get the query from world
                // TODO
                system.Arguments[system.QueryIndexes[i]] = world.GetQuery(system.QuerieySignatures[i]);
            }

            //
            system.SystemFunction.Invoke(system.Method.Target, system.Arguments);
        }*/
    }
}