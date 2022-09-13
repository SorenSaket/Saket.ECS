using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Tests
{
    internal class TestBundle : Bundle
    {
        public override Type[] Components => components;
        public override object[] Data => data;


        private readonly static Type[] components = new Type[] { typeof(Position), typeof(Velocity)};
        private object[] data;

        internal TestBundle(Position position, Velocity velocity)
        {
            data = new object[2]
            {
                position,
                velocity
            };
        }
    }


    struct Position
    {
        public float x;
        public float y;

        public Position(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Velocity
    {
        public float x;
        public float y;

        public Velocity(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }


    struct Complex : IComponent
    {
        public float value;
        public float value2;
        public bool cool;

        public Complex(float value, float value2, bool cool)
        {
            this.value = value;
            this.value2 = value2;
            this.cool = cool;
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 6)]
    struct ExplicitComplex : IComponent
    {
        [FieldOffset(0)]
        public bool cool;
        [FieldOffset(1)]
        public float value;
        [FieldOffset(5)]
        public flags flags;

        public ExplicitComplex(float value, bool cool, flags flags)
        {
            this.value = value;
            this.cool = cool;
            this.flags = flags;
        }

        public override bool Equals(object? obj)
        {
            return obj is ExplicitComplex complex &&
                   value == complex.value &&
                   cool == complex.cool &&
                   flags == complex.flags;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 5)]
    struct UnionComplex : IComponent
    {
        [FieldOffset(0)]
        public bool cool;
        [FieldOffset(1)]
        public int coolValue;
        [FieldOffset(1)]
        public float UncoolValue;

    }

    [Flags]
    enum flags : byte
    {
        none = 0,
        first = 1,
        second = 2,
        third = 4,
        fourth = 8,
    }
}
