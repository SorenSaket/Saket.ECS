using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public struct InternalEntityPointer
    {
        public int Archetype = -1;
        public int Row = -1;
        public uint Version = 0;
        //public int ID;

        public InternalEntityPointer(int archetype = -1, int row =-1, uint version = 0)
        {
            Archetype = archetype;
            Row = row;
            Version = version;
        }
    }
}
