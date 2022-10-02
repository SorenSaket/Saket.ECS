using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Tests
{
    [TestClass]
    public class Test_Archetype
    {

        /// <summary>
        /// Tests that the correct components types are set in the archetype
        /// Ensures that the storage is properly initialized
        /// </summary>
        [TestMethod]
        public void Test_Archetype_Creation()
        {/*
            var testTypes = new Type[] { typeof(Position), typeof(Velocity) };

            var a = new Archetype(testTypes);
            
            Assert.IsTrue(Enumerable.SequenceEqual(a.ComponentTypes, testTypes));

            //
            Assert.AreEqual(a.storage.Length, 2);

            */

        }

        [TestMethod]
        public void Test_Archetype_Entity_Add()
        {
            HashSet<Type> testTypes = new() { typeof(Position), typeof(Velocity) };

            var a = new Archetype(testTypes);

            int entity_row = a.AddEntity();

            Assert.AreEqual(0, entity_row);
            Assert.AreEqual(1, a.Count);
            Assert.AreEqual(1, a.Capacity);

            // Check that the storage is initialized

            

        }

        [TestMethod]
        public void Test_Archetype_Entity_Remove()
        {
            HashSet<Type> testTypes = new () { typeof(Position), typeof(Velocity) };

            var a = new Archetype(testTypes);
            int entity_row = a.AddEntity();

            a.RemoveEntity(entity_row);

            Assert.AreEqual(0, a.Count);
            Assert.AreEqual(1, a.Capacity);
        }

        [TestMethod]
        public void Test_Archetype_Entity_Get()
        {
            HashSet<Type> testTypes = new() { typeof(Position), typeof(Velocity) };

            var a = new Archetype(testTypes);
            int entity_row = a.AddEntity();

            a.RemoveEntity(entity_row);

            Assert.AreEqual(0, a.Count);
            Assert.AreEqual(1, a.Capacity);
        }

        [TestMethod]
        public void Test_Archetype_Clear()
        {

        }

        [TestMethod]
        public void Test_Archetype_Overwrite()
        {
            var vel = new Velocity(255, 643);
            var com = new Complex(0.42915f, 1.8456f, true);
            var components = new HashSet<Type>()
            {
                typeof(Complex),
                typeof(Velocity)
            };

            Archetype arc1 = new Archetype(components);
            arc1.AddEntity();
            arc1.Set(0, vel);
            arc1.AddEntity();
            arc1.Set(1, com);


            Archetype arc2 = new Archetype(components);

            arc1.Overwrite(arc2);

            Assert.AreEqual(vel, arc2.Get<Velocity>(0));
            Assert.AreEqual(com, arc2.Get<Complex>(1));

            Assert.AreEqual(2, arc2.Capacity);
            Assert.AreEqual(2, arc2.Count);
        }
    }
}
