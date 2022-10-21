using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public struct InternalEntityPointer
    {
        public int Archetype;
        public int Row;
        public EntityPointer Pointer;

        public InternalEntityPointer(int archetype, int row, EntityPointer pointer)
        {
            Archetype = archetype;
            Row = row;
            Pointer = pointer;
        }
    }
}
