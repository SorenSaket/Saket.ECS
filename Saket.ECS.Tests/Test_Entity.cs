using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Tests
{
    [TestClass]
    public class Test_Entity
    {
        [TestMethod]
        public void Test_Entity_Creation()
        {
            World world = new World();

            var e = world.CreateEntity();

            Assert.AreEqual(e.EntityPointer, world.entities[0]);
        }

        [TestMethod]
        public void Test_Entity_AddComponent()
        {
            World world = new World();

            var e = world.CreateEntity();
            e.Add(new Position(22, 3123));
            e.Add(new Velocity(22, 3123));

            Assert.IsTrue(Enumerable.SequenceEqual(world.archetypes[0].ComponentTypes, new Type[] { typeof(Position) }));
            Assert.IsTrue(Enumerable.SequenceEqual(world.archetypes[1].ComponentTypes, new Type[] { typeof(Position), typeof(Velocity) }));
        }

        [TestMethod]
        public void Test_Entity_Move()
        {
            var inputPosition = new Position(22, 3123);
            var inputVelocity = new Velocity(455, 98756);

            World world = new World();

            var e = world.CreateEntity();

            e.Add(inputPosition);
            Assert.AreEqual(e.EntityPointer.index_archetype, 0);
            e.Add(inputVelocity);
            Assert.AreEqual(e.EntityPointer.index_archetype, 1);
            Assert.AreEqual(e.EntityPointer.index_row, 0);

            var actualPosition = e.Get<Position>();

            Assert.AreEqual(inputPosition, actualPosition);
            Assert.AreEqual(inputVelocity, e.Get<Velocity>());

        }
    }
}
