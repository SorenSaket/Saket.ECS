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

            var a = world.CreateEntity();
            Assert.AreEqual((uint)0, a.EntityPointer.Version);
            Assert.AreEqual(0, a.EntityPointer.ID);
            Assert.AreEqual(-1, world.entities[0].Archetype);
            Assert.AreEqual(-1, world.entities[0].Row);
            Assert.AreEqual((uint)0, world.entities[0].Version);

            var b = world.CreateEntity();

            Assert.AreEqual(1, b.EntityPointer.ID);
            Assert.AreEqual((uint)0, b.EntityPointer.Version);
            Assert.AreEqual((uint)0, a.EntityPointer.Version);

            Assert.AreEqual(-1, world.entities[0].Archetype);
            Assert.AreEqual(-1, world.entities[0].Row);
            Assert.AreEqual((uint)0, world.entities[0].Version);

            Assert.AreEqual(-1, world.entities[1].Archetype);
            Assert.AreEqual(-1, world.entities[1].Row);
            Assert.AreEqual((uint)0, world.entities[1].Version);

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
		public void Test_Entity_RemoveComponent()
		{
			World world = new World();

			var e = world.CreateEntity();
			e.Add(new Position(22, 3123));
			e.Add(new Velocity(22, 3123));
			e.Remove<Velocity>();

			Assert.AreEqual(1, world.archetypes[0].Count);
			Assert.AreEqual(0, world.archetypes[1].Count);
		}


		[TestMethod]
        public void Test_Entity_Move()
        {
            var inputPosition = new Position(22, 3123);
            var inputVelocity = new Velocity(455, 98756);

            World world = new World();

            var e = world.CreateEntity();
            Assert.AreEqual(e.EntityPointer.ID, 0);
            e.Add(inputPosition);
            Assert.AreEqual(e.EntityPointer.ID, 0);
            e.Add(inputVelocity);
            Assert.AreEqual(e.EntityPointer.ID, 0);
            Assert.AreEqual(e.Archetype, 1);
            Assert.AreEqual(e.Row, 0);

            var actualPosition = e.Get<Position>();

            Assert.AreEqual(inputPosition, actualPosition);
            Assert.AreEqual(inputVelocity, e.Get<Velocity>());
        }


        [TestMethod]
        public void Test_Entity_AddBundle()
        {
            var inputPosition = new Position(22, 3123);
            var inputVelocity = new Velocity(455, 98756);

            World world = new World();

            var e = world.CreateEntity();
            e.Add(new TestBundle(inputPosition, inputVelocity));

            Assert.AreEqual(inputPosition, e.Get<Position>());
            Assert.AreEqual(inputVelocity, e.Get<Velocity>());
        }

    }
}
