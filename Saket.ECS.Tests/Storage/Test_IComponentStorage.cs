using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            ComponentStorage store = new ComponentStorage(typeof(Velocity));
            store.EnsureCapacity(10000);
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(9999);
                Velocity input = new Velocity((float)random.NextDouble(), (float)random.NextDouble());
                storetest(store, index, input);
            }

        }

        [TestMethod]
        public void Test_Storage_ExplicitLayout()
        {
            ComponentStorage store = new ComponentStorage(typeof(ExplicitComplex));
            store.EnsureCapacity(10000);
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
            store.EnsureCapacity(10000);
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
            from.EnsureCapacity(10);
            to.EnsureCapacity(10);

            // Random fill value
            Random random = new Random();

            for (int y = 0; y < 10; y++)
            {
                Complex input = new Complex((float)random.NextDouble(), (float)random.NextDouble(), true);
                from.Set(y, input);
            }

            for (int y = 0; y < 10; y++)
            {
                to.Set(y, from.Get(y));
            }

            for (int y = 0; y < 10; y++)
            {
                Assert.AreEqual(from.Get<Complex>(y), to.Get<Complex>(y));
            }
        }
        
        
        /// <summary>
        /// Ensures that storage stay constant through expansion
        /// </summary>
        [TestMethod]
        public void Test_Storage_Expansion()
        {
            var vel = new Velocity(323.323f, 1239f);
            ComponentStorage store = new ComponentStorage(typeof(Velocity));
            store.Set(0, vel);
            store.Set(7, vel);

            store.EnsureCapacity(69);

            Assert.AreEqual(vel,store.Get<Velocity>(0));
            Assert.AreEqual(vel, store.Get<Velocity>(7));
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
