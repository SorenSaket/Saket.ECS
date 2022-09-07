using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public delegate void DelegateSystem(World world);
    public delegate void DelegateSystemParallel(World world, int thread);
    public interface ISystem
    {
    }
}
