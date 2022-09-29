using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saket.ECS.Storage;
using static System.Formats.Asn1.AsnWriter;

namespace Saket.ECS.Tests.Storage
{
    [TestClass]
    public class Test_IComponentStorage
    {
        // TODO may solid tests

        [TestMethod]
        public void Test_Storage_Basic()
        {
            ComponentStorage store = new ComponentStorage(typeof(Complex));
            store.EnsureSize(10000);
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(9999);
                Complex input = new Complex((float)random.NextDouble(), (float)random.NextDouble(), true);
                storetest(store, index, input);
            }

        }

        [TestMethod]
        public void Test_Storage_ExplicitLayout()
        {
            ComponentStorage store = new ComponentStorage(typeof(ExplicitComplex));
            store.EnsureSize(10000);
            Random random = new Random();
            flags flags = 0;
            flags |= flags.first;
            flags |= flags.third;

            ExplicitComplex input = new ExplicitComplex(1.912492f, true, flags);



            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(9999);
               
                storetest(store, index, input);
            }

        }

        [TestMethod]
        public void Test_Storage_Union()
        {
            ComponentStorage store = new ComponentStorage(typeof(UnionComplex));
            store.EnsureSize(10000);
            Random random = new Random();

            UnionComplex input = new UnionComplex{cool = false, UncoolValue = float.MaxValue };

            for (int i = 0; i < 10; i++)
            {
                storetest(store, 9-i, input);
            }

        }

        [TestMethod]
        public unsafe void Test_Storage_Copy_GetSet()
        {
            ComponentStorage from = new ComponentStorage(typeof(Complex));
            ComponentStorage to = new ComponentStorage(typeof(Complex));
            from.EnsureSize(10);
            to.EnsureSize(10);

            // Random fill value
            Random random = new Random();

            // Input value
            Complex input = new Complex((float)random.NextDouble(), (float)random.NextDouble(), true);


            for (int y = 0; y < 10; y++)
            {
                from.Set(y, input);
            }

            for (int y = 0; y < 10; y++)
            {
                to.Set(y, from.Get(y));
            }

            for (int y = 0; y < 10; y++)
            {
                Assert.AreEqual(input, to.Get<Complex>(y));
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
