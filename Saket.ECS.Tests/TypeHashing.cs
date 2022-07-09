using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.ECS;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Saket.ECS.Tests
{
    [TestClass]
    public class TypeHashing
    {
        /// <summary>
        /// Make sure types with generics have unqiue hashcodes
        /// Note that overlap is still theoretically and possibly practically possible.
        /// </summary>
        /*[TestMethod]
        public void GenericHashing()
        {
            Assert.AreNotEqual(typeof(Query<Velocity>).GetHashCode(), typeof(Query<Position>).GetHashCode());

            Assert.AreNotEqual(typeof(Query<Velocity,Position>).GetHashCode(), typeof(Query<Position,Velocity>).GetHashCode());

            Assert.AreNotEqual(typeof(Query<Velocity, Position>).GetHashCode(), typeof(Query<Position, Velocity>).GetHashCode());
        }*/

        [TestMethod]
        public void TypeArrayHashing()
        {
            Type[] typesA = new Type[] { typeof(Velocity), typeof(Position) };

            Type[] typesB = new Type[] { typeof(Position), typeof(Velocity) };

            Type[] typesC = new Type[] { typeof(Velocity) };

            Assert.AreEqual(Archetype.GetComponentGroupHashCode(typesA), Archetype.GetComponentGroupHashCode(typesB));
            Assert.AreNotEqual(Archetype.GetComponentGroupHashCode(typesC), Archetype.GetComponentGroupHashCode(typesB));
            Assert.AreNotEqual(Archetype.GetComponentGroupHashCode(typesC), Archetype.GetComponentGroupHashCode(typesA));
        }
    }
}
