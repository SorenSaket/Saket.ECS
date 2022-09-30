using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public struct ArchetypeEnumerator : IEnumerator<int>
    {
        public int Current => position;

        object IEnumerator.Current => Current;

        public Archetype archetype;
        public int position = -1;
        public int counter = 0;

        public ArchetypeEnumerator(Archetype archetype)
        {
            this.archetype = archetype;
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            do
            {
                position++;
            }
            while (archetype.avaliableRows.Contains(position));
            counter++;
            return (counter < archetype.Count);
        }

        public void Reset()
        {
            position = -1;
            counter = 0;
        }
    }
}
