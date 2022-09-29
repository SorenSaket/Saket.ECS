using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Tests
{
    [TestClass]
    public class Test_Query
    {
        [TestMethod]
        public void Test_Query_Signature()
        {
            Query a = new Query().With<Position>().With<Velocity>();

            Query b = new Query().With<Velocity>().With<Position>();

            Assert.AreEqual(a.Signature, b.Signature);
        }


    }
}
