using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public class QueryResult : IEnumerable<Entity>
    {
        internal List<EntityPointer> entities;
        public int Count { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        private readonly World world;

        internal QueryResult(World world, List<EntityPointer> entities)
        {
            this.world = world;
            this.entities = entities;
            Start = 0;
            End = Count = entities.Count;
        }

        public Entity this[int index]
        {
            get
            {
                return new Entity(world, entities[index]);
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
