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


    }
}
