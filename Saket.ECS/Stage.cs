using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace engine.ecs
{
    internal class Stage
    {
        Delegate _delegate;
        //
        public Stage(Delegate function)
        {
            _delegate = function;
        }

        //
        public void Invoke(World world)
        {
            // TODO move this to constructor
            var paramters =  _delegate.GetMethodInfo().GetParameters();
            object[] args = new object[paramters.Length];

            //
            for (int i = 0; i < paramters.Length; i++)
            {
                //
                if(paramters[i].ParameterType == typeof(Query))
                {
                    // Get cached query and
                    //args[i] =
                }
                else
                {
                    // https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype
                    // This should be illegal and throw an exception
                    args[i] = Activator.CreateInstance(paramters[i].ParameterType);
                }
            }

            // http://www.tomdupont.net/2016/11/10x-faster-than-delegatedynamicinvoke.html
            _delegate.DynamicInvoke(args);

        }
    }
}
