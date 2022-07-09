using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saket.ECS.Storage;

namespace Saket.ECS.Tests.Storage
{
    [TestClass]
    public class Test_IComponentStorage
    {
        struct Complex : IComponent
        {
            public float value;
            public bool cool;

            public Complex(float value, bool cool)
            {
                this.value = value;
                this.cool = cool;
            }

            public override bool Equals(object? obj)
            {
                return obj is Complex complex &&
                       value == complex.value &&
                       cool == complex.cool;
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 6, CharSet = CharSet.Ansi)]
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

        [StructLayout(LayoutKind.Explicit, Size = 5, CharSet = CharSet.Ansi)]
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



        [TestMethod]
        public void Test_Storage_Basic()
        {
            ComponentStorage store = new ComponentStorage(typeof(Complex));

            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(999999);
                Complex input = new Complex(1.912492f, true);
                storetest(store, index, input);

            }
        }

        [TestMethod]
        public void Test_Storage_ExplicitLayout()
        {
            ComponentStorage store = new ComponentStorage(typeof(ExplicitComplex));

            Random random = new Random();
            flags flags = 0;
            flags |= flags.first;
            flags |= flags.third;

            ExplicitComplex input = new ExplicitComplex(1.912492f, true, flags);



            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(999999);
               
                storetest(store, index, input);
            }

        }

        [TestMethod]
        public void Test_Storage_Union()
        {
            ComponentStorage store = new ComponentStorage(typeof(UnionComplex));

            Random random = new Random();

            UnionComplex input = new UnionComplex{cool = false, UncoolValue = float.MaxValue };

            for (int i = 0; i < 10; i++)
            {
                storetest(store, 9-i, input);
            }

        }

        private void storetest<T>(IComponentStorage store, int index, T input)
            where T : unmanaged
        {
            store.Set<T>(index, input);
            T output = store.Get<T>(index);
            Assert.AreEqual(input, output);
        }

    }
}
