using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.ECS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

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
        struct Position : IComponent
        {
            public Vector2 Value;

            public Position(float x, float y)
            {
                Value = new Vector2(x, y);
            }
            public Position(Vector2 value)
            {
                Value = value;
            }
            public static implicit operator Position(Vector2 v) => new Position(v);
        }
        struct Velocity : IComponent
        {
            public Vector2 Value;

            public Velocity(float x, float y)
            {
                Value = new Vector2(x, y);
            }
            public Velocity(Vector2 value)
            {
                Value = value;
            }
            public static implicit operator Velocity(Vector2 v) => new Velocity(v);
        }
        [TestMethod]
        public void TypeArrayHashing()
        {
            HashSet<Type> typesA = new (){ typeof(Velocity), typeof(Position) };

            HashSet<Type> typesB = new (){ typeof(Position), typeof(Velocity) };

            HashSet<Type> typesC = new (){ typeof(Velocity) };

            Assert.AreEqual(Archetype.GetComponentGroupHashCode(typesA), Archetype.GetComponentGroupHashCode(typesB));
            Assert.AreNotEqual(Archetype.GetComponentGroupHashCode(typesC), Archetype.GetComponentGroupHashCode(typesB));
            Assert.AreNotEqual(Archetype.GetComponentGroupHashCode(typesC), Archetype.GetComponentGroupHashCode(typesA));
        }
    }
}
