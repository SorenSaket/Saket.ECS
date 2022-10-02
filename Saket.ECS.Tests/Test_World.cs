using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saket.ECS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Tests
{
    [TestClass]
    public class Test_World
    {
        [TestMethod]
        public void Test_World_Creation()
        {
            World world = new World();

            //Assert.IsNotNull(world.archetypes);
        }


        [TestMethod]
        public void Test_World_Move()
        {
            var inputComplex = new Complex(22, 3123,false);
            var inputVelocity = new Velocity(455, 98756);

            World world = new World();

            var e = world.CreateEntity();

            e.Add(inputComplex);
            e.Add(inputVelocity);

            // Check that the archetypes have the right amount of entities
            Assert.AreEqual(0, world.archetypes[0].Count);
            Assert.AreEqual(1, world.archetypes[1].Count);


            Assert.AreEqual(inputComplex, world.archetypes[1].storage[typeof(Complex)].Get<Complex>(0));
            Assert.AreEqual(inputVelocity, world.archetypes[1].storage[typeof(Velocity)].Get<Velocity>(0));
        }


        [TestMethod]
        public void Test_World_Clear()
        {

        }

        [TestMethod]
        public void Test_World_Overwrite()
        {
            //World a = new World();
            //var e1 = a.CreateEntity();
            //e1.Add<Velocity>(new Velocity())
        }
    }
}
