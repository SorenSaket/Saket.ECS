using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    struct Velocity : IEquatable<Velocity>
    {
        Vector2 position;
        public Velocity(float x, float y)
        {
            position = new Vector2(x, y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Velocity velocity && Equals(velocity);
        }

        public bool Equals(Velocity other)
        {
            return position.Equals(other.position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position);
        }

        public static bool operator ==(Velocity left, Velocity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Velocity left, Velocity right)
        {
            return !(left == right);
        }
    }


    struct Complex : IComponent, IEquatable<Complex>
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

        public override bool Equals(object? obj)
        {
            return obj is Complex complex && Equals(complex);
        }

        public bool Equals(Complex other)
        {
            return value == other.value &&
                   value2 == other.value2 &&
                   cool == other.cool;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Complex left, Complex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return !(left == right);
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
