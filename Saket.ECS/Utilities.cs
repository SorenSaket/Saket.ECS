using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public static class Utilities
    {


        // Todo add other eventual conditions 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValidComponent(Type type)
        {
            if (!type.IsValueType)
                return false;

            return true;
        }

    }
}
