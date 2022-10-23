
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Benchmark
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Position 
    {
        public Vector2 value;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct Velocity
    {
        public Vector2 value;

        public Velocity(Vector2 value)
        {
            this.value = value;
        }
    }
}
