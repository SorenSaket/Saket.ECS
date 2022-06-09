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
    public class Stage
    {
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

            public int ArgumentIndex_Delta;

            public List<int> QueryIndexes;

            public List<int> QuerieySignatures;

            public DynamicSystem(Delegate method)
            {
                Method = method;
                var methodInfo = method.GetMethodInfo();
                var parameters = methodInfo.GetParameters();
                SystemFunction = methodInfo.Bind();
               
                
                Arguments = new object[parameters.Length];
                ArgumentIndex_Delta = -1;
                QueryIndexes = new List<int>();
                QuerieySignatures = new List<int>();

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
                        QueryIndexes.Add(i);
                        QuerieySignatures.Add(parameters[i].ParameterType.GetHashCode());
                    }
                }
            }
        }

        List<DynamicSystem> dynamicSystems;

        //
        public Stage()
        {
            dynamicSystems = new List<DynamicSystem>();
        }

        public Stage Add(Delegate @delegate)
        {
            dynamicSystems.Add(new DynamicSystem(@delegate));
            return this;
        }


        internal void Update(World world)
        {
            for (int i = 0; i < dynamicSystems.Count; i++)
            {
                Invoke(dynamicSystems[i], world);
            }
        }
        //
        internal void Invoke(DynamicSystem system, World world)
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
        }
    }
}