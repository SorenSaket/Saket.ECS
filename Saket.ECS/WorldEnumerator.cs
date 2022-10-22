using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public struct WorldEnumerator : IEnumerator<Entity>
    {
        public Entity Current => new Entity(world, new EntityPointer(position, world.entities[position].Version));

        object IEnumerator.Current => Current;

        private World world;
        public int position = -1;


        public WorldEnumerator(World world)
        {
            this.world = world;
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
            while (world.destroyedEntities.Contains(position));
            return (position < world.entities.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
