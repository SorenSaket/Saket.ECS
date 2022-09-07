using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{

    // TODO. Make allocation free
    //
    public class QueryResult : IEnumerable<Entity>
    {
        public List<int> Entities;
        public List<int> Archetypes;
        public int Count;
        public int Start;
        public int End;

        private readonly World world;

        internal QueryResult(World world, List<int> entities, List<int> archetypes)
        {
            this.world = world;
            this.Entities = entities;
            this.Archetypes = archetypes;
            Start = 0;
            End = Count = entities.Count;
        }

        public Entity this[int index]
        {
            get
            {
                return new Entity(world, world.entities[Entities[index]]);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return new QueryEnumerator(this, 0, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
